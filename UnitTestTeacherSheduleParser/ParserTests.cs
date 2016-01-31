using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedule.Parsers;

namespace UnitTestTeacherSheduleParser
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void GeneralTest()
        {
            var xlsFile = File.Open(@"TestFiles\GeneralTest.xls", FileMode.Open, FileAccess.Read);
            var resultFile = File.Open(@"TestFiles\GeneralTest.txt", FileMode.Open, FileAccess.Read);
            var result = new StreamReader(resultFile);
            Assert.IsNotNull(result);

            var parser = new TeacherScheduleParser(xlsFile);

            while (parser.ReadNextRow())
            {
                var row = parser.ReadRow();

                if (row.Contains("расписан"))
                {
                    var teacher = parser.GetTeacherSchedule();
                    var testTeacher =
                        $"N: {teacher.Name}; F: {teacher.Cathedra?.Faculty?.Name}; C: {teacher.Cathedra?.Name}";
                    var resultTeacher = ReadRowFrom(result);

                    Assert.AreEqual(resultTeacher, testTeacher);

                    foreach (var l in teacher.Lessons)
                    {
                        var testLesson =
                            $"N: {l.Number}; N: {l.Name}; D: {l.DayOfWeek}; WN: {l.NumberOfWeek}; C: {l.Cabinet}; G: {l.Group.Name}; T: {l.Teacher.Name}";
                        var resultLesson = ReadRowFrom(result);

                        Assert.AreEqual(testLesson, resultLesson);
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