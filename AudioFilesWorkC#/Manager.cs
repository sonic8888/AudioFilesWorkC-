using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            foreach (Track track in tracksY)
            {
                Manager.NormalizeNameTrack(track);
            }
            //foreach (Track track in trackD)
            //{
            //    Manager.NormalizeNameTrack(track);
            //}

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
        /// вставляем в БД "NameMyDB" данные из  массива Track
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
        /// Записываем данные из Track в БД(NameMyDB).
        /// </summary>
        /// <param name="track">Track</param>
        /// <param name="pathDbDestination">путь к папке назначения</param>
        /// <returns>номер строки в БД</returns>
        public static int InsertData(Track track, string pathDbDestination)
        {
            var dicParam = new Dictionary<string, string?>();
            dicParam.Add("@name", track.Name);
            dicParam.Add("@artist", track.Artist);
            dicParam.Add("@trackid", track.TrackId);
            dicParam.Add("@artistid", track.ArtistId);
            dicParam.Add("@nameartist", track.NameArtist);
            dicParam.Add("@data", YandexMusic.Data);
            var comParams = DbSqlite.Get_list_params(dicParam);
            int rows = DbSqlite.ExecuteNonQuery(DbSqlite.Get_str_connection(pathDbDestination), DbSqlite.queries["str_insert"], comParams);
            return rows;
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
        /// <param name="t">IEnumerable</param>
        public static void Display(IEnumerable t)
        {

            foreach (var item in t)
            {
                Console.WriteLine(item);

            }
        }


        public static void CheckDirDestination()
        {
            Track[] trackD = GetDataFromPathDestination();
            var files = Directory.GetFiles(Manager.pathDestination!);
            Console.WriteLine(files.Length);
            var files_mp3 = files.Count(f => f.EndsWith(".mp3"));
            Console.WriteLine(files_mp3);
        }


        /// <summary>
        /// Копирует недостающие треки из папки Яндекс Музыка в папку назначения и заносит данные в БД.
        /// </summary>
        /// <returns>true если успешно</returns>
        /// <exception cref="Exception">ошибки при копировании и  при вставке данных в БД</exception>
        public static bool CopyFromYandexMusic()
        {
            bool isSuccessfully = false;
            string pathDbDestination = YandexMusic.GetPathDbSqliteDestination();
            Track[] tracks = GetDifferenceYandexAndDestination();

            try
            {
                foreach (var item in tracks)
                {
                    bool isException;
                    YandexMusic.CopyTo(item, YandexMusic.PathMusicDirYandex, YandexMusic.PathCopyTo, out isException, true, true);
                    if (isException)
                    {
                        Manager.InsertData(item, pathDbDestination);
                        Manager.DisplayTrack(item);

                    }
                }
                Console.Clear();
                isSuccessfully = true;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return isSuccessfully;
        }

        static void DisplayPercent(int count, int length)
        {
            if (length != 0)
            {
                double five = 5;
                double percent = (count * 100) / length;
                percent = Math.Round(percent);
                if (percent % five == 0)
                {
                    Console.Clear();
                    Console.Write(percent);
                }

            }

        }

        public static void DisplayColor(string message, System.ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void DisplayTrack(Track track) => Console.WriteLine(track);

        public static string NormalizeName(string? name)
        {
            if (name == null) return "";
            string pattern = @"\W";
            string target = ".";
            Regex regex = new Regex(pattern);
            string result = regex.Replace(name, target);
            return result;
        }

        public static Track NormalizeNameTrack(Track tr)
        {
            tr.Name = NormalizeName(tr.Name);
            tr.Artist = NormalizeName(tr.Artist);
            return tr;
        }
    }
}
