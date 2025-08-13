using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AudioFilesWorkC_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            YandexMusic.PathCopyTo = Manager.pathDestination!;
            //Manager.CopyFromYandexMusic();

            Manager.CheckDirDestination();


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


