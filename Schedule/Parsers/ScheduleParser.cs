using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Excel;
using Schedule.Helpers;
using TeacherSchedule.Models;

namespace Schedule.Parsers
{
    public class ScheduleParser : IDisposable
    {
        private readonly IExcelDataReader _reader;
        private const int MaxCountOfLesson = 8;
        private readonly ScheduleContext _db = new ScheduleContext();
        private bool _isEndOfFile;
        private List<Teacher> _teachers = new List<Teacher>();

        public ScheduleParser(FileStream stream)
        {
            string fileType = stream.Name.Split('.').Last();
            if (fileType != "xls" && fileType != "xlsx")
                throw new NotExcelDocumentFormatException();

            _reader = fileType == "xls" ? 
                ExcelReaderFactory.CreateBinaryReader(stream) : 
                ExcelReaderFactory.CreateOpenXmlReader(stream);

            SetCursorToScheduleBegin();
        }

        private void SetCursorToScheduleBegin()
        {
            for (int i = 0; i < 20; i++)
            {
                if (_isEndOfFile || ReadRow().Contains("расписан"))
                    return;

                _isEndOfFile = !ReadNextRow();
            }
        }

        public string ReadRow()
        {
            var row = new StringBuilder();
            for (int i = 0; i < _reader.FieldCount; i++)
                row.Append(_reader.GetString(i));

            var result = row.ToString();
            return result;
        }

        public bool ReadNextRow()
        {
            while (true)
            {
                bool isNotEnd = _reader.Read();
                if (!isNotEnd)
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    return isNotEnd;

                for (int i = 0; i < _reader.FieldCount; i++)
                {
                    if (!string.IsNullOrEmpty(_reader.GetString(i)))
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        return isNotEnd;
                }
            }
        }

        public List<Teacher> GetTeachersSchedules()
        {
            if (_isEndOfFile) return null;
            do {
                string row = ReadRow();
                if (row.Contains("расписан"))
                {
                    _teachers.Add(GetTeacherSchedule());
                }
            } while (ReadNextRow());
            return _teachers;
        }

        public Teacher GetTeacherSchedule()
        {
            ReadNextRow();
            var teacher = GetTeacher();
            ReadNextRow();
            var lessons = new List<Lesson>();

            for (int number = 0; number < MaxCountOfLesson; number++)
            {
                string[][] rowOfLessons = new string[4][];
                for (int i = 0; i < rowOfLessons.Length; i++)
                {
                    ReadNextRow();
                    rowOfLessons[i] = new string[_reader.FieldCount];
                    for (int j = 0; j < rowOfLessons[i].Length; j++)
                    {
                        rowOfLessons[i][j] = _reader.GetString(j)?.Trim();
                    }
                }

                for (int numberOfWeek = 1; numberOfWeek <= 2; numberOfWeek++)
                {
                    var weekLessons = new Dictionary<int, Lesson>();

                    for (int j = 1; j < 7; j++)
                    {
                        string cell = rowOfLessons[(numberOfWeek - 1) * 2][j];
                        if (String.IsNullOrEmpty(cell))
                            continue;

                        var lesson = GetLesson(cell, number + 1, j, numberOfWeek, teacher);
                        if (lesson != null)
                            weekLessons.Add(j, lesson);
                    }

                    for (int j = 1; j < 7; j++)
                    {
                        string cell = rowOfLessons[numberOfWeek + (numberOfWeek - 1)][j];
                        if (string.IsNullOrEmpty(cell))
                            continue;

                        try
                        {
                            weekLessons[j].Name = cell;
                        }
                        catch (KeyNotFoundException)
                        {
                            var lesson1 = GetLesson(cell, number + 1, j, 1, teacher);
                            var lesson2 = GetLesson(cell, number + 1, j, 2, teacher);

                            cell = rowOfLessons[2][j];
                            rowOfLessons[2][j] = "";
                            lesson1.Name = lesson2.Name = cell;

                            lessons.Add(lesson1);
                            lessons.Add(lesson2);
                        }
                    }

                    lessons.AddRange(weekLessons.Values.ToList());
                }
            }

            teacher.Lessons = lessons;
            return teacher;
        }

        private Teacher GetTeacher()
        {
            var teacher = new Teacher();
            string name = _reader.GetString(1)?.Trim();
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                string[] namePaths = name.Split();
                teacher.Name = namePaths[0] + " " + namePaths[1] + "." + namePaths[2] + ".";
            }
            catch (Exception)
            {
                teacher.Name = name;
                ErrorNotificationToMail.Warninig("Ошибка при парсинге имени", "Не удалось правильно распарсить имя: " + name);
            }

            var cathedraName = _reader.GetString(2)?.Trim();
            var facultyName = Departments.GetFaculty(cathedraName);
            teacher.Cathedra = new Cathedra { Name = cathedraName, Faculty = new Faculty { Name = facultyName } };
            return teacher;
        }

        private Lesson GetLesson(string cell, int number, int dayOfWeek, int numberOfWeek, Teacher teacher)
        {
            var cellParts = cell.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (cellParts.Length < 2)
                return null;

            var lesson = new Lesson
            {
                Number = number,
                Cabinet = cellParts[1],
                DayOfWeek = dayOfWeek,
                NumberOfWeek = numberOfWeek,
                TeacherId = teacher.Id,
                Teacher = teacher
            };

            var groupName = cellParts[0];
            lesson.Group = new Group { Name = groupName };

            return lesson;
        }

        public void SaveDataInDatabase()
        {
            var faculties = _db.Faculties.ToList();
            var cathedries = _db.Cathedries.ToList();
            var teachers = _db.Teachers.ToList();
            var groups = _db.Groups.ToList();

            foreach (var teacher in _teachers)
            {
                var faculty = faculties.FirstOrDefault(f => f.Name == teacher.Cathedra.Faculty.Name);
                if (faculty == null)
                {
                    faculty = new Faculty { Name = teacher.Cathedra.Faculty.Name };
                    _db.Faculties.Add(faculty);
                    _db.SaveChanges();
                }
                teacher.Cathedra.Faculty = faculty;
                teacher.Cathedra.FacultyId = faculty.Id;

                var cathedra = cathedries.FirstOrDefault(f => f.Name == teacher.Cathedra.Name);
                if (cathedra == null)
                {
                    cathedra = new Cathedra
                    {
                        Name = teacher.Cathedra.Name,
                        FacultyId = teacher.Cathedra.Faculty.Id
                    };
                    _db.Cathedries.Add(cathedra);
                    _db.SaveChanges();
                }
                teacher.Cathedra = cathedra;
                teacher.CathedraId = cathedra.Id;

                var teach = teachers.FirstOrDefault(t => t.Name == teacher.Name);
                if (teach == null)
                {
                    teach = new Teacher
                    {
                        Name = teacher.Name,
                        CathedraId = teacher.Cathedra.Id
                    };
                    _db.Teachers.Add(teach);
                    _db.SaveChanges();
                }
                teacher.Id = teach.Id;

                _db.Database.ExecuteSqlCommand("DELETE FROM [Lessons] WHERE [TeacherId] = " + teacher.Id);

                var lessons = new List<Lesson>(teacher.Lessons.Count);
                foreach (var l in teacher.Lessons)
                {
                    var group = groups.FirstOrDefault(g => g.Name == l.Group.Name);
                    if (group == null)
                    {
                        group = new Group { Name = l.Group.Name };
                        _db.Groups.Add(group);
                        _db.SaveChanges();
                    }

                    var lesson = new Lesson
                    {
                        Name = l.Name,
                        Number = l.Number,
                        NumberOfWeek = l.NumberOfWeek,
                        GroupId = group.Id,
                        Group = group,
                        Cabinet = l.Cabinet,
                        DayOfWeek = l.DayOfWeek,
                        TeacherId = teacher.Id
                    };
                    lessons.Add(lesson);
                }
                _db.Lessons.AddRange(lessons);
                _db.SaveChanges();
            }
        }


        public void Dispose()
        {
            _db.Dispose();
            _reader.Dispose();
            _teachers = null;

            GC.Collect();
        }
    }

    public class NotExcelDocumentFormatException : Exception
    {
        public NotExcelDocumentFormatException() : base("File have not Excel document format (xls or xslx)")
        { }
    }
}
