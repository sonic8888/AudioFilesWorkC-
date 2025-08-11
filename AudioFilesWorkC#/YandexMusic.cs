using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal class YandexMusic
    {
        /// <summary>
        /// PathYandexDir указывает путь к папке корневой Яндекс Музыка.
        /// </summary>
        public static string? PathYandexDir
        {
            get; private set;
        }
        /// <summary>
        /// PathMusicSours указывает путь к папке с треками приложения Яндекс Музыка.
        /// </summary>
        public static string? PathMusicSours { get; private set; }
        /// <summary>
        /// PathDBSqlite путь к БД Яндекс Музыка.
        /// </summary>
        public static string? PathDBSqlite { get; private set; }

        /// <summary>
        /// PathCopyTo путь к папке куда копируются треки.
        /// </summary>
        public static string PathCopyTo { get; set; } = "";

        /// <summary>
        /// Data сегодняшняя дата.
        /// </summary>
        public static string Data { get; private set; }

        public static string PathMusicDirYandex {  get; private set; }
        static YandexMusic()
        {

            PathYandexDir = GetPathYandexMusic();
            PathMusicSours = GetPathMusicSoursDir(PathYandexDir);
            PathDBSqlite = GetPathDbSqliteYandex(PathYandexDir);
            Data = DateTime.Now.ToString("d");
            PathMusicDirYandex = GetPathMusicSoursDir(PathYandexDir);


        }



        /// <summary>
        /// Возвращает путь к корневой папке Яндекс Музыка.
        /// </summary>
        /// <returns>путь к папке</returns>
        /// <exception cref="Exception"></exception>
        private static string? GetPathYandexMusic()
        {
            string user_dict = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

            string path1 = Path.Combine(user_dict, @"AppData\Local\Packages");
            string yandex = "Yandex.Music_";
            string path2 = "LocalState";
            string? _path_yandex_music = null;
            foreach (var dir in Directory.GetDirectories(path1))
            {
                if (dir.Contains(yandex))
                {
                    _path_yandex_music = Path.Combine(dir, path2);
                    break;
                }

            }
            if (_path_yandex_music == null)
                throw new Exception($"{path1}\\...{yandex}... not found.");
            else
                return _path_yandex_music;
        }


        /// <summary>
        /// Находит путь к папке с треками приложения Яндекс Музыка.
        /// </summary>
        /// <param name="path">Путь к корневой папке Яндекс Музыка</param>
        /// <returns>Путь к папке с треками</returns>
        /// <exception cref="Exception">Если путь не найден.</exception>
        private static string GetPathMusicSoursDir(string? path)
        {
            string music = "Music";
            string path_music_files ="";
            if (path != null)
            {
                path_music_files = Path.Combine(path, music);
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    if (file.EndsWith(".sqlite"))
                    { 
                        FileInfo fileDB = new FileInfo(file);
                        string name = fileDB.Name;
                        name = name[..^7];
                        name = name.Substring(8);
                        path_music_files = Path.Combine(path_music_files, name);
                    }
                }

            }
            return path_music_files;
        }

        /// <summary>
        /// Находит путь к БД Яндекс Музыка.
        /// </summary>
        /// <param name="path">Путь к корневой папке Яндекс Музыка</param>
        /// <returns> Путь к БД</returns>
        /// <exception cref="Exception">Если БД не нашли.</exception>
        private static string? GetPathDbSqliteYandex(string? path)
        {
            string? path_db = null;
            if (path != null)
                foreach (var file in Directory.GetFiles(path))
                {
                    if (new FileInfo(file).Extension.ToLower() == ".sqlite")
                    {
                        path_db = file;
                        break;
                    }
                }
            if (path_db == null)
            {
                throw new Exception($"{path}\\..DB.sqlite  not found.");
            }
            else
            {
                return path_db;
            }

        }

        /// <summary>
        /// Комбинирует имя трека и имя артиста.  
        /// </summary>
        /// <param name="track">класс Track</param>
        /// <returns>имя трека + (артист)</returns>
        public static string GetName(Track track) => $"{track.Name}. ({track.Artist}).mp3";

        ///// <summary>
        ///// Копирует трек из папки источника в папку назначения.
        ///// </summary>
        ///// <param name="track">класс Track</param>
        ///// <param name="sours">папка источник</param>
        ///// <param name="destination">папка назначение</param>
        ///// <param name="isRename">изменяем имя трека или нет</param>
        ///// <returns>FileInfo скопированного трека</returns>
        ///// <exception cref="ArgumentException">если не нашел папку назначения</exception>
        ///// <exception cref="Exception">ошибка при копировании</exception>
        //public static FileInfo CopyTo(Track track, string? sours, string destination, bool isRename = true)
        //{
        //    if (Path.Exists(destination)!) throw new ArgumentException($"Path:{destination} - There is no such way.");
        //    string _sours = Path.Combine(sours!, track.TrackId + ".mp3");
        //    string _destination = "";
        //    if (isRename)
        //        _destination = Path.Combine(destination, GetName(track));
        //    else
        //        _destination = Path.Combine(destination, track.TrackId + ".mp3");
        //    FileInfo file = new FileInfo(_sours);

        //    try
        //    {
        //        if (file.Exists)
        //        { file = file.CopyTo(_destination, true); }
        //        else
        //            throw new ArgumentException($"Path:{_destination} - there is no such file");
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception(ex.Message);
        //    }
        //    return file;

        //}

        /// <summary>
        /// Копирует трек из папки источника в папку назначения.
        /// </summary>
        /// <param name="track">AudioFilesWorkC_.Track</param>
        /// <param name="sours">папка источник</param>
        /// <param name="destination">папка назначение</param>
        /// <param name="isException">false если было исключение</param>
        /// <param name="isRename">изменяем имя трека или нет</param>
        /// <param name="isOverwrite">true если файл надо перезаписывать</param>
        /// <returns>System.IO.FileInfo скопированного трека</returns>
        /// <exception cref="ArgumentException">если путь к папке назначения не существует</exception>
        public static FileInfo CopyTo(Track track, string? sours, string destination, out bool isException, bool isRename = true, bool isOverwrite = true)
        {
            isException = true;
            if (!Path.Exists(destination)) throw new ArgumentException($"Path:{destination} - There is no such way.");
            string _sours = Path.Combine(sours!, track.TrackId + ".mp3");
            string _destination = "";
            if (isRename)
                _destination = Path.Combine(destination, GetName(track));
            else
                _destination = Path.Combine(destination, track.TrackId + ".mp3");
            FileInfo file = new FileInfo(_sours);

            try
            {
                if (file.Exists)
                { file = file.CopyTo(_destination, isOverwrite); }
                else
                    throw new ArgumentException($"Path:{_destination} - there is no such file");
            }
            catch (IOException)
            {
                Console.WriteLine($"Трек '{file.Name}' уже существует. Не перезаписываем его.");
                isException = false;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message); ;
                isException = false;
            }
            return file;

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
                    DbSqlite.ExecuteNonQuery(_str_connection, DbSqlite.queries["str_create"]);
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
