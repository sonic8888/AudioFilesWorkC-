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
        public string? PathYandexMusicDir
        {
            get; private set;
        }
        public string? PathMusicSours { get; private set; }
        public string? PathDBSqlite { get; private set; }

        public string PathCopyTo { get; set; } = "";
        public YandexMusic()
        {

            PathYandexMusicDir = _get_path_yandex_music();
            PathMusicSours = _get_path_music_sours_dir(PathYandexMusicDir);
            PathDBSqlite = _get_path_db_sqlite(PathYandexMusicDir);


        }



        private string? _get_path_yandex_music()
        {
            string user_dict = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

            string path1 = Path.Combine(user_dict, @"AppData\Local\Packages");
            string yandex = "Yandex.Music_";
            string path2 = "LocalState";
            string? _path_yndex_music = null;
            foreach (var dir in Directory.GetDirectories(path1))
            {
                if (dir.Contains(yandex))
                {
                    _path_yndex_music = Path.Combine(dir, path2);
                    break;
                }

            }
            if (_path_yndex_music == null)
                throw new Exception($"{path1}\\...{yandex}... not found.");
            else
                return _path_yndex_music;
        }


        private string? _get_path_music_sours_dir(string? path)
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

        private string? _get_path_db_sqlite(string? path)
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

        public string GetName(Track track) => $"{track.Name}. ({track.Artist}).mp3";

        public FileInfo CopyTo(Track track, string? sours, string destination, bool isRename = true)
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



    }
}
