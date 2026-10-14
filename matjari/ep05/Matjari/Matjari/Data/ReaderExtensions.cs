using Microsoft.Data.Sqlite;

namespace Matjari.Data;

// قراءة الأعمدة باسمها لا برقمها، وتحمّل الفارغ.
public static class ReaderExtensions
{
    public static string Str(this SqliteDataReader r, string column)
    {
        int i = r.GetOrdinal(column);
        return r.IsDBNull(i) ? "" : r.GetString(i);
    }

    public static int Int(this SqliteDataReader r, string column)
    {
        int i = r.GetOrdinal(column);
        return r.IsDBNull(i) ? 0 : r.GetInt32(i);
    }

    public static int? IntOrNull(this SqliteDataReader r, string column)
    {
        int i = r.GetOrdinal(column);
        return r.IsDBNull(i) ? null : r.GetInt32(i);
    }

    public static decimal Dec(this SqliteDataReader r, string column)
    {
        int i = r.GetOrdinal(column);
        return r.IsDBNull(i) ? 0m : (decimal)r.GetDouble(i);
    }

    public static bool Bool(this SqliteDataReader r, string column)
    {
        int i = r.GetOrdinal(column);
        return !r.IsDBNull(i) && r.GetInt32(i) != 0;
    }

}