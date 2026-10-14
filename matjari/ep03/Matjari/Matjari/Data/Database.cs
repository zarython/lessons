using Microsoft.Data.Sqlite;

namespace Matjari.Data;

// المكان الوحيد الذي يعرف أين القاعدة وكيف تُفتح.
public static class Database
{
    public static string DataFolder => Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData),
        "Matjari");

    public static string DatabasePath =>
        Path.Combine(DataFolder, "matjari.db");

    // مفاتيح الربط مطفأة في SQLite ما لم تُطلب.
    public static string ConnectionString =>
        new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            ForeignKeys = true
        }.ToString();

    public static SqliteConnection Open()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public static void Initialize()
    {
        Directory.CreateDirectory(DataFolder);
        using var connection = Open();
        CreateSchema(connection);
    }

    // جداول البرنامج، تُصنع مرّة واحدة.
    private static void CreateSchema(
        SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Categories (
                Id   INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE
            );
            CREATE TABLE IF NOT EXISTS Products (
                Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                Code            TEXT NOT NULL DEFAULT '',
                Barcode         TEXT UNIQUE,
                Name            TEXT NOT NULL,
                CategoryId      INTEGER
                    REFERENCES Categories(Id) ON DELETE SET NULL,
                Unit            TEXT NOT NULL DEFAULT 'قطعة',
                CostPrice       REAL NOT NULL DEFAULT 0,
                SalePrice       REAL NOT NULL DEFAULT 0,
                Quantity        REAL NOT NULL DEFAULT 0,
                ReorderLevel    REAL NOT NULL DEFAULT 0,
                IsActive        INTEGER NOT NULL DEFAULT 1,
                Notes           TEXT NOT NULL DEFAULT '',
                CreatedAt       TEXT NOT NULL,
                ExpiryDate      TEXT,
                SourceProductId INTEGER
                    REFERENCES Products(Id) ON DELETE SET NULL
            );
            CREATE INDEX IF NOT EXISTS IX_Products_Name
                ON Products(Name);

            """;
        command.ExecuteNonQuery();
    }

}