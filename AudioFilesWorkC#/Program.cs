using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AudioFilesWorkC_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Уеажите путь к папке:");
            //string? path = Console.ReadLine();
            //YandexMusic.PathCopyTo = Manager.pathDestination!;
            //YandexMusic.PathCopyFrom = Manager.pathFrom!;
            //YandexMusic.PathCopyTo = Path.GetFullPath(path!);
            //Manager.AddFiles();



            //Task task = Manager.CopyFromYandexMusic();
            //task.Wait();

            //Manager.CheckDirDestination();
            //Test();
            //FileInfo file = new FileInfo(@"D:\test\1721.mp3");
            //file.CopyTo(@"D:\test\dir\myfile.mp3");
            Track[] track = Manager.GetDataFromYandexDB();
            Console.WriteLine(track);


        }


        static void GetDataFromDestination()
        {
            Track[]? tracks = Manager.GetDataFromPathDestination();
            if (tracks != null)
            {
                Manager.Display(tracks);
            }
        }

        static void Test()
        {
            var tfile = TagLib.File.Create(@"D:\test\dir\myfile.mp3");
            string title = tfile.Tag.Title;
            string album = tfile.Tag.Album;
            var gg = tfile.Tag.Year;
            Console.WriteLine(gg);
            //tfile.Tag.TrackCount = 1721;
            //tfile.Save();
            Console.WriteLine(tfile.Tag.TrackCount);
            Console.WriteLine(tfile.Tag.Year);
            TimeSpan duration = tfile.Properties.Duration;
            Console.WriteLine("Title: {0}, duration: {1}", title, duration);
        }

        static string TestRegex(string word)
        {
            
            string pattern = @"[\*\|\\\:\""<>\?\/]";
            string target = ".";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(word)) {
                Console.WriteLine(word);
            }
            string result = regex.Replace(word, target);
            return result;
        }




    }
}


