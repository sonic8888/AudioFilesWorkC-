using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal static class Manager
    {
        /// <summary>
        /// Путь к папке в которой находятся а-файлы и БД.
        /// </summary>
        public static string? pathDestination = @"D:\test";
        /// <summary>
        /// Выбор действия
        /// </summary>
        static string? action = "";


        /// <summary>
        /// Выбор действия работы с аудиофайлами.
        /// </summary>
        public static void PrintMessage()
        {
            Console.WriteLine("Программа работы  с аудиофайлами Яндекс Музыка");
            while (true)
            {
                Console.WriteLine("Укажите путь к папке назначения:");
                pathDestination = Console.ReadLine();
                if (Path.Exists(pathDestination))
                {
                    YandexMusic.PathCopyTo = pathDestination;
                    break;
                }
                else
                    Console.WriteLine("Такого пути не существует.");
            }
            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("y - копирование файлов из Яндекс Музыка в указанную папку.");
                Console.WriteLine("d - копирование файлов из выбранной папки в  указанную папку");
                action = Console.ReadLine();
                switch (action)
                {
                    case "y":
                        Console.WriteLine("вызов метода у");
                        break;
                    case "d":
                        Console.WriteLine("вызов метода d");
                        break;
                    default:
                        Console.WriteLine("действие выбрано не корректно:");
                        break;
                }


            }
        }


        /// <summary>
        /// выбирает информацию о треках в корневой папке "Яндекс Музыка".
        /// </summary>
        /// <returns>массив Track[]</returns>
        public static Track[] GetDataFromYandexDB()
        {
            Track[] tracks = new Track[0];
            try
            {

                string? sours_db = YandexMusic.PathDBSqlite;
                string sql_conn = DbSqlite.Get_str_connection(sours_db);
                List<SqliteParameter> com_params = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", "5" } });
                object? res = DbSqlite.ExecuteScalar(sql_conn, DbSqlite.queries["str1"], com_params);

                if (res != null)
                {
                    int result = Convert.ToInt32(res);
                    tracks = new Track[result];
                    for (int i = 0; i < tracks.Length; i++)
                    {
                        tracks[i] = new Track();
                    }
                    DbSqlite.ExecuteReader(sql_conn, DbSqlite.queries["str2"], ("TrackId", "GetString"), ref tracks, com_params);
                    foreach (var item in tracks)
                    {

                        List<SqliteParameter> lp_title = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", item.TrackId! } });
                        DbSqlite.ExecuteReader(sql_conn, DbSqlite.queries["str3"], ("Name", "GetString"), item, lp_title);
                        DbSqlite.ExecuteReader(sql_conn, DbSqlite.queries["str4"], ("ArtistId", "GetString"), item, lp_title);
                        List<SqliteParameter> lp_artist = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", item.ArtistId! } });
                        DbSqlite.ExecuteReader(sql_conn, DbSqlite.queries["str5"], ("Artist", "GetString"), item, lp_artist);

                    }
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            return tracks;
        }

        /// <summary>
        /// Возвращает массив треков - разницу корневой папки Яндекс Музыка 
        /// и папки назначения (pathDestination).
        /// </summary>
        /// <returns>Track[]</returns>
        public static Track[] GetDifferenceYandexAndDestination()
        {
            Track[] tracksY = GetDataFromYandexDB();
            Track[] trackD = GetDataFromPathDestination();
            var res = tracksY.Except(trackD);
            Track[] result = res.ToArray();
            return result;
        }

        /// <summary>
        /// получаем количество треков в папке "pathDestination" и создаем массив пустых "Track"
        /// </summary>
        /// <returns>массив Track[]</returns>
        static Track[] GetEmptyTrackFromDB()
        {
            Track[] tracks = new Track[0];
            try
            {
                string sours_dir = YandexMusic.GetPathDbSqliteDestination();
                string str_connection = DbSqlite.Get_str_connection(sours_dir);
                object? res = DbSqlite.ExecuteScalar(str_connection, DbSqlite.queries["str6"]);
                if (res != null)
                {
                    int result = Convert.ToInt32(res);
                    tracks = new Track[result];
                    for (int i = 0; i < tracks.Length; i++)
                    {
                        tracks[i] = new Track();
                    }

                }
            }

            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            return tracks;
        }

        /// <summary>
        /// вставляем в БД "NameMyDB" данные из "Track"
        /// </summary>
        /// <param name="tracks">содержит данные о треке</param>
        public static void InsertData(Track[] tracks)
        {

            string pathDbDestination = YandexMusic.GetPathDbSqliteDestination();
            foreach (Track item in tracks)
            {
                var dicParam = new Dictionary<string, string?>();
                dicParam.Add("@name", item.Name);
                dicParam.Add("@artist", item.Artist);
                dicParam.Add("@trackid", item.TrackId);
                dicParam.Add("@artistid", item.ArtistId);
                dicParam.Add("@nameartist", item.NameArtist);
                dicParam.Add("@data", YandexMusic.Data);
                var comParams = DbSqlite.Get_list_params(dicParam);
                DbSqlite.ExecuteNonQuery(DbSqlite.Get_str_connection(pathDbDestination), DbSqlite.queries["str_insert"], comParams);

            }

        }


        /// <summary>
        /// Извлекает данные из БД и упаковывает их в массив 'Track'.
        /// </summary>
        /// <returns>Track[]</returns>
        public static Track[] GetDataFromPathDestination()
        {
            Track[] tracks = GetEmptyTrackFromDB();
            if (tracks != null)
            {
                string sours_dir = YandexMusic.GetPathDbSqliteDestination();
                string str_connection = DbSqlite.Get_str_connection(sours_dir);
                (string, string)[] pr_meth = new (string, string)[] { ("Name", "GetString"), ("Artist", "GetString"), ("TrackId", "GetString"), ("ArtistId", "GetString"), ("Name_Artist", "GetString"), ("DataCreate", "GetString") };
                DbSqlite.ExecuteReader(str_connection, DbSqlite.queries["str7"], pr_meth, ref tracks);
            }
            return tracks!;
        }

        /// <summary>
        /// Выводит на консоль последовательность.
        /// </summary>
        /// <param name="t">последовательнось</param>
        public static void Display(IEnumerable t)
        {

            foreach (var item in t)
            {
                Console.WriteLine(item);

            }
        }


        /// <summary>
        /// Копирует недостающие треки из папки Яндекс Музыка в папку назначения и заносит данные в БД.
        /// </summary>
        /// <returns>true если успешно</returns>
        /// <exception cref="Exception">ошибки при копировании и  при вставке данных в БД</exception>
        public static bool CopyFromYandexMusic()
        {
            bool isSuccessfully = false;
            try
            {
                Track[] tracks = GetDifferenceYandexAndDestination();

                foreach (var item in tracks)
                {
                    YandexMusic.CopyTo(item, YandexMusic.PathYandexMusicDir, YandexMusic.PathCopyTo);
                    Console.WriteLine("Вносим в БД");
                }
                Manager.InsertData(tracks);
                isSuccessfully = true;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return isSuccessfully;
        }
    }
}
