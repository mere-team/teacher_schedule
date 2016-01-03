using System.IO;
using System.Net;
using TeacherSchedule.Models;
using System.Text;
using System.Diagnostics;
using Schedule.Parsers;

namespace TryTeacherSheduleParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string remoteUri = "http://www.ulstu.ru/schedule/teachers/%88%ad%e4%ae%e0%ac%a0%e6%a8%ae%ad%ad%eb%a5%20%e1%a8%e1%e2%a5%ac%eb.xls";
            string fileName = "foreign_languages.xls";
            var webClient = new WebClient();
            Teacher teacher = new Teacher();

            webClient.DownloadFile(remoteUri, fileName);

            var file = File.Open(fileName, FileMode.Open, FileAccess.Read);
            var result_file = File.Open("result.txt", FileMode.Create, FileAccess.ReadWrite);
            var sw = new StreamWriter(result_file);
            var parser = new ScheduleParser(file);

            sw.WriteLine("========================================= ВЫВОД ДАННЫХ ============================================\r\n");
            while (parser.ReadNextRow())
            {
                string row = parser.ReadRow();

                if (!row.Contains("расписан"))
                    continue;

                teacher = parser.GetTeacherSchedule();

                var info = new StringBuilder();
                info.AppendFormat("N: {0}   C: {1}\r\n\r\n",
                    teacher.Name,
                    teacher.Cathedra?.Name);

                foreach (Lesson l in teacher.Lessons)
                {
                    info.AppendFormat("N: {0,-2}   N: {1,-17}  D: {2,-2}  WN: {3,-2}  C: {4,-12}  G: {5,-12}  T: {6,-16}\r\n",
                        l.Number,
                        l.Name,
                        l.DayOfWeek,
                        l.NumberOfWeek,
                        l.Cabinet,
                        l.Group.Name,
                        l.Teacher?.Name);
                }
                info.Append("-------------------------------------------------------------------------------------------------\r\n\n");
                sw.WriteLine(info.ToString());
            }
            sw.WriteLine("=========================================== КОНЕЦ ВЫВОДA ==============================================");
            sw.Close();

            Process.Start("result.txt");
        }
    }
}