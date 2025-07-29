using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace AudioFilesWorkC_
{
    internal class Program
    {

        static string str = "SELECT Count(TrackId) FROM T_PlaylistTrack WHERE Kind = @value;";
        static void Main(string[] args)
        {

            try
            {
                YandexMusic yandexMusic = new YandexMusic();
                Console.WriteLine(yandexMusic.PathYndexMusicDir);
                Console.WriteLine(yandexMusic.PathMusicSours);
                //Console.WriteLine(yandexMusic.PathDBSqlite);
                string? sours_db = yandexMusic.PathDBSqlite;
                string sql_conn = DbSqlite.Get_str_connection(sours_db);
                Console.WriteLine(sql_conn);
                List<SqliteParameter> com_params = new List<SqliteParameter>();
                //com_params.Add(DbSqlite.Get_sql_parametr("$field", "TrackId"));
                //com_params.Add(DbSqlite.Get_sql_parametr("$table", "T_PlaylistTrack"));
                com_params.Add(DbSqlite.Get_sql_parametr("value", "5"));
                int res = DbSqlite.ExecuteScalar(sql_conn, str, com_params);
                Console.WriteLine(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }



        }


    }
}
