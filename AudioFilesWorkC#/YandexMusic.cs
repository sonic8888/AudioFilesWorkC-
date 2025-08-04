using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal class YandexMusic
    {
        public static string? PathYandexMusicDir
        {
            get; private set;
        }
        public static string? PathMusicSours { get; private set; }
        public static string? PathDBSqlite { get; private set; }

        public static string PathCopyTo { get; set; } = "";

        public static string Data {  get; private set; }
        static YandexMusic()
        {

            PathYandexMusicDir = GetPathYandexMusic();
            PathMusicSours = GetPathMusicSoursDir(PathYandexMusicDir);
            PathDBSqlite = GetPathDbSqliteYandex(PathYandexMusicDir);
            Data = DateTime.Now.ToString("d");


        }



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


        private static string? GetPathMusicSoursDir(string? path)
        {
            string music = "Music";
            string? path_music_files = null;
            if (path != null)
                foreach (var dir in Directory.GetDirectories(path))
                {
                    if (dir.Contains(music))
                    {
                        foreach (var d in Directory.GetDirectories(dir))
                        {
                            foreach (var file in Directory.GetFiles(d))
                            {
                                if (new FileInfo(file).Extension.ToLower() == ".mp3")
                                    path_music_files = d; break;
                            }
                        }
                    }

                }
            if (path_music_files == null)
            {
                throw new Exception($"{path}\\{music}\\..?  not found.");
            }
            return path_music_files;
        }

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

        public static string GetName(Track track) => $"{track.Name}. ({track.Artist}).mp3";

        public static FileInfo CopyTo(Track track, string? sours, string destination, bool isRename = true)
        {
            if (Path.Exists(destination)!) throw new ArgumentException($"Path:{destination} - There is no such way.");
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
                { file = file.CopyTo(_destination, true); }
                else
                    throw new ArgumentException($"Path:{_destination} - there is no such file");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return file;

        }

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
