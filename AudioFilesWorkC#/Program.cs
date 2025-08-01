using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace AudioFilesWorkC_
{
    internal class Program
    {

        static string str1 = "SELECT Count(TrackId) FROM T_PlaylistTrack WHERE Kind = @value;";
        static string str2 = "SELECT TrackId FROM T_PlaylistTrack WHERE Kind = @value;";
        static string str3 = "SELECT Title FROM T_Track WHERE Id = @value";
        static string str4 = "SELECT ArtistId FROM T_TrackArtist WHERE TrackId = @value";
        static string str5 = "SELECT Name FROM T_Artist WHERE Id = @value";
        static void Main(string[] args)
        {

            try
            {
                YandexMusic yandexMusic = new ();
                string? sours_db = yandexMusic.PathDBSqlite;
                string sql_conn = DbSqlite.Get_str_connection(sours_db);
                List<SqliteParameter> com_params = DbSqlite.Get_list_params(new Dictionary<string, string> { { "value", "5" } });
                int res = DbSqlite.ExecuteScalar(sql_conn, str1, com_params);
                Track[] tracks = new Track[res];
                for (int i = 0; i < tracks.Length; i++)
                {
                    tracks[i] = new Track();
                }
                DbSqlite.ExecuteReader(sql_conn, str2, ("TrackId", "GetString"), ref tracks, com_params);
                foreach (var item in tracks)
                {

                    List<SqliteParameter> lp_title = DbSqlite.Get_list_params(new Dictionary<string, string> { { "value", item.TrackId! } });
                    DbSqlite.ExecuteReader(sql_conn, str3, ("Name", "GetString"), item, lp_title);
                    DbSqlite.ExecuteReader(sql_conn, str4, ("ArtistId", "GetString"), item, lp_title);
                    List<SqliteParameter> lp_artist = DbSqlite.Get_list_params(new Dictionary<string, string> { { "value", item.ArtistId! } });
                    DbSqlite.ExecuteReader(sql_conn, str5, ("Artist", "GetString"), item, lp_artist);
                    Console.WriteLine($"TrackId:{item.TrackId}, Name:{item.Name}, ArtistId:{item.ArtistId}, Artist:{item.Artist}");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }



        }


    }
}
