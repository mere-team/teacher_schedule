using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Schedule.Models;
using Schedule.Models.Student_Schedule_Models;
using WebGrease.Css.Extensions;

namespace Schedule.Parsers
{
    public class StudentScheduleParser
    {
        private readonly StudentScheduleContext _db = new StudentScheduleContext();

        private readonly List<StudentGroup> _groups = new List<StudentGroup>();
        private readonly List<StudentLesson> _lessons = new List<StudentLesson>();
        private readonly List<StudentTeacher> _teachers = new List<StudentTeacher>();

        private readonly List<string> _scheduleUrls = new List<string>();
        //private readonly string _examsDatesUrl = "http://www.ulstu.ru/main/view/article/12443";

        private readonly string _groupsScheduleUrl = "http://www.ulstu.ru/schedule/students/raspisan.htm";
        private string[] _shortSurnames;

        public IEnumerable<StudentGroup> GetGroups()
        {
            if (_groups.Count == 0)
                ParseGroupsAndScheduleUrls();
            return _groups;
        }

        private void ParseGroupsAndScheduleUrls()
        {
            var webClient = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var html = webClient.DownloadString(_groupsScheduleUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rows = doc.DocumentNode.SelectNodes("//tr");
            rows.Skip(1).ForEach((node, i) =>
            {
                if (node == null) return;

                var cells = node.SelectNodes(".//td/a");
                cells.ForEach(cell =>
                {
                    if (string.IsNullOrEmpty(cell.InnerText)) return;

                    var groupName = cell.InnerText.Trim();
                    var group = new StudentGroup { Name = groupName };
                    _groups.Add(group);

                    var link = "http://www.ulstu.ru/schedule/students/" + cell.Attributes["href"].Value;
                    _scheduleUrls.Add(link);
                });
            });
        }

//        private void GetDateExam()
//        {
//            var doc = new HtmlDocument();
//            doc.LoadHtml(_examsDatesUrl);
//            var noAltElements = doc.DocumentNode.SelectNodes("//li/a");
//            if (noAltElements != null)
//            {
//                foreach (var text in noAltElements)
//                {
//                    //DEI.Name = text.InnerText.Trim();
//                    //DEI.date = text.Attributes["title"].Value;
//                }
//            }
//        }

        public IEnumerable<StudentLesson> GetSchedule()
        {
            if (_groups.Count == 0)
                ParseGroupsAndScheduleUrls();

            if (_lessons.Count != 0)
                return _lessons;

            _shortSurnames = new string[0];
            try
            {
                var teacherDb = new ScheduleContext();
                var teachers = teacherDb.Teachers.Select(t=> t.Name).ToList();
                _shortSurnames = teachers.Where(s => s.Split(' ').First().Length <= 4).ToArray();
            }
            catch (Exception ex)
            {
                _shortSurnames = new[] {"ЮДИН", "ТУР"};
            }
            var webClient = new WebClient {Encoding = Encoding.GetEncoding("windows-1251")};
            for (int k = 0; k < _scheduleUrls.Count; k++)
            {
                var group = _groups[k];
                var url = _scheduleUrls[k];
                var html = webClient.DownloadString(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var rows = doc.DocumentNode.SelectNodes("//tr");
                rows.Skip(2).ForEach((row, i) =>
                {
                    if (row == null) return;

                    var cols = row.SelectNodes(".//td/font");
                    cols.Skip(1).ForEach((col, j) =>
                    {
                        if (col.InnerText.Trim() == "_") return;

                        var lesson = Algorithm2(group, col, i, j);

                        if (lesson.Name.Contains("Физкультура")) lesson.Name = "Физкультура";

                        _lessons.Add(lesson);
                    });
                });
            }

            return _lessons;
        }

        public void SaveInDatabase()
        {
            GetSchedule();

            _db.Database.ExecuteSqlCommand("DELETE FROM [StudentLessons]");

            _lessons.ForEach(l =>
            {
                var group = _db.Groups.FirstOrDefault(g => g.Name == l.Group.Name);
                if (group == null)
                {
                    group = new StudentGroup {Name = l.Group.Name};
                    try {
                        _db.Groups.Add(group);
                        _db.SaveChanges();
                    } catch (Exception ex) {
                        string error = ex.Message;
                    }
                }
                l.Group = group;
                l.GroupId = group.Id;

                var teacher = _db.Teachers.FirstOrDefault(t => t.Name == l.Teacher.Name);
                if (teacher == null)
                {
                    teacher = new StudentTeacher {Name = l.Teacher.Name};
                    try { 
                        _db.Teachers.Add(teacher);
                        _db.SaveChanges();
                    } catch (Exception ex) {
                        string error = ex.Message;
                    }
                }
                l.Teacher = teacher;
                l.TeacherId = teacher.Id;

                var lesson = new StudentLesson
                {
                    Name = l.Name,
                    Number = l.Number,
                    NumberOfWeek = l.NumberOfWeek,
                    Cabinet = l.Cabinet,
                    DayOfWeek = l.DayOfWeek,
                    GroupId = l.GroupId,
                    TeacherId = l.TeacherId
                };
                try { 
                    _db.Lessons.Add(lesson);
                    _db.SaveChanges();
                } catch (Exception ex) {
                    string error = ex.Message;
                }
            });
        }

        private StudentLesson Algorithm1(StudentGroup group, HtmlNode node, int i, int j)
        {
            var cellPieces = node.InnerText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var lessonName = cellPieces.First() + " ";
            var subgroup = cellPieces.Skip(1)
                .TakeWhile(word => word.ToCharArray()
                                    .All(c => (char.IsLower(c) || char.IsNumber(c) || char.IsPunctuation(c))) ||
                                        (word.Length <= 4 && !_shortSurnames.Contains(word)))
                                    .DefaultIfEmpty().ToList();
            lessonName += subgroup.Aggregate((word, nextWord) => word + " " + nextWord);

            var lesson = new StudentLesson
            {
                Number = j + 1,
                DayOfWeek = (i + 1) >= 7 ? i - 5 : i + 1,
                NumberOfWeek = (i + 1) >= 7 ? 2 : 1,
                Name = lessonName,
                Group = group,
                GroupId = group.Id,
                Cabinet = cellPieces.LastOrDefault()
            };

            foreach (var element in subgroup)
                cellPieces.Remove(element);

            cellPieces.Remove(cellPieces.First());
            cellPieces.Remove(lesson.Cabinet);
            cellPieces.RemoveAll(word => word.Trim().Length == 0);
            cellPieces = cellPieces.ConvertAll(word => (word.Length == 1) ? word + "." : word[0] + word.ToLowerInvariant().Substring(1));
            var teacher = new StudentTeacher
            {
                Name = cellPieces.DefaultIfEmpty()
                            .Aggregate((word1, word2) => word1 + " " + word2)
                            ?.Replace("-", "")
            };
            lesson.Teacher = teacher;

            return lesson;
        }

        private StudentLesson Algorithm2(StudentGroup group, HtmlNode node, int i, int j)
        {
            var cellPieces = node.InnerText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            cellPieces = cellPieces.ConvertAll(word => word.EndsWith("-") ? word.Substring(0, word.Length - 1) : word);
            cellPieces.RemoveAll(word => word.Length == 0);

            var cabinets = cellPieces.Where(p => p.Any(char.IsDigit) && p.Contains("а.") || p.Contains("а.")).ToList();
            foreach (var c in cabinets)
            {
                int index = cellPieces.IndexOf(c) - 1;
                if (cellPieces[index] == "-")
                    cellPieces.RemoveAt(index);
                cellPieces.Remove(c);
            }

            string lessonName = null;

            var teachers = new List<string>();
            bool haveTeacher = true;
            while (haveTeacher && cellPieces.Count > 3)
            {
                int indexOfLast = cellPieces.Count - 1;
                var patronymic = cellPieces[indexOfLast];
                var name = cellPieces[indexOfLast - 1];
                var surname = cellPieces[indexOfLast - 2];
                if (patronymic.Length == 1 && name.Length == 1 &&
                    (surname.Length > 4 || _shortSurnames.Contains(surname)))
                {
                    string teacherName = surname[0] + surname.ToLower().Substring(1) + " " + name.ToUpper() + "." + patronymic.ToUpper();
                    teachers.Add(teacherName);
                    cellPieces.RemoveRange(indexOfLast - 2, 3);
                }
                else
                {
                    haveTeacher = false;
                }
            }
            if (!teachers.Any())
            {
                lessonName = cellPieces.First() + " ";
                var subgroup = cellPieces.Skip(1)
                    .TakeWhile(word => word.ToCharArray()
                                        .All(c => (char.IsLower(c) || char.IsNumber(c) || char.IsPunctuation(c))) ||
                                            (word.Length <= 4 && !_shortSurnames.Contains(word)))
                                        .DefaultIfEmpty().ToList();
                lessonName += subgroup.Aggregate((word, nextWord) => word + " " + nextWord);

                foreach (var element in subgroup)
                    cellPieces.Remove(element);

                cellPieces.Remove(cellPieces.First());
                cellPieces = cellPieces.ConvertAll(word => (word.Length == 1) ? word + "." : word[0] + word.ToLowerInvariant().Substring(1));
                var teacherName = cellPieces.DefaultIfEmpty()
                    .Aggregate((word1, word2) => word1 + " " + word2)
                    ?.Replace("-", "");
                teachers.Add(teacherName);
            }

            lessonName = lessonName ?? cellPieces.Aggregate((word, nextWord) => word + " " + nextWord);
            var lesson = new StudentLesson
            {
                Number = j + 1,
                DayOfWeek = (i + 1) >= 7 ? i - 5 : i + 1,
                NumberOfWeek = (i + 1) >= 7 ? 2 : 1,
                Name = lessonName,
                Group = group,
                GroupId = group.Id,
                Cabinet = cabinets.DefaultIfEmpty().Aggregate((word1, word2) => word1 + ", " + word2)
            };
            var teacher = new StudentTeacher
            {
                Name = teachers.DefaultIfEmpty().Aggregate((word1, word2) => word1 + ", " + word2)
            };
            lesson.Teacher = teacher;

            return lesson;
        }
    }
}