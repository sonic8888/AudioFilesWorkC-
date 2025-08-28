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
        /// <summary>
        /// Словарь содержащий SQL запросы.
        /// </summary>
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
            {"str_create", "CREATE TABLE T_Trask_Yandex (Id  INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE  NOT NULL, Name VARCHAR,  Title  VARCHAR, Artist  VARCHAR, Album VARCHAR, Year VARCHAR, TrackId  VARCHAR, Data  VARCHAR, Sours VARCHAR );" },
            {"str_insert","INSERT INTO T_Trask_Yandex (Name, Title, Artist, Album, Year, TrackId, Data, Sours)  VALUES (@name, @title, @artist, @album, @year, @track_id, @data, @sours); SELECT last_insert_rowid();" },
            { "str11", "SELECT Title, Year, ArtistsString FROM T_Album WHERE Id = @value" },
            { "str12", "SELECT  TrackId FROM T_Trask_Yandex WHERE Sours = @value" },
            { "str13", "SELECT TrackId FROM T_Trask_Yandex" }
        };

        private static string PathCopyTo = Manager.pathDestination;
        public static string NameMyDB = "my_music.sqlite";
        /// <summary>
        /// Создает строку подключения к БД
        /// </summary>
        /// <param name="data_sours">Абсолютный путь к БД</param>
        /// <param name="mode">Режим подключения к БД(ReadWriteCreate-по умолчанию)</param>
        /// <param name="params_com">Словарь с дополнительными параметрами подключения(null-по умолчанию)</param>
        /// <returns>Строка поключения</returns>
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

        /// <summary>
        /// Выполняет запрос к БД и возвращает одно скалярное значение.
        /// (возвращает количество треков в папке "Яндекс Музыка").
        /// </summary>
        /// <param name="str_connection">Строка подключения</param>
        /// <param name="sqlExpression">SQL запрос к БД.</param>
        /// <param name="sql_params">Параметры SQL запроса.</param>
        /// <returns>объект класса "Object?"</returns>
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

        /// <summary>
        /// Создает объект "SqliteParameter". 
        /// </summary>
        /// <param name="name">Имя параметра запроса</param>
        /// <param name="value">Значение параметра запроса</param>
        /// <returns> SqliteParameter</returns>
        public static SqliteParameter Get_sql_params(string name, string? value)
        {

            SqliteParameter par = new SqliteParameter(name, value);
            par.IsNullable = true;
            return par;

        }

        /// <summary>
        /// Создает список параметров запроса.
        /// </summary>
        /// <param name="param_val">Словарь содержащий название параметра и его значение.</param>
        /// <returns>Список парамеров запроса.</returns>
        public static List<SqliteParameter> Get_list_params(Dictionary<string, string?> param_val)
        {
            List<SqliteParameter> sqliteParameters = new List<SqliteParameter>();
            foreach (var item in param_val)
            {
                sqliteParameters.Add(Get_sql_params(item.Key, item.Value));
            }
            return sqliteParameters;
        }

        /// <summary>
        /// Извлекает данные из БД(SELECT).
        /// </summary>
        /// <param name="str_connection">Строка подключения к БД.</param>
        /// <param name="sqlExpression">SQL запрос к БД.</param>
        /// <param name="sql_params">Список содержащий параметры запроса и их значений.</param>
        /// <returns>Список содержащий результаты запроса к БД.</returns>
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
                try
                {
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
                catch (SqliteException se)
                {

                    Manager.DisplayColor(se.Message, ConsoleColor.Red);
                }

            }
            return list_data;
        }

        /// <summary>
        /// Извлекает данные из БД(SELECT).
        /// </summary>
        /// <param name="str_connection">Строка подключения к БД.</param>
        /// <param name="sqlExpression">SQL запрос к БД.</param>
        /// <param name="property_method">Кортеж содержащий название свойства класса "Track" и название метода класса "SqliteDataReader"</param>
        /// <param name="track">Объект класса "Track"/</param>
        /// <param name="sql_params">Список содержащий параметры запроса БД и их значений.</param>
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

        /// <summary>
        /// Извлекает данные из БД(SELECT).
        /// </summary>
        /// <param name="str_connection">Строка подключения к БД.</param>
        /// <param name="sqlExpression">SQL запрос к БД.</param>
        /// <param name="property_method">Массив кортежей содержащих название свойства класса "Track" и название метода класса "SqliteDataReader"</param>
        /// <param name="track">Объект класса "Track"</param>
        /// <param name="sql_params">Список содержащий параметры запроса БД и их значений</param>
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

        /// <summary>
        /// Выполняет запрос к БД(INSERT, UPDATE, DELETE, CREATE).
        /// </summary>
        /// <param name="str_connection">Строка подключения к БД.</param>
        /// <param name="sqlExpression">SQL запрос к БД.</param>
        /// <param name="sql_params">Список содержащий параметры запроса БД и их значений</param>
        /// <returns>Число сток (INSERT, UPDATE, DELETE)</returns>
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
