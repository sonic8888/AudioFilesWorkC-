using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AudioFilesWorkC_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Уеажите путь к папке:");
            string? path = Console.ReadLine();
            //YandexMusic.PathCopyTo = Manager.pathDestination!;
            YandexMusic.PathCopyTo = Path.GetFullPath(path!);




            Task task = Manager.CopyFromYandexMusic();
            task.Wait();

            //Manager.CheckDirDestination();


        }


        static void GetDataFromDestination()
        {
            Track[]? tracks = Manager.GetDataFromPathDestination();
            if (tracks != null)
            {
                Manager.Display(tracks);
            }
        }




    }
}


