using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using TagLib.Ape;
using TagLib.Matroska;

namespace AudioFilesWorkC_
{
    internal static class Manager
    {
        /// <summary>
        /// Путь к папке в которой находятся а-файлы и БД.
        /// </summary>
        public static string pathDestination = @"D:\test";
        public static string? pathFrom = @"D:\music\test";
        private static string Pattern = @"[\*\|\\\:\""<>\?\/]";
        private static string Target = ".";
        private static Regex regex = new Regex(Manager.Pattern);
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
                var path = Console.ReadLine();
                if (path != null) pathDestination = path;
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
            string? sours_db = YandexMusic.PathDBSqlite;
            string sql_conn = DbSqlite.Get_str_connection(sours_db);
            List<SqliteParameter> com_params = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", "5" } });
            object? res = DbSqlite.ExecuteScalar(sql_conn, DbSqlite.Dictionary_query["str1"], com_params);

            if (res != null)
            {
                int result = Convert.ToInt32(res);
                tracks = new Track[result];
                for (int i = 0; i < tracks.Length; i++)
                {
                    tracks[i] = new Track();
                }
                DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str2"], ("TrackId", "GetString"), tracks, com_params);
                foreach (var item in tracks)
                {
                    TrackContentFromYAndexApp(item, sql_conn);
                }
            }
            return tracks;
        }

        private static void CreateTags(Track track, string path_dir)
        {
            try
            {
                var tfile = TagLib.File.Create(Path.Combine(path_dir, track.Name + track.Extension));
                tfile.Tag.Title = track.Title;
                tfile.Tag.Album = track.Album;
                try
                {
                    uint year = Convert.ToUInt32(track.Year);
                    tfile.Tag.Year = year;
                }
                catch (Exception)
                {

                    tfile.Tag.Year = 0;
                }
                tfile.Tag.Performers = new string[] { track.Artist! };
                tfile.Tag.TrackCount = Convert.ToUInt32(track.TrackId);
                tfile.Tag.DateTagged = DateTime.Now;
                tfile.Save();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// вставляем в БД "NameMyDB" данные из  массива Track
        /// и копируем файлы в папку 
        /// </summary>
        /// <param name="tracks">содержит данные о треке</param>
        public static void CopyInsertDataFromYandexAppToDestination(Track[] tracks)
        {
            YandexMusic.PathCopyTo = Manager.pathDestination;
            string pathDbDestination = DbSqlite.GetPathDbSqliteDestination();
            foreach (Track item in tracks)
            {
                int rows = InsertData(item, pathDbDestination);
                if (rows > 0)
                {
                    bool isException;
                    YandexMusic.CopyTo(item, YandexMusic.PathMusicDirYandex, YandexMusic.PathCopyTo, out isException, false);
                    if (!isException)//если копирование не получилось то удаляем данные из БД
                    {
                        Manager.DisplayColor($"DELETE: rows:{rows}, track:{item}", ConsoleColor.Red);
                        DbSqlite.ExecuteNonQuery(DbSqlite.Get_str_connection(pathDbDestination),
                DbSqlite.Dictionary_query["del"], DbSqlite.Get_list_params(new Dictionary<string, string?>() { { "@value", rows.ToString() } }));
                        continue;
                    }
                    else
                    {
                        CreateTags(item, Manager.pathDestination);
                    }

                }
            }
        }


        /// <summary>
        /// Записываем данные из Track в БД(NameMyDB).
        /// </summary>
        /// <param name="track">Track</param>
        /// <param name="pathDbDestination">путь к папке назначения</param>
        /// <returns>номер строки в БД или -1</returns>
        public static int InsertData(Track track, string pathDbDestination, string sours = "Yandex")
        {
            int rows = -1;
            try
            {
                var dicParam = new Dictionary<string, string?>();
                dicParam.Add("@name", track.Name + track.Extension);
                dicParam.Add("@title", track.Title);
                dicParam.Add("@artist", track.Artist ?? "unknown");
                dicParam.Add("@album", track.Album ?? "unknown");
                dicParam.Add("@year", track.Year ?? "unknown");
                dicParam.Add("@track_id", track.TrackId);
                dicParam.Add("@data", Track.Data());
                dicParam.Add("@sours", sours);
                var comParams = DbSqlite.Get_list_params(dicParam);
                var r = DbSqlite.ExecuteScalar(DbSqlite.Get_str_connection(pathDbDestination), DbSqlite.Dictionary_query["str_insert"], comParams);
                rows = Convert.ToInt32(r);
            }
            catch (Exception ex)
            {
                Console.WriteLine(track);
                Manager.DisplayColor(ex.Message, ConsoleColor.Cyan);
            }

            return rows;
        }



        public static void DisplayColor(string message, System.ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Display<T>(IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
            {
                Console.WriteLine(item);
            }
        }
        public static void DisplayFileInfo(IEnumerable<FileInfo> fileInfo)
        {
            foreach (var item in fileInfo) { Console.WriteLine(item.FullName); }
        }



        private static List<string> GetProperty(Track[] track, string nameProperty, bool isAddExtension = true)
        {
            Type track_type = typeof(Track);
            var property = track_type.GetProperty(nameProperty);
            if (property == null) throw new ArgumentNullException(nameof(property));
            var propertys = new List<string>();
            foreach (var item in track)
            {
                var value = property.GetValue(item);
                if (value == null) continue;
                string value_property = "";
                if (isAddExtension)
                {
                    value_property = value.ToString()! + item.Extension;
                }
                else
                {
                    value_property = value.ToString()!;
                }
                propertys.Add(value_property);
            }
            return propertys;
        }

        public static List<Track> GetDifferentTracks()
        {
            string path = DbSqlite.GetPathDbSqliteDestination();
            string str_connection = DbSqlite.Get_str_connection(path);
            List<SqliteParameter> com_params_destination = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", "Yandex" } });
            var list_trackId_destination = DbSqlite.ExecuteReader(str_connection, DbSqlite.Dictionary_query["str12"], com_params_destination);

            string? sours_db = YandexMusic.PathDBSqlite;
            string sql_conn = DbSqlite.Get_str_connection(sours_db);
            List<SqliteParameter> com_params_yandex = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", "5" } });
            var list_trackId_yandex = DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str2"], com_params_yandex);
            foreach (var item in list_trackId_destination)
            {
                if (list_trackId_yandex.Contains(item))
                {
                    list_trackId_yandex.Remove(item);
                }
            }
            //Manager.Display(list_trackId_yandex);
            List<Track> list = new List<Track>();
            foreach (var item in list_trackId_yandex)
            {
                Track track = new Track() { TrackId = item };
                TrackContentFromYAndexApp(track, sql_conn);
                list.Add(track);
            }
            return list;
        }

        private static void TrackContentFromYAndexApp(Track item, string sql_conn)
        {
            try
            {
                List<SqliteParameter> lp_title = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", item.TrackId! } });
                DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str3"], ("Title", "GetString"), item, lp_title);
                DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str10"], ("AlbumId", "GetString"), item, lp_title);
                List<SqliteParameter> lp_album = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", item.AlbumId! } });
                DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str11"], new (string, string)[] { ("Album", "GetString"), ("Year", "GetString"), ("Artist", "GetString") }, item, lp_album);
                item.Name = item.Title;
            }
            catch (Exception ex)
            {
                item.Name = item.Title;
                Console.WriteLine(item);
                DisplayColor(ex.Message, ConsoleColor.DarkYellow);
            }
        }


        private static string GetRandomTrackId(string pathDirDestination)
        {
            string _sours_db = Path.Combine(pathDirDestination, DbSqlite.NameMyDB);
            string sql_conn = DbSqlite.Get_str_connection(_sours_db);
            var list_trackId_destination = DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str13"]);
            string trackId = "-1";
            do
            {
                trackId = new Random().Next().ToString();

            } while (list_trackId_destination.Contains(trackId));
            return "-" + trackId;
        }

        public static string NormalizeName(string? name)
        {
            if (name == null) return "";
            if (regex.IsMatch(name))
            {
                string result = regex.Replace(name, Manager.Target);
                return result;
            }
            else
            {

                return name;
            }
        }
        public static Track CreateTrackFromFileInfo(FileInfo file)
        {
            var track = new Track();
            var tfile = TagLib.File.Create(file.FullName);
            string title = tfile.Tag.Title;
            string album = tfile.Tag.Album;
            uint year = tfile.Tag.Year;
            string[] perf = tfile.Tag.Performers;
            if (title != null)
            { track.Title = title; }
            else { track.Title = Path.GetFileNameWithoutExtension(file.FullName); }
            if (album != null) track.Album = album;
            if (year != 0) track.Year = year.ToString();
            if (perf.Length > 0) track.Artist = perf[0];
            track.TrackId = Manager.GetRandomTrackId(Manager.pathDestination);
            track.Extension = file.Extension;
            track.Name = file.Name;
            return track;
        }

        public static bool IsAudio(FileInfo file)
        {
            if (file == null) return false;
            if (file.Extension.ToLower() == ".mp3" || file.Extension.ToLower() == ".flack" || file.Extension.ToLower() == ".wav") { return true; } else return false;
        }

        public static bool Copy(Track track, FileInfo sours, string PathToDir)
        {
            bool isCopy = false;
            var file_destination = new FileInfo(Path.Combine(PathToDir, sours.Name));
            if (!file_destination.Exists)
            {
                try
                {
                    sours.CopyTo(file_destination.FullName);
                    isCopy = true;
                }
                catch (Exception ex)
                {
                    Manager.DisplayColor($"Файл:{sours.Name} - скопировать не удалось. Exception: {ex.Message}", ConsoleColor.Red);
                }
            }
            return isCopy;
        }

        private static string FirstCharToUp(string str)
        {
            char[] chars = str.ToCharArray();
            char first = chars[0];
            chars[0] = char.ToUpper(first);
            return new string(chars);
        }

    }
}
