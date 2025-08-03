using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace AudioFilesWorkC_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Track[]? tracks = GetDataFromYandexDB();
            if (tracks != null)
            {
                foreach (Track item in tracks)
                {
                    Console.WriteLine($"TrackId:{item.TrackId}, Name:{item.Name}, ArtistId:{item.ArtistId}, Artist:{item.Artist}");
                }



            }


            static Track[]? GetDataFromYandexDB()
            {
                Track[]? tracks = null;
                try
                {
                    YandexMusic yandexMusic = new();
                    string? sours_db = YandexMusic.PathDBSqlite;
                    string sql_conn = DbSqlite.Get_str_connection(sours_db);
                    List<SqliteParameter> com_params = DbSqlite.Get_list_params(new Dictionary<string, string> { { "value", "5" } });
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

                            List<SqliteParameter> lp_title = DbSqlite.Get_list_params(new Dictionary<string, string> { { "value", item.TrackId! } });
                            DbSqlite.ExecuteReader(sql_conn, DbSqlite.queries["str3"], ("Name", "GetString"), item, lp_title);
                            DbSqlite.ExecuteReader(sql_conn, DbSqlite.queries["str4"], ("ArtistId", "GetString"), item, lp_title);
                            List<SqliteParameter> lp_artist = DbSqlite.Get_list_params(new Dictionary<string, string> { { "value", item.ArtistId! } });
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


        }
    }
}
