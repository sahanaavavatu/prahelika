using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public static class DbHelper
{
    // Telugu: కనెక్షన్ స్ట్రింగ్ వెబ్.config లో "DefaultConnection" అనే key లో పెట్టాలి
    private static string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

    // Telugu: SELECT queries కోసం
    public static DataTable ExecuteDataTable(string query)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    // Telugu: INSERT, UPDATE, DELETE కోసం
    public static int ExecuteNonQuery(string query)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            conn.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
