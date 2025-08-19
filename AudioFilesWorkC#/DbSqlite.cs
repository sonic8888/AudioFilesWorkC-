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
        public static Dictionary<string, string> queries = new Dictionary<string, string>()
        {
            {"str1","SELECT Count(TrackId) FROM T_PlaylistTrack WHERE Kind = @value;" },
            {"str2", "SELECT TrackId FROM T_PlaylistTrack WHERE Kind = @value;" },
            {"str10", "SELECT AlbumId FROM T_TrackAlbum WHERE TrackId = @value;" },
            {"str3", "SELECT Title FROM T_Track WHERE Id = @value" },
            {"str4", "SELECT ArtistId FROM T_TrackArtist WHERE TrackId = @value" },
            {"str5", "SELECT Name FROM T_Artist WHERE Id = @value" },
            {"str6", "SELECT Count(Name) From T_Trask_Yandex" },
            {"str7","SELECT * FROM  T_Trask_Yandex " },
            {"str8", "SELECT Title FROM T_Album WHERE Id = @value" },
            {"str9", "SELECT Year FROM T_Album WHERE Id = @value" },
            {"str_create", "CREATE TABLE T_Trask_Yandex (Id  INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE  NOT NULL, Title  VARCHAR, Artist  VARCHAR, Album VARCHAR, Year VARCHAR, TrackId  VARCHAR, ArtistId  VARCHAR, NameArtist   VARCHAR, Data  VARCHAR );" },
            {"str_insert","INSERT INTO T_Trask_Yandex (Title, Artist,Album, Year, TrackId, ArtistId, NameArtist, Data)  VALUES (@title, @artist,@album, @year, @track_id, @artist_id, @name_artist, @data)" }
        };


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

        public static void ExecuteReader(string str_connection, string sqlExpression, (string, string) property_method, ref Track[] tracks, List<SqliteParameter>? sql_params = null)
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



        public static void ExecuteReader(string str_connection, string sqlExpression, (string, string)[] property_method_array, ref Track[] tracks, List<SqliteParameter>? sql_params = null)
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
                        var type_reader = reader.GetType();
                        int lenght = property_method_array.Length;
                        PropertyInfo?[] pi = new PropertyInfo[lenght];
                        MethodInfo?[] mi = new MethodInfo[lenght];
                        for (int i = 0; i < lenght; i++)
                        {
                            var pr_m = property_method_array[i];
                            pi[i] = type_track.GetProperty(pr_m.Item1);
                            mi[i] = type_reader.GetMethod(pr_m.Item2);
                        }
                        while (reader.Read())   // построчно считываем данные
                        {
                            Track track = tracks[n++];
                            for (int i = 0; i < lenght; i++)
                            {
                                MethodInfo? method = mi[i];
                                PropertyInfo? property = pi[i];
                                var res = method?.Invoke(reader, parameters: new object[] { i + 1 });
                                property?.SetValue(track, res);
                            }




                        }
                    }
                }

            }
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

        public static int ExecuteNonQuery(string str_connection, string sqlExpression, List<SqliteParameter>? sql_params = null)
        {
            int rows = -1;
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
            return rows;
        }
    }
}
