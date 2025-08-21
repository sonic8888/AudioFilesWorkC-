using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
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
            object? res = DbSqlite.ExecuteScalar(sql_conn, DbSqlite.Dictionary_quearis["str1"], com_params);

            if (res != null)
            {
                int result = Convert.ToInt32(res);
                tracks = new Track[result];
                for (int i = 0; i < tracks.Length; i++)
                {
                    tracks[i] = new Track();
                }
                DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_quearis["str2"], ("TrackId", "GetString"), ref tracks, com_params);
                foreach (var item in tracks)
                {
                    try
                    {
                        List<SqliteParameter> lp_title = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", item.TrackId! } });
                        DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_quearis["str3"], ("Title", "GetString"), item, lp_title);
                        DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_quearis["str10"], ("AlbumId", "GetString"), item, lp_title);
                        List<SqliteParameter> lp_album = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", item.AlbumId! } });
                        DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_quearis["str11"], new (string, string)[] { ("Album", "GetString"), ("Year", "GetString"), ("Artist", "GetString") }, item, lp_album);
                        item.Name = item.Title;
                    }
                    catch (Exception ex)
                    {
                        item.Name = item.Title;
                        Console.WriteLine(item);
                        DisplayColor(ex.Message, ConsoleColor.DarkYellow);
                    }


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
                tfile.Tag.Performers = new string[] {track.Artist!} ;
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
        public static void CopyInsertDataToDestination(Track[] tracks)
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
                DbSqlite.Dictionary_quearis["del"], DbSqlite.Get_list_params(new Dictionary<string, string?>() { { "@value", rows.ToString() } }));
                    }

                }

                CreateTags(item,Manager.pathDestination);

                //Console.WriteLine(item);
            }
        }

        //}

        /// <summary>
        /// Записываем данные из Track в БД(NameMyDB).
        /// </summary>
        /// <param name="track">Track</param>
        /// <param name="pathDbDestination">путь к папке назначения</param>
        /// <returns>номер строки в БД или -1</returns>
        public static int InsertData(Track track, string pathDbDestination)
        {
            int rows = -1;
            try
            {
                var dicParam = new Dictionary<string, string?>();
                dicParam.Add("@title", track.Title);
                dicParam.Add("@artist", track.Artist ?? "unknown");
                dicParam.Add("@album", track.Album ?? "unknown");
                dicParam.Add("@year", track.Year ?? "unknown");
                dicParam.Add("@track_id", track.TrackId);
                dicParam.Add("@data", Track.Data());
                var comParams = DbSqlite.Get_list_params(dicParam);
                var r = DbSqlite.ExecuteScalar(DbSqlite.Get_str_connection(pathDbDestination), DbSqlite.Dictionary_quearis["str_insert"], comParams);
                rows = Convert.ToInt32(r);
            }
            catch (Exception ex)
            {
                Console.WriteLine(track);
                Manager.DisplayColor(ex.Message, ConsoleColor.Cyan);
            }

            return rows;
        }

        ///// <summary>
        ///// Извлекает данные из БД и упаковывает их в массив 'Track'.
        ///// </summary>
        ///// <returns>Track[]</returns>
        //public static Track[] GetDataFromPathDestination()
        //{
        //    Track[] tracks = GetEmptyTrackFromDB();
        //    if (tracks != null)
        //    {
        //        string sours_dir = YandexMusic.GetPathDbSqliteDestination();
        //        string str_connection = DbSqlite.Get_str_connection(sours_dir);
        //        (string, string)[] pr_meth = new (string, string)[] { ("Name", "GetString"), ("Artist", "GetString"), ("TrackId", "GetString"), ("ArtistId", "GetString"), ("Name_Artist", "GetString"), ("DataCreate", "GetString") };
        //        DbSqlite.ExecuteReader(str_connection, DbSqlite.Dictionary_quearis["str7"], pr_meth, ref tracks);
        //    }
        //    return tracks!;
        //}

        ///// <summary>
        ///// Выводит на консоль последовательность.
        ///// </summary>
        ///// <param name="t">IEnumerable</param>
        //public static void Display(IEnumerable t)
        //{

        //    foreach (var item in t)
        //    {
        //        Console.WriteLine(item);

        //    }
        //}
        //public static void CheckDirDestination()
        //{
        //    Track[] trackDB = GetDataFromPathDestination();
        //    var files = Directory.GetFiles(Manager.pathDestination!);
        //    var files_info = files.Where(f => f.EndsWith(".mp3") || f.EndsWith(".flac")).Select(f => new FileInfo(f));
        //    FileInfo f = new FileInfo(Manager.pathDestination!);

        //    Track[] trackDir = CreateTrackArray(files_info);
        //    List<Track> trDB = new List<Track>(trackDB);
        //    List<Track> trDir = new List<Track>(trackDir);
        //    var ls = new List<Track>();
        //    foreach (var track in trDir)
        //    {
        //        if (trDB.Contains(track))
        //        { continue; }
        //        else { ls.Add(track); }
        //    }

        //    //var f = trackDir.Except(trackDB);// if f > 0 add in DB or DELETE from DIR
        //    //var d = trackDB.Except(trackDir);//if d > 0 add in DIR or DELETE from DB
        //    //var c = d.Except(f);
        //    //DisplayTracks(f);
        //    DisplayTracks(ls);
        //    //Console.WriteLine("***********************************************************************");
        //    //DisplayTracks(f);
        //    //Console.WriteLine("***********************************************************************");
        //    //DisplayTracks(c);
        //    //var prop = GetProperty(trackD, "NameArtist");
        //    //Console.WriteLine($"DB:{prop.Count}");
        //    //Console.WriteLine($"DIR:{files_info.Count}");
        //    //var not_DB = files_info.Where(f=>!prop.Contains(f.Name)).ToList();
        //    //var f = files_info.Except(copy_files);
        //    //Console.WriteLine(f.Count());

        //}

        //public static void AddFiles()
        //{
        //    var files = Directory.GetFiles(Manager.pathFrom!);
        //    var files_info = files.Where(f => f.EndsWith(".mp3") || f.EndsWith(".flac")).Select(f => new FileInfo(f));
        //    string pathDbDestination = YandexMusic.GetPathDbSqliteDestination();
        //    //DisplayFileInfo(files_info);
        //    var tracks = CreateTrackArray(files_info);
        //    try
        //    {
        //        var display = Manager.DisplayTrack(new Track());
        //        foreach (var item in tracks)
        //        {
        //            bool isException;
        //            YandexMusic.CopyFromTo(item, YandexMusic.PathCopyFrom, YandexMusic.PathCopyTo, out isException, true, false);
        //            if (isException)
        //            {
        //                Manager.InsertData(item, pathDbDestination);
        //                display(item);


        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        DisplayColor(ex.Message, ConsoleColor.Red);
        //    }
        //}

        ///// <summary>
        ///// Копирует недостающие треки из папки Яндекс Музыка в папку назначения и заносит данные в БД.
        ///// </summary>
        ///// <returns></returns>
        ///// <exception cref="Exception"></exception>
        //async public static Task CopyFromYandexMusic()
        //{

        //    static void Copy()
        //    {
        //        string pathDbDestination = YandexMusic.GetPathDbSqliteDestination();
        //        //Track[] tracks = GetDifferenceYandexAndDestination();
        //        try
        //        {
        //            var display = Manager.DisplayTrack(new Track());
        //            foreach (var item in tracks)
        //            {
        //                bool isException;
        //                YandexMusic.CopyTo(item, YandexMusic.PathMusicDirYandex, YandexMusic.PathCopyTo, out isException, true, false);
        //                if (isException)
        //                {
        //                    Manager.InsertData(item, pathDbDestination);
        //                    display(item);


        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            DisplayColor(ex.Message, ConsoleColor.Red);
        //        }
        //    }

        //    await Task.Run(Copy);

        //}

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
        //public static Track[] CreateTrackArray(IEnumerable<FileInfo> files)
        //{
        //    var tracks = new Track[files.Count()];
        //    string pattern = @"\w+?.*?\(\w*.*\)";
        //    int i = 0;
        //    foreach (var file in files)
        //    {
        //        string name = file.Name;
        //        string extension = file.Extension;
        //        name = name.TrimEnd(extension.ToArray());
        //        if (Regex.IsMatch(name, pattern))
        //        {
        //            int index = name.IndexOf('(');
        //            string n = name.Substring(0, index);
        //            string a = name.Substring(index + 1).Trim(')');
        //            tracks[i++] = new Track() { name = n, artist = a, Extension = extension };

        //        }
        //        else
        //        {
        //            tracks[i++] = new Track() { name = name, Extension = extension };
        //        }
        //    }
        //    return tracks;
        //}
    }
}
