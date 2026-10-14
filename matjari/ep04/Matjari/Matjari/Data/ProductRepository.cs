using Matjari.Models;
using Microsoft.Data.Sqlite;

namespace Matjari.Data;

// كلّ ما يُقرأ من جدول الأصناف يمرّ من هنا.
public static class ProductRepository
{
    private const string SelectSql = """
        SELECT p.Id, p.Code, p.Barcode, p.Name, p.CategoryId,
               p.Unit, p.CostPrice, p.SalePrice,
               p.Quantity, p.ReorderLevel, p.IsActive,
               IFNULL(c.Name, '') AS CategoryName
        FROM Products p
        LEFT JOIN Categories c ON c.Id = p.CategoryId
        """;

    private static Product Read(SqliteDataReader r) => new()
    {
        Id = r.Int("Id"),
        Code = r.Str("Code"),
        Barcode = r.Str("Barcode"),
        Name = r.Str("Name"),
        CategoryId = r.IntOrNull("CategoryId"),
        CategoryName = r.Str("CategoryName"),
        Unit = r.Str("Unit"),
        CostPrice = r.Dec("CostPrice"),
        SalePrice = r.Dec("SalePrice"),
        Quantity = r.Dec("Quantity"),
        ReorderLevel = r.Dec("ReorderLevel"),
        IsActive = r.Bool("IsActive")
    };

    public static List<Product> Search(string term = "")
    {
        var list = new List<Product>();
        using var connection = Database.Open();
        using var command = connection.CreateCommand();

        string sql = SelectSql + " WHERE 1 = 1";

        if (!string.IsNullOrWhiteSpace(term))
        {
            sql += " AND (p.Name LIKE $term";
            sql += " OR p.Code LIKE $term";
            sql += " OR p.Barcode LIKE $term)";
            command.Parameters.AddWithValue(
                "$term", "%" + term.Trim() + "%");
        }

        sql += " ORDER BY p.Name;";

        command.CommandText = sql;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(Read(reader));
        }

        return list;
    }

    // الكود: أكبر كود وواحد، وأوّله 100
    public static void Insert(Product p) => Write("""
        INSERT INTO Products
            (Code, Barcode, Name, SalePrice, Quantity, CreatedAt)
        VALUES (
            (SELECT IFNULL(MAX(CAST(Code AS INTEGER)), 99) + 1
             FROM Products),
            $barcode, $name, $price, $qty,
            datetime('now', 'localtime'));
        """, p);

    // يعدّل الصفّ الذي رقمه رقم الصنف فقط
    public static void Update(Product p) => Write("""
        UPDATE Products SET Barcode = $barcode, Name = $name,
            SalePrice = $price, Quantity = $qty
        WHERE Id = $id;
        """, p);

    public static void Delete(Product p) =>
        Write("DELETE FROM Products WHERE Id = $id;", p);

    // كلّ كتابة تمرّ من هنا
    private static void Write(string sql, Product p)
    {
        using var connection = Database.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        // الباركود الفارغ «لا شيء»، فلا يتكرّر
        object barcode = p.Barcode == "" ? DBNull.Value : p.Barcode;

        command.Parameters.AddWithValue("$id", p.Id);
        command.Parameters.AddWithValue("$barcode", barcode);
        command.Parameters.AddWithValue("$name", p.Name);
        command.Parameters.AddWithValue("$price", p.SalePrice);
        command.Parameters.AddWithValue("$qty", p.Quantity);
        command.ExecuteNonQuery();
    }

}