﻿using static System.Console;
using System.IO;
using System.Net;
using TeacherSheduleParser;
using TeacherSheduleParser.Models;
using System.Text;
using System.Diagnostics;

namespace TryTeacherSheduleParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string remoteUri = "http://www.ulstu.ru/schedule/teachers/%a8%ad.%ef%a7..xls";
            string fileName = "foreign_languages.xls";
            var webClient = new WebClient();
            Teacher teacher = new Teacher();

            //webClient.DownloadFile(remoteUri, fileName);

            var file = File.Open(fileName, FileMode.Open, FileAccess.Read);
            var result_file = File.Open("result.txt", FileMode.Create, FileAccess.ReadWrite);
            var sw = new StreamWriter(result_file);
            var parser = new SheduleParser(file);

            sw.WriteLine("================== ВЫВОД ДАННЫХ ======================");
            int count = 0;
            while (parser.ReadNextRow())// && count++ < 5)
            {
                string row = parser.ReadRow();

                if (row.Contains("расписан"))
                {
                    teacher = parser.GetTeacherShedule();

                    var info = new StringBuilder();
                    info.AppendFormat("N: {0}   F: {1}   C: {2}\r\n\r\n",
                        teacher.Name,
                        teacher.Faculty.Name,
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
                    info.Append("----------------------------------------------------------------\r\n\n");
                    sw.WriteLine(info.ToString());
                }
            }
            sw.WriteLine("================== КОНЕЦ ВЫВОДA ======================");
            sw.Close();

            Process.Start("result.txt");
        }
    }
}
