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
        public static string? pathFrom = @"D:\music\test";
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
            List<Track> tracksYY = new List<Track>(tracksY);
            List<Track> tracksDD = new List<Track>(trackD);
            tracksYY.Sort();
            tracksDD.Sort();
            foreach (var item in tracksDD)
            {
                if (tracksYY.Contains(item))
                    tracksYY.Remove(item);
            }
            tracksYY.Sort();
            return tracksYY.ToArray();
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
            Track[] trackDB = GetDataFromPathDestination();
            var files = Directory.GetFiles(Manager.pathDestination!);
            var files_info = files.Where(f => f.EndsWith(".mp3") || f.EndsWith(".flac")).Select(f => new FileInfo(f));
            FileInfo f = new FileInfo(Manager.pathDestination!);
           
            Track[] trackDir = CreateTrackArray(files_info);
            List<Track> trDB = new List<Track>(trackDB);
            List<Track> trDir = new List<Track>(trackDir);
            var ls = new List<Track>();
            foreach (var track in trDir)
            {
                if (trDB.Contains(track))
                { continue; }
                else { ls.Add(track); }
            }

            //var f = trackDir.Except(trackDB);// if f > 0 add in DB or DELETE from DIR
            //var d = trackDB.Except(trackDir);//if d > 0 add in DIR or DELETE from DB
            //var c = d.Except(f);
            //DisplayTracks(f);
            DisplayTracks(ls);
            //Console.WriteLine("***********************************************************************");
            //DisplayTracks(f);
            //Console.WriteLine("***********************************************************************");
            //DisplayTracks(c);
            //var prop = GetProperty(trackD, "NameArtist");
            //Console.WriteLine($"DB:{prop.Count}");
            //Console.WriteLine($"DIR:{files_info.Count}");
            //var not_DB = files_info.Where(f=>!prop.Contains(f.Name)).ToList();
            //var f = files_info.Except(copy_files);
            //Console.WriteLine(f.Count());

        }

        public static void AddFiles()
        {
            var files = Directory.GetFiles(Manager.pathFrom!);
            var files_info = files.Where(f => f.EndsWith(".mp3") || f.EndsWith(".flac")).Select(f => new FileInfo(f));
            string pathDbDestination = YandexMusic.GetPathDbSqliteDestination();
            //DisplayFileInfo(files_info);
            var tracks = CreateTrackArray(files_info);
            try
            {
                var display = Manager.DisplayTrack(new Track());
                foreach (var item in tracks)
                {
                    bool isException;
                    YandexMusic.CopyFromTo(item, YandexMusic.PathCopyFrom, YandexMusic.PathCopyTo, out isException, true, false);
                    if (isException)
                    {
                        Manager.InsertData(item, pathDbDestination);
                        display(item);


                    }
                }
            }
            catch (Exception ex)
            {

                DisplayColor(ex.Message, ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Копирует недостающие треки из папки Яндекс Музыка в папку назначения и заносит данные в БД.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        async public static Task CopyFromYandexMusic()
        {

            static void Copy()
            {
                string pathDbDestination = YandexMusic.GetPathDbSqliteDestination();
                Track[] tracks = GetDifferenceYandexAndDestination();
                try
                {
                    var display = Manager.DisplayTrack(new Track());
                    foreach (var item in tracks)
                    {
                        bool isException;
                        YandexMusic.CopyTo(item, YandexMusic.PathMusicDirYandex, YandexMusic.PathCopyTo, out isException, true, false);
                        if (isException)
                        {
                            Manager.InsertData(item, pathDbDestination);
                            display(item);


                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayColor(ex.Message, ConsoleColor.Red);
                }
            }

            await Task.Run(Copy);

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

        public static void DisplayColor(string message, System.ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void DisplayTrack(Track track, int n) => Console.WriteLine(n + ". " + track);

        public static Action<Track> DisplayTrack(Track track)
        {
            int n = 0;
            void display(Track track) => Console.WriteLine(++n + ". " + track);
            return display;
        }
        public static void DisplayTracks(IEnumerable<Track> track)
        {
            int n = 0;
            foreach (var item in track)
            {
                Console.WriteLine($"{++n} Name:{item.Name}, Artist:{item.Artist}, Extension:{item.Extension}, TrackId:{item.TrackId}, ArtistId:{item.ArtistId}");
            }
        }
        public static void DisplayFileInfo(IEnumerable<FileInfo> fileInfo)
        {
            foreach (var item in fileInfo) { Console.WriteLine(item.FullName); }
        }

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
        public static Track[] CreateTrackArray(IEnumerable<FileInfo> files)
        {
            var tracks = new Track[files.Count()];
            string pattern = @"\w+?.*?\(\w*.*\)";
            int i = 0;
            foreach (var file in files)
            {
                string name = file.Name;
                string extension = file.Extension;
                name = name.TrimEnd(extension.ToArray());
                if (Regex.IsMatch(name, pattern))
                {
                    int index = name.IndexOf('(');
                    string n = name.Substring(0, index);
                    string a = name.Substring(index + 1).Trim(')');
                    tracks[i++] = new Track() { name = n, artist = a, Extension = extension };

                }
                else
                {
                    tracks[i++] = new Track() { name = name, Extension = extension };
                }
            }
            return tracks;
        }
    }
}
