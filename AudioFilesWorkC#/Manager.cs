using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal class Manager
    {
        string? pathDestination = "";
        string? action = "";
        public Manager() { }

        public void PrintMessage()
        {
            Console.WriteLine("Программа работы  с аудиофайлами Яндекс Музыка");
            while (true)
            {
                Console.WriteLine("Укажите путь к папке назначения:");
                pathDestination = Console.ReadLine();
                if (Path.Exists(pathDestination)) break;
                else Console.WriteLine("Такого пути не существует.");
            }
            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("y - копирование файлов из Яндекс Музыка в указанную папку.");
                Console.WriteLine("d - копирование файлов из выбранной папки в  указанную папку");
                action = Console.ReadLine();
                switch (action)
                {
                    case "y":
                        Console.WriteLine("вызов метода у");
                        break;
                    case "d":
                        Console.WriteLine("вызов метода d");
                        break;
                    default:
                        Console.WriteLine("действие выбрано не корректно:");
                        break;
                }


            }
        }
    }
}
