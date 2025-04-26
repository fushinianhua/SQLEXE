using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public static class SQLHelper
{
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

    // 执行查询，返回 DataTable
    public static DataTable ExecuteDataTable(string query, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }
            connection.Open();
            DataTable dataTable = new DataTable();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }
            return dataTable;
        }
    }

    // 执行增删改，返回受影响的行数
    public static int ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand command = new SqlCommand(commandText, connection))
        {
            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }

    // 创建 SqlParameter 的快捷方法
    public static SqlParameter CreateParameter(string name, object value)
    {
        return new SqlParameter(name, value ?? DBNull.Value);
    }
}