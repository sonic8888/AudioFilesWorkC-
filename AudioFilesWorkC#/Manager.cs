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
        public static string Kind = "5";
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
            bool isWork = true;
            while (isWork)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("y - копирование файлов из Яндекс Музыка в указанную папку.");
                Console.WriteLine("d - добавление файлов из выбранной папки в  указанную папку");
                Console.WriteLine("e - для завершения работы");
                action = Console.ReadLine();
                switch (action)
                {
                    case "y":
                        //Console.WriteLine("вызов метода у");
                        AddFilesYandexMusic();
                        break;
                    case "d":
                        string pathOther = "";
                        while (true)
                        {
                            Console.WriteLine("Укажите путь к папке c файлами для добавления:");
                            var path = Console.ReadLine();
                            if (path != null) pathOther = path;
                            if (Path.Exists(pathOther))
                            {
                                break;
                            }
                            else
                                Console.WriteLine("Такого пути не существует.");
                        }
                        AddFilesOther(pathOther);
                        ;
                        break;

                    case "e":
                        isWork = false;
                        break;
                    default:
                        Console.WriteLine("действие выбрано не корректно:");
                        break;
                }


            }
        }




        /// <summary>
        /// Создаем теги аудиофайла скопированного  в папку(pathDestination) из данных извлеченных из объекта класса "Track". 
        /// </summary>
        /// <param name="track">Объект класса "Track".</param>
        /// <param name="path_dir">Путь к папке с аудиофайлами(pathDestination )</param>
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
                    }
                    else
                    {
                        CreateTags(item, Manager.pathDestination);
                        Manager.DisplayColor(item.Name);
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



        /// <summary>
        /// Выводит на консоль текст разными цветами.
        /// </summary>
        /// <param name="message">Текст</param>
        /// <param name="color">ConsoleColor color</param>
        public static void DisplayColor(string message, System.ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Выводит на консоль элементы последовательности.
        /// </summary>
        /// <typeparam name="T">Тип элемента</typeparam>
        /// <param name="enumerable">Последовательность</param>
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




        /// <summary>
        /// Находим разницу треков между БД((pathDestination) и БД("ЯНДЕКС МУЗЫКА").
        /// </summary>
        /// <returns>Список содержащий разницу треков</returns>
        public static List<Track> GetDifferentTracks()
        {
            // извлекаем данные из БД находящийся в папке(pathDestination).
            var list_trackId_destination = GetDataFromDestinationDB();

            // извлекаем данные из БД находящийся в корневой папке "ЯНДЕКС МУЗЫКА".
            string? sours_db = YandexMusic.PathDBSqlite;
            string sql_conn = DbSqlite.Get_str_connection(sours_db);
            var list_trackId_yandex = GetDataFromYandex(sql_conn);

            //Находим разницу данных между БД(pathDestination) и БД("ЯНДЕКС МУЗЫКА").
            foreach (var item in list_trackId_destination)
            {
                if (list_trackId_yandex.Contains(item))
                {
                    list_trackId_yandex.Remove(item);
                }
            }
            // Разницу в данных помещаем в список.
            List<Track> list = new List<Track>();
            foreach (var item in list_trackId_yandex)
            {
                Track track = new Track() { TrackId = item };
                TrackContentFromYAndexApp(track, sql_conn);
                list.Add(track);
            }
            return list;
        }

        /// <summary>
        /// Извлекаем данные из БД находящийся в папке(pathDestination).
        /// </summary>
        /// <returns>Список TrackId</returns>
        private static List<string> GetDataFromDestinationDB()
        {
            // извлекаем данные из БД находящийся в папке(pathDestination).
            string path = DbSqlite.GetPathDbSqliteDestination();
            string str_connection = DbSqlite.Get_str_connection(path);
            List<SqliteParameter> com_params_destination = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", "Yandex" } });
            var list_trackId_destination = DbSqlite.ExecuteReader(str_connection, DbSqlite.Dictionary_query["str12"], com_params_destination);
            return list_trackId_destination;
        }

        /// <summary>
        /// Извлекаем данные из БД находящийся в корневой папке "ЯНДЕКС МУЗЫКА".
        /// </summary>
        /// <param name="sql_conn">Строка подключения к БД(ЯндексМузыка)</param>
        /// <returns>Список TrackId</returns>
        private static List<string> GetDataFromYandex(string sql_conn)
        {
            // извлекаем данные из БД находящийся в корневой папке "ЯНДЕКС МУЗЫКА".
            List<SqliteParameter> com_params_yandex = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", Manager.Kind } });
            var list_trackId_yandex = DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str2"], com_params_yandex);
            return list_trackId_yandex;
        }

        /// <summary>
        /// Заполняет пустой объект класса "Track" данными считаными из 
        /// </summary>
        /// <param name="item">Объект класса "Track"</param>
        /// <param name="sql_conn">Строка подключения к БД</param>
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
                DisplayColor(item.ToString());
                DisplayColor(ex.Message, ConsoleColor.DarkYellow);
            }
        }


        /// <summary>
        /// Создает "случайный" trackId и проверяет уникальность в БД(pathDestination).
        /// </summary>
        /// <param name="pathDirDestination"></param>
        /// <returns></returns>
        private static string GetRandomTrackId(string pathDirDestination)
        {
            string _sours_db = Path.Combine(pathDirDestination, DbSqlite.NameMyDB);
            string sql_conn = DbSqlite.Get_str_connection(_sours_db);
            var list_trackId_destination = DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str13"]);
            int trackId = -1;
            do
            {
                trackId = new Random().Next() * -1;

            } while (list_trackId_destination.Contains(trackId.ToString()));
            return trackId.ToString();
        }

        /// <summary>
        /// Заменяет недопустимые символы в имени файла(Manager.Target = "."). 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Создает объект класса "Track" из аудиофайла.
        /// Извлекает теги аудиофайла и записывает их значение в свойства объекта класса "Track".
        /// </summary>
        /// <param name="file">Объект класса "FileInfo"(аудиофайл)</param>
        /// <returns>Track</returns>
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

        /// <summary>
        /// Возвращает "true" если расширение файла - ".mp3",".flack" или ".wav".
        /// </summary>
        /// <param name="file">аудиофайл</param>
        /// <returns>bool</returns>
        public static bool IsAudio(FileInfo file)
        {
            if (file == null) return false;
            if (file.Extension.ToLower() == ".mp3" || file.Extension.ToLower() == ".flack" || file.Extension.ToLower() == ".wav") { return true; } else return false;
        }

        /// <summary>
        /// Копирует файл.
        /// </summary>
        /// <param name="sours">аудиофайл</param>
        /// <param name="PathToDir">папка назначения</param>
        /// <returns>True если копирование прошло удачно.</returns>
        public static bool Copy(FileInfo sours, string PathToDir)
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



        /// <summary>
        /// Добавляет аудиофайлу в папку (pathDestination):
        /// Создает объект класса "Track", записывает его в БД(pathDestination),
        /// Копирует аудиофайл в папку (pathDestination).
        /// </summary>
        /// <param name="pathDir"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void AddNewFiles(string pathDir)
        {
            if (!Directory.Exists(pathDir)) throw new ArgumentException($"папка: {pathDir} - не найдена.");
            var files = new DirectoryInfo(pathDir).GetFiles();
            YandexMusic.PathCopyTo = Manager.pathDestination;
            string pathDbDestination = DbSqlite.GetPathDbSqliteDestination();
            foreach (var item in files)
            {
                if (Manager.IsAudio(item))
                {
                    Track track = Manager.CreateTrackFromFileInfo(item);

                    FileInfo file_destination = new FileInfo(Path.Combine(Manager.pathDestination, item.Name));
                    int rows = Manager.InsertData(track, pathDbDestination, "Other");

                    bool isCopy = Manager.Copy(item, YandexMusic.PathCopyTo);
                    if (!isCopy)// если копирование не удалось то удаляем из БД
                    {
                        Manager.DisplayColor($"DELETE: rows:{rows}, track:{track}", ConsoleColor.Red);
                        DbSqlite.ExecuteNonQuery(DbSqlite.Get_str_connection(pathDbDestination),
                DbSqlite.Dictionary_query["del"], DbSqlite.Get_list_params(new Dictionary<string, string?>() { { "@value", rows.ToString() } }));
                    }
                    else
                    {
                        Manager.DisplayColor(item.Name);
                    }
                }
            }
        }


        async public static Task AddFilesFromYandexMusic()
        {
            Task<List<Track>> task_tracks = new Task<List<Track>>(() => Manager.GetDifferentTracks());
            task_tracks.Start();
            List<Track> list = task_tracks.Result;
            await task_tracks;
            Task task_copy = new Task(() => Manager.CopyInsertDataFromYandexAppToDestination(list.ToArray()));
            task_copy.Start();
            await task_copy;

        }

        async public static Task AddNewFilesToDestination(string pathDir)
        {
            Task task_add = Task.Run(() => AddNewFiles(pathDir));
            await task_add;
        }

        static void AddFilesYandexMusic()
        {
            Task task = Manager.AddFilesFromYandexMusic();
            task.Wait();

        }
        static void AddFilesOther(string pathDir)
        {
            Task task = Manager.AddNewFilesToDestination(pathDir);
            task.Wait();
        }

    }
}
