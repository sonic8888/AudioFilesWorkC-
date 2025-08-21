using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal static class DbSqlite
    {
        public static string[] values = new string[] { "5" };
        public static Dictionary<string, string> Dictionary_query = new Dictionary<string, string>()
        {
            {"str1","SELECT Count(TrackId) FROM T_PlaylistTrack WHERE Kind = @value;" },
            {"str2", "SELECT TrackId FROM T_PlaylistTrack WHERE Kind = @value;" },
            {"str10", "SELECT AlbumId FROM T_TrackAlbum WHERE TrackId = @value;" },
            {"str3", "SELECT Title FROM T_Track WHERE Id = @value" },
            {"str4", "SELECT ArtistId FROM T_TrackArtist WHERE TrackId = @value" },
            {"str5", "SELECT Name FROM T_Artist WHERE Id = @value" },
            {"str6", "SELECT Count(Name) From T_Trask_Yandex" },
            {"str7","SELECT * FROM  T_Trask_Yandex " },
            {"del", "DELETE FROM T_Trask_Yandex WHERE Id = @value" },
            {"str9", "SELECT Year FROM T_Album WHERE Id = @value" },
            {"str_create", "CREATE TABLE T_Trask_Yandex (Id  INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE  NOT NULL, Title  VARCHAR, Artist  VARCHAR, Album VARCHAR, Year VARCHAR, TrackId  VARCHAR, Data  VARCHAR );" },
            {"str_insert","INSERT INTO T_Trask_Yandex (Title, Artist, Album, Year, TrackId, Data)  VALUES (@title, @artist, @album, @year, @track_id, @data); SELECT last_insert_rowid();" },
            { "str11", "SELECT Title, Year, ArtistsString FROM T_Album WHERE Id = @value" },
            { "str12", "SELECT  TrackId FROM T_Trask_Yandex" }
        };

        public static string PathCopyTo = @"D:\test";
        public static string NameMyDB = "my_music.sqlite";
        public static string Get_str_connection(string? data_sours, string mode = "ReadWriteCreate", Dictionary<string, string>? params_com = null)
        {
            string str = $"Data Source={data_sours};Mode={mode};";
            if (params_com != null)
            {
                foreach (var item in params_com)
                {
                    string param = $"{item.Key}={item.Value};";
                    str += item;
                }
            }
            return str;
        }

        public static object? ExecuteScalar(string str_connection, string sqlExpression, List<SqliteParameter>? sql_params = null)
        {
            object? val = 0;
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
            return val;
        }

        public static SqliteParameter Get_sql_params(string name, int value)
        {
            SqliteParameter par = new SqliteParameter(name, value);
            return par;

        }

        public static SqliteParameter Get_sql_params(string name, string? value)
        {

            SqliteParameter par = new SqliteParameter(name, value);
            par.IsNullable = true;
            return par;

        }

        public static List<SqliteParameter> Get_list_params(Dictionary<string, string?> param_val)
        {
            List<SqliteParameter> sqliteParameters = new List<SqliteParameter>();
            foreach (var item in param_val)
            {
                sqliteParameters.Add(Get_sql_params(item.Key, item.Value));
            }
            return sqliteParameters;
        }

        public static void ExecuteReader(string str_connection, string sqlExpression, (string, string) property_method, Track[] tracks, List<SqliteParameter>? sql_params = null)
        {
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
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        int n = 0;
                        var type_track = typeof(Track);
                        var property_track = type_track.GetProperty(property_method.Item1);
                        var type_reader = reader.GetType();
                        var method_reader = type_reader.GetMethod(property_method.Item2);
                        while (reader.Read())   // построчно считываем данные
                        {
                            Track track = tracks[n++];
                            var res = method_reader?.Invoke(reader, parameters: new object[] { 0 });
                            property_track?.SetValue(track, res);


                        }
                    }
                }

            }
        }
        public static List<string> ExecuteReader(string str_connection, string sqlExpression, List<SqliteParameter>? sql_params = null)
        {
            List<string> list_data = new List<string>();
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
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        while (reader.Read())
                        {
                            var result = reader.GetString(0);
                            list_data.Add(result);
                        }
                    }
                }
            }
            return list_data;
        }




        public static void ExecuteReader(string str_connection, string sqlExpression, (string, string) property_method, Track track, List<SqliteParameter>? sql_params = null)
        {
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
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        var type_track = typeof(Track);
                        var property_track = type_track.GetProperty(property_method.Item1);
                        var type_reader = reader.GetType();
                        var method_reader = type_reader.GetMethod(property_method.Item2);
                        while (reader.Read())
                        {
                            var res = method_reader?.Invoke(reader, parameters: new object[] { 0 });
                            property_track?.SetValue(track, res);


                        }
                    }
                }

            }
        }

        public static void ExecuteReader(string str_connection, string sqlExpression, (string, string)[] property_method, Track track, List<SqliteParameter>? sql_params = null)
        {
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
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        var type_track = typeof(Track);
                        var type_reader = reader.GetType();
                        Queue<PropertyInfo?> property = new Queue<PropertyInfo?>();
                        Queue<MethodInfo?> methods = new Queue<MethodInfo?>();
                        foreach (var item in property_method)
                        {
                            var property_track = type_track.GetProperty(item.Item1);
                            property.Enqueue(property_track);
                            var method_reader = type_reader.GetMethod(item.Item2);
                            methods.Enqueue(method_reader);
                        }
                        while (reader.Read())
                        {
                            for (int i = 0; i < property_method.Length; i++)
                            {
                                var method_reader = methods.Dequeue();
                                var res = method_reader?.Invoke(reader, parameters: new object[] { i });
                                var property_track = property.Dequeue();
                                property_track?.SetValue(track, res);

                            }
                        }
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string str_connection, string sqlExpression, List<SqliteParameter>? sql_params = null)
        {
            int rows = -1;
            try
            {
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
                    rows = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Manager.DisplayColor(ex.Message, ConsoleColor.Yellow);
            }

            return rows;
        }

        /// <summary>
        /// Находит путь к БД папки назначения, если нет то создает БД.
        /// </summary>
        /// <returns>Путь к БД</returns>
        /// <exception cref="ArgumentException">Если не нашел папку назначения</exception>
        public static string GetPathDbSqliteDestination()
        {
            if (Path.Exists(PathCopyTo))
            {
                string _sours_db = Path.Combine(PathCopyTo, DbSqlite.NameMyDB);
                if (Path.Exists(_sours_db))
                    return _sours_db;
                else
                {
                    string _str_connection = DbSqlite.Get_str_connection(_sours_db);
                    DbSqlite.ExecuteNonQuery(_str_connection, DbSqlite.Dictionary_query["str_create"]);
                    return _sours_db;
                }

            }
            else
            {
                throw new ArgumentException($"{PathCopyTo} - There is not such way");
            }



        }
    }
}
