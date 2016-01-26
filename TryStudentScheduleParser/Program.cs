using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Schedule.Models;
using Schedule.Parsers;

namespace TryStudentScheduleParser
{
    class Program
    {
        static void Main(string[] args)
        {
            StudentScheduleParser parser = new StudentScheduleParser();

            var groups = parser.GetGroups();
            var lessons = parser.GetSchedule();

            var resultFile = File.Open("result.txt", FileMode.Create, FileAccess.ReadWrite);
            var sw = new StreamWriter(resultFile);

            sw.WriteLine("========================================= ВЫВОД ДАННЫХ ============================================\r\n");
            foreach (var group in groups)
            {
                var info = new StringBuilder();
                info.AppendFormat("Group: {0} \r\n\r\n", group.Name);

                foreach (var l in lessons.Where(l => l.Group.Name == group.Name))
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
