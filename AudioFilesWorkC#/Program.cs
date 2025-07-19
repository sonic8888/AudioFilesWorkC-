using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace AudioFilesWorkC_
{
    internal class Program
    {
        static void Main(string[] args)
        {

            try
            {
                YandexMusic yandexMusic = new YandexMusic();
                Console.WriteLine(yandexMusic.PathYndexMusicDir);
                Console.WriteLine(yandexMusic.PathMusicSours);
                Console.WriteLine(yandexMusic.PathDBSqlite);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }



        }


    }
}
