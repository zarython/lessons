using Matjari.Models;

namespace Matjari.Data;

// كلّ ما يمسّ جدول التصنيفات يمرّ من هنا
public static class CategoryRepository
{
    public static List<Category> All()
    {
        var list = new List<Category>();
        using var connection = Database.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT Id, Name FROM Categories ORDER BY Name;";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Category
                { Id = reader.Int("Id"), Name = reader.Str("Name") });
        }

        return list;
    }

    // الاسم فريد: المكرّر تتجاهله القاعدة بلا خطأ
    public static void Insert(Category c) => Write(
        "INSERT OR IGNORE INTO Categories (Name) VALUES ($name);", c);

    public static void Rename(Category c) => Write("""
        UPDATE OR IGNORE Categories SET Name = $name WHERE Id = $id;
        """, c);

    public static void Delete(Category c) =>
        Write("DELETE FROM Categories WHERE Id = $id;", c);

    // كلّ كتابة تمرّ من هنا
    private static void Write(string sql, Category c)
    {
        using var connection = Database.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("$id", c.Id);
        command.Parameters.AddWithValue("$name", c.Name);
        command.ExecuteNonQuery();
    }
}