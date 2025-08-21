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
            //Track[] track = Manager.GetDataFromYandexDB();

            //Manager.CopyInsertDataToDestination(track);
            //TestTabLib();

            string path = DbSqlite.GetPathDbSqliteDestination();
            string str_connection = DbSqlite.Get_str_connection(path);
            var list_trackId_destination = DbSqlite.ExecuteReader(str_connection, DbSqlite.Dictionary_query["str12"]);
            Manager.Display(list_trackId_destination);

            string? sours_db = YandexMusic.PathDBSqlite;
            string sql_conn = DbSqlite.Get_str_connection(sours_db);
            List<SqliteParameter> com_params = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", "5" } });
            var list_trackId_yandex = DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_query["str2"], com_params);
            Manager.Display(list_trackId_destination);

        }




        //static void TestTabLib()
        //{
        //    var tfile = TagLib.File.Create(@"D:\test\All That She Wants.mp3");
        //    string title = tfile.Tag.Title;
        //    string album = tfile.Tag.Album;
        //    var gg = tfile.Tag.Year;
        //    Console.WriteLine(gg);
        //    //tfile.Tag.TrackCount = 1721;
        //    //tfile.Save();
        //    Console.WriteLine(tfile.Tag.TrackCount);
        //    Console.WriteLine(tfile.Tag.Year);
        //    TimeSpan duration = tfile.Properties.Duration;
        //    Console.WriteLine("Title: {0}, duration: {1}", title, duration);
        //}

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

        //static void Test()
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>() {
        //                    {"str1","SELECT Count(TrackId) FROM T_PlaylistTrack WHERE Kind = @value;" },
        //    {"str2", "SELECT TrackId FROM T_PlaylistTrack WHERE Kind = @value;" },
        //    {"str10", "SELECT AlbumId FROM T_TrackAlbum WHERE TrackId = @value;" },
        //    {"str3", "SELECT Title FROM T_Track WHERE Id = @value" },
        //    {"str4", "SELECT ArtistId FROM T_TrackArtist WHERE TrackId = @value" },
        //    {"str5", "SELECT Name FROM T_Artist WHERE Id = @value" },
        //    {"str6", "SELECT Count(Name) From T_Trask_Yandex" },
        //    {"str7", "SELECT * FROM  T_Trask_Yandex " },
        //    {"str8", "SELECT Title, Year, ArtistsString FROM T_Album WHERE Id = @value" },
        //    //{"str9", "SELECT Year FROM T_Album WHERE Id = @value" },
        //    {"str_create", "CREATE TABLE T_Trask_Yandex (Id  INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE  NOT NULL, Title  VARCHAR, Artist  VARCHAR, Album VARCHAR, Year VARCHAR, TrackId  VARCHAR, ArtistId  VARCHAR, NameArtist   VARCHAR, Data  VARCHAR );" },
        //    {"str_insert","INSERT INTO T_Trask_Yandex (Title, Artist,Album, Year, TrackId, ArtistId, NameArtist, Data)  VALUES (@title, @artist,@album, @year, @track_id, @artist_id, @name_artist, @data)" }
        //    };

        //    string? sours_db = YandexMusic.PathDBSqlite;
        //    string sql_conn = DbSqlite.Get_str_connection(sours_db);
        //    Track track = new Track() { AlbumId = "23691030" };
        //    List<SqliteParameter> lp_title = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", track.AlbumId! } });
        //    DbSqlite.ExecuteReader(sql_conn, "SELECT Title, Year, ArtistsString FROM T_Album WHERE Id = @value", new (string, string)[] { ("Album", "GetString"), ("Year", "GetString"), ("Artist", "GetString") }, track, lp_title);
        //    Console.WriteLine(track);

        //}

        //    public static Track[] GetDataFromYandexDB()
        //    {
        //        Track[] tracks = new Track[0];
        //        string? sours_db = YandexMusic.PathDBSqlite;
        //        string sql_conn = DbSqlite.Get_str_connection(sours_db);
        //        List<SqliteParameter> com_params = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", "5" } });
        //        object? res = DbSqlite.ExecuteScalar(sql_conn, DbSqlite.Dictionary_quearis["str1"], com_params);

        //        if (res != null)
        //        {
        //            int result = Convert.ToInt32(res);
        //            tracks = new Track[result];
        //            for (int i = 0; i < tracks.Length; i++)
        //            {
        //                tracks[i] = new Track();
        //            }
        //            DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_quearis["str2"], ("TrackId", "GetString"), ref tracks, com_params);
        //            foreach (var item in tracks)
        //            {
        //                try
        //                {
        //                    List<SqliteParameter> lp_title = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", item.TrackId! } });
        //                    DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_quearis["str3"], ("Title", "GetString"), item, lp_title);
        //                    DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_quearis["str10"], ("AlbumId", "GetString"), item, lp_title);
        //                    List<SqliteParameter> lp_album = DbSqlite.Get_list_params(new Dictionary<string, string?> { { "value", item.AlbumId! } });
        //                    DbSqlite.ExecuteReader(sql_conn, DbSqlite.Dictionary_quearis["str11"], new (string, string)[] { ("Album", "GetString"), ("Year", "GetString"), ("Artist", "GetString") }, item, lp_album);
        //                }
        //                catch (Exception)
        //                {

        //                }
        //            }
        //        }
        //        return tracks;
        //    }




    }
}


