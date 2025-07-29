using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal static class DbSqlite
    {
        public static string Get_str_connection(string? data_sours, string mode = "ReadOnly", Dictionary<string, string>? paramss = null)
        {
            string str = $"Data Source={data_sours};Mode={mode};";
            if (paramss != null)
            {
                foreach (var item in paramss)
                {
                    string param = $"{item.Key}={item.Value};";
                    str += item;
                }
            }
            return str;
        }

        public static int ExecuteScalar(string str_connection, string sqlExpression, List<SqliteParameter>? sql_params=null)
        {
            object? val = 0;
            int res = 0;
            using (var connection = new SqliteConnection(str_connection))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                if (sql_params != null)
                {
                    foreach (var item in sql_params)
                    {
                        command.Parameters.Add(item);
                    }
                }
                    val = command.ExecuteScalar();
            }
            res = Convert.ToInt32(val);
            return res;
        }

        public static SqliteParameter Get_sql_parametr(string name, int value)
        {
            SqliteParameter par = new SqliteParameter(name, value);
            return par;

        }

        public static SqliteParameter Get_sql_parametr(string name, string value)
        {
            SqliteParameter par = new SqliteParameter(name, value);
            return par;

        }
    }
}
