using static System.Console;
using System.IO;
using System.Net;
using TeacherSheduleParser;

namespace TryTeacherSheduleParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string remoteUri = "http://www.ulstu.ru/schedule/teachers/%a8%ad.%ef%a7..xls";
            string fileName = "foreign_languages.xls";
            var webClient = new WebClient();

            webClient.DownloadFile(remoteUri, fileName);

            var file = File.Open(fileName, FileMode.Open, FileAccess.Read);
            var parser = new SheduleParser(file);

            WriteLine("================== ВЫВОД ДАННЫХ ======================");
            string row;
            while ((row = parser.ReadRow()) != null)
                WriteLine(row);
            WriteLine("================== КОНЕЦ ВЫВОДA ======================");

            ReadLine();
        }
    }
}
