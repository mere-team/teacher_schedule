using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeacherSchedule.Models;
using System.IO;
using TeacherSchedule;

namespace UnitTestTeacherScheduleParser
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void GeneralTest()
        {
            var xls_file = File.Open(@"TestFiles\GeneralTest.xls", FileMode.Open, FileAccess.Read);
            var result_file = File.Open(@"TestFiles\GeneralTest.txt", FileMode.Open, FileAccess.Read);
            var result = new StreamReader(result_file);
            Assert.IsNotNull(result);

            var parser = new ScheduleParser(xls_file);
            var teacher = new Teacher();

            while (parser.ReadNextRow())
            {
                string row = parser.ReadRow();

                if (row.Contains("расписан"))
                {
                    teacher = parser.GetTeacherSchedule();
                    
                    string test_teacher = String.Format("N: {0}; F: {1}; C: {2}",
                        teacher.Name,
                        teacher.Faculty.Name,
                        teacher.Cathedra?.Name);
                    string result_teacher = ReadRowFrom(result);

                    Assert.AreEqual(result_teacher, test_teacher);

                    foreach (Lesson l in teacher.Lessons)
                    {
                        string test_lesson = String.Format("N: {0}; N: {1}; D: {2}; WN: {3}; C: {4}; G: {5}; T: {6}",
                            l.Number,
                            l.Name,
                            l.DayOfWeek,
                            l.NumberOfWeek,
                            l.Cabinet,
                            l.Group.Name,
                            l.Teacher.Name);
                        string result_lesson = ReadRowFrom(result);

                        Assert.AreEqual(test_lesson, result_lesson);
                    }
                }
            }
        }

        private string ReadRowFrom(StreamReader sr)
        {
            string row;
            do
            {
                row = sr.ReadLine()?.Trim();
            } while (row == "");

            return row;
        }


    }
}
