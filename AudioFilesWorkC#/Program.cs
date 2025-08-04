using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AudioFilesWorkC_
{
    internal class Program
    {
        static string PathDestination = @"D:\test";
        static void Main(string[] args)
        {
            YandexMusic.PathCopyTo = PathDestination;
            //Track[]? tracks = GetDataFromYandexDB();
            //if (tracks != null)
            //{
            //    Track[] short_track = tracks[0..20];
            //    PrintTrack(short_track);

            //}
            Track[]? tracks = GetEmtyTrackFromDB();
            if (tracks != null)
            {
                string sours_dir = YandexMusic.GetPathDbSqliteDestination();
                string str_connection = DbSqlite.Get_str_connection(sours_dir);
                (string, string)[] pr_meth = new (string, string)[] { ("Name", "GetString"), ("Artist", "GetString"), ("TrackId", "GetString"), ("ArtistId", "GetString"), ("Name_Artist", "GetString"), ("DataCreate", "GetString") };
                DbSqlite.ExecuteReader(str_connection, DbSqlite.queries["str7"], pr_meth, ref tracks);
                PrintTrack(tracks);

                //}
            }
        }
        static Track[]? GetDataFromYandexDB()
        {
            Track[]? tracks = null;
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


        static Track[]? GetEmtyTrackFromDB()
        {
            Track[]? tracks = null;
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

        static void InsertData(Track[] tracks)
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

        static void PrintTrack(Track[] t)
        {

            foreach (Track item in t)
            {
                Console.WriteLine(item);

            }
        }


    }
}


