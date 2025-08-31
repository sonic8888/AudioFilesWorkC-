using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TagLib.Ape;
using TagLib.Matroska;
using static System.Net.Mime.MediaTypeNames;

namespace AudioFilesWorkC_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ////Massage();
            //string pathDbDestination = DbSqlite.GetPathDbSqliteDestination();
            //Task task = Manager.AddFilesFromYandexMusic();
            //task.Wait();

            ////Test();
            ////TestTabLib();
            ////AddNewFiles(@"D:\other");
            Manager.PrintMessage();
        }




        static void TestTabLib()
        {
            var file = new FileInfo(@"D:\other\Гела Чёрный   Твои глаза.mp3");
            //CreateTrackFromFileInfo(file);




        }


        //static string TestRegex(string word)
        //{

        //    string pattern = @"[\*\|\\\:\""<>\?\/]";
        //    string target = ".";
        //    Regex regex = new Regex(pattern);
        //    if (regex.IsMatch(word))
        //    {
        //        Console.WriteLine(word);
        //    }
        //    string result = regex.Replace(word, target);
        //    return result;
        //}

        static void Test()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>() {
                            {"str1","SELECT Count(TrackId) FROM T_PlaylistTrack WHERE Kind = @value;" },
            {"str2", "SELECT TrackId FROM T_PlaylistTrack WHERE Kind = @value;" },
            {"str10", "SELECT AlbumId FROM T_TrackAlbum WHERE TrackId = @value;" },
            {"str3", "SELECT Title FROM T_Track WHERE Id = @value" },
            {"str4", "SELECT ArtistId FROM T_TrackArtist WHERE TrackId = @value" },
            {"str5", "SELECT Name FROM T_Artist WHERE Id = @value" },
            {"str6", "SELECT Count(Name) From T_Trask_Yandex" },
            {"str7", "SELECT * FROM  T_Trask_Yandex " },
            {"str8", "SELECT Title, Year, ArtistsString FROM T_Album WHERE Id = @value" },
            //{"str9", "SELECT Year FROM T_Album WHERE Id = @value" },
            {"str_create", "CREATE TABLE T_Trask_Yandex (Id  INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE  NOT NULL, Title  VARCHAR, Artist  VARCHAR, Album VARCHAR, Year VARCHAR, TrackId  VARCHAR, ArtistId  VARCHAR, NameArtist   VARCHAR, Data  VARCHAR , Sours VARCHAR DEFAULT ('Yandex'));" },
            {"str_insert","INSERT INTO T_Trask_Yandex (Title, Artist,Album, Year, TrackId, ArtistId, NameArtist, Data)  VALUES (@title, @artist,@album, @year, @track_id, @artist_id, @name_artist, @data)" },
             { "str12", "SELECT  TrackId FROM T_Trask_Yandex WHERE Sours = @value" },
             { "str13", "SELECT TrackId FROM T_Trask_Yandex" }
            };

            string? sours_db = @"D:\test";
            string _sours_db = Path.Combine(sours_db, DbSqlite.NameMyDB);
            string sql_conn = DbSqlite.Get_str_connection(_sours_db);
            //DbSqlite.ExecuteNonQuery(sql_conn, dic["str_create"]);
            //var tracks = Manager.GetDataFromYandexDB();
            //for (int i = 0; i < 10; i++)
            //{
            //    var track = tracks[i];
            //    Manager.InsertData(track, _sours_db);
            //}
            //string path = DbSqlite.GetPathDbSqliteDestination();
            //string str_connection = DbSqlite.Get_str_connection(path);
            //List<SqliteParameter> com_params = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", "Yandex" } });
            var list_trackId_destination = DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str13"]);
            //Manager.Display(list_trackId_destination);
            string trackId = "-1";
            do
            {
                trackId = new Random().Next().ToString();

            } while (list_trackId_destination.Contains(trackId));
            Console.WriteLine(trackId);




        }

        //static void AddNewFiles(string pathDir)
        //{
        //    if (!Directory.Exists(pathDir)) throw new ArgumentException($"папка: {pathDir} - не найдена.");
        //    var files = new DirectoryInfo(pathDir).GetFiles();
        //    YandexMusic.PathCopyTo = Manager.pathDestination;
        //    string pathDbDestination = DbSqlite.GetPathDbSqliteDestination();
        //    foreach (var item in files)
        //    {
        //        if (Manager.IsAudio(item))
        //        {
        //            Track track = Manager.CreateTrackFromFileInfo(item);
        //            int rows = Manager.InsertData(track, pathDbDestination, "Other");

        //            bool isCopy = Manager.Copy(track, item, YandexMusic.PathCopyTo);
        //            if (!isCopy)// если копирование не удалось то удаляем из БД
        //            {
        //                Manager.DisplayColor($"DELETE: rows:{rows}, track:{track}", ConsoleColor.Red);
        //                DbSqlite.ExecuteNonQuery(DbSqlite.Get_str_connection(pathDbDestination),
        //        DbSqlite.Dictionary_query["del"], DbSqlite.Get_list_params(new Dictionary<string, string?>() { { "@value", rows.ToString() } }));
        //            }
        //            else
        //            {
        //                Manager.DisplayColor(item.Name);
        //            }
        //        }
        //    }
        //}

        static void Massage()
        {
            Console.WriteLine("Укажите путь к папке:");
            string? path = Console.ReadLine();
            string path_path = Path.GetFullPath(path!);
            Manager.pathDestination = path_path;
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


