using Excel;
using Schedule.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TeacherSchedule.Models;

namespace TeacherSchedule
{
    public class ScheduleParser : IDisposable
    {
        private IExcelDataReader _Reader;
        private const int MAX_COUNT_OF_LESSON = 8;
        private ScheduleContext db = new ScheduleContext();

        public ScheduleParser(FileStream stream)
        {
            string fileType = stream.Name.Split('.').Last();
            if (fileType != "xls" && fileType != "xlsx")
                throw new NotExcelDocumentFormatException();

            if (fileType == "xls")
                _Reader = ExcelReaderFactory.CreateBinaryReader(stream);
            else
                _Reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        }

        public string ReadRow()
        {
            var row = new StringBuilder();
            for (int i = 0; i < _Reader.FieldCount; i++)
            {
                row.Append(_Reader.GetString(i));
            }
            var result = row.ToString();
            return result;
        }

        public bool ReadNextRow()
        {
            while (true)
            {
                bool isNotEnd = _Reader.Read();
                if (!isNotEnd)
                    return isNotEnd;

                for (int i = 0; i < _Reader.FieldCount; i++)
                {
                    if (!String.IsNullOrEmpty(_Reader.GetString(i)))
                        return isNotEnd;
                }
            }
        }

        public Teacher GetTeacherShedule()
        {
            ReadNextRow();
            var teacher = GetTeacher();
            ReadNextRow();
            var lessons = new List<Lesson>();

            for (int number = 0; number < MAX_COUNT_OF_LESSON; number++)
            {
                string[][] row_of_lessons = new string[4][];
                for (int i = 0; i < row_of_lessons.Length; i++)
                {
                    ReadNextRow();
                    row_of_lessons[i] = new string[_Reader.FieldCount];
                    for (int j = 0; j < row_of_lessons[i].Length; j++)
                    {
                        row_of_lessons[i][j] = _Reader.GetString(j)?.Trim();
                    }
                }

                for (int number_of_week = 1; number_of_week <= 2; number_of_week++)
                {
                    var week_lessons = new Dictionary<int, Lesson>();

                    for (int j = 1; j < 7; j++)
                    {
                        string cell = row_of_lessons[(number_of_week - 1) * 2][j];
                        if (String.IsNullOrEmpty(cell))
                            continue;

                        var lesson = GetLesson(cell, number + 1, j, number_of_week, teacher);
                        if (lesson != null)
                            week_lessons.Add(j, lesson);
                    }

                    for (int j = 1; j < 7; j++)
                    {
                        string cell = row_of_lessons[number_of_week + (number_of_week - 1)][j];
                        if (String.IsNullOrEmpty(cell))
                            continue;

                        try {
                            week_lessons[j].Name = cell;
                        }
                        catch (KeyNotFoundException ex)
                        {
                            var lesson1 = GetLesson(cell, number + 1, j, 1, teacher);
                            var lesson2 = GetLesson(cell, number + 1, j, 2, teacher);

                            cell = row_of_lessons[2][j];
                            lesson1.Name = lesson2.Name = cell;

                            lessons.Add(lesson1);
                            lessons.Add(lesson2);
                        }
                    }

                    lessons.AddRange(week_lessons.Values.ToList());
                }
            }
            teacher.Lessons = lessons;
            return teacher;
        }

        private Teacher GetTeacher()
        {
            var teacher = new Teacher();
            string name = _Reader.GetString(1).Trim();
            string[] name_paths = name.Split();
            try {
                teacher.Name = name_paths[0] + " " + name_paths[1] + "." + name_paths[2] + ".";
            }
            catch (ArgumentOutOfRangeException ex)
            {
                teacher.Name = name;
                ErrorNotificationToMail.Warninig("Ошибка при парсинге имени", "Не удалось правильно распарсить имя: " + name);
            }

            var cathedra_name = _Reader.GetString(2).Trim();
            var cathedra = db.Cathedries.Where(c => c.Name == cathedra_name).FirstOrDefault();
            if (cathedra == null)
            {
                cathedra = new Cathedra { Name = cathedra_name };
                db.Cathedries.Add(cathedra);
                db.SaveChanges();
            }

            teacher.CathedraId = cathedra.Id;
            teacher.Cathedra = cathedra;

            var faculty_name = Departments.GetFaculty(cathedra_name);
            var faculty = db.Faculties.Where(f => f.Name == faculty_name).FirstOrDefault();
            if (faculty == null)
            {
                faculty = new Faculty { Name = faculty_name };
                db.Faculties.Add(faculty);
                db.SaveChanges();
            }

            return teacher;
        }

        private Lesson GetLesson(string cell, int number, int day_of_week, int number_of_week, Teacher teacher)
        {
            string[] temp = cell.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length < 2)
                return null;

            var lesson = new Lesson
            {
                Number = number,
                Cabinet = temp[1],
                DayOfWeek = day_of_week,
                NumberOfWeek = number_of_week,
                TeacherId = teacher.Id,
                Teacher = teacher
            };

            var t = temp[0];
            var group = db.Groups.Where(g => g.Name == t).FirstOrDefault();
            if (group == null)
            {
                group = new Group { Name = temp[0] };
                db.Groups.Add(group);
                db.SaveChanges();
            }
            lesson.Group = group;
            lesson.GroupId = group.Id;

            return lesson;
        }

        public void Dispose()
        {
            db.Dispose();
            _Reader.Dispose();
        }
    }

    public class NotExcelDocumentFormatException : Exception
    {
        public NotExcelDocumentFormatException(): base("File have not Excel document format (xls or xslx)")
        { }
    }
}
