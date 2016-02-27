using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Schedule.Helpers;
using Schedule.Models;
using Schedule.Models.Student_Schedule_Models;
using WebGrease.Css.Extensions;

namespace Schedule.Parsers
{
    public class StudentScheduleParser
    {
        private readonly StudentScheduleContext _db = new StudentScheduleContext();

        private readonly List<StudentGroup> _newGroups = new List<StudentGroup>();
        private readonly List<StudentTeacher> _newTeachers = new List<StudentTeacher>();
        private readonly List<StudentLesson> _newLessons = new List<StudentLesson>();

        private readonly List<string> _scheduleUrls = new List<string>();
        private readonly string _groupsScheduleUrl = "http://www.ulstu.ru/schedule/students/raspisan.htm";
        private string[] _shortSurnames;
        private readonly WebClient _webClient;

        public StudentScheduleParser()
        {
            _webClient = new WebClient {Encoding = Encoding.GetEncoding("windows-1251")};
        }

        public IEnumerable<StudentGroup> GetGroups()
        {
            if (_newGroups.Count == 0)
                ParseGroupsAndScheduleUrls();
            return _newGroups;
        }
        
        private void ParseGroupsAndScheduleUrls()
        {
            var html = _webClient.DownloadString(_groupsScheduleUrl);
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
                    _newGroups.Add(group);

                    var link = "http://www.ulstu.ru/schedule/students/" + cell.Attributes["href"].Value;
                    _scheduleUrls.Add(link);
                });
            });
        }

        public IEnumerable<StudentLesson> GetSchedule()
        {
            if (_newGroups.Count == 0)
                ParseGroupsAndScheduleUrls();

            if (_newLessons.Count != 0)
                return _newLessons;

            FindShortSurnames();

            for (int k = 0; k < _scheduleUrls.Count; k++)
            {
                var group = _newGroups[k];
                var url = _scheduleUrls[k];

                ParseHtmlPage(url, group);
            }

            return _newLessons;
        }

        public IEnumerable<StudentLesson> GetSchedule(string url)
        {
            if (_newGroups.Count == 0)
                ParseGroupsAndScheduleUrls();

            var group = _newGroups[_scheduleUrls.IndexOf(url)];

            if (_newLessons.Count != 0)
                return _newLessons.Where(l => l.GroupId == group.Id);

            FindShortSurnames();

            ParseHtmlPage(url, group);

            return _newLessons.Where(l => l.GroupId == group.Id);
        }

        private void ParseHtmlPage(string url, StudentGroup group)
        {
            var html = _webClient.DownloadString(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rows = doc.DocumentNode.SelectNodes("//tr");
            rows.Skip(2).ForEach((row, i) =>
            {
                if (row == null) return;

                var cols = row.SelectNodes(".//td/font");
                cols.ForEach((col, j) =>
                {
                    if (col.InnerText.Trim() == "_") return;

                    var lesson = ParseLessonHtmlNode(group, col.InnerText, i, j);

                    if (lesson.Name.Contains("Физкультура")) lesson.Name = "Физкультура";

                    _newLessons.Add(lesson);
                });
            });
        }

        private void FindShortSurnames()
        {
            _shortSurnames = new[] { "ЮДИН", "ТУР", "ЛЯХ" };
            try
            {
                var teacherDb = new ScheduleContext();
                var teachers = teacherDb.Teachers.Select(t => t.Name).ToList();
                var shortSurnames = teachers.Where(s => s.Split(' ').First().Length <= 4).ToList();
                shortSurnames.AddRange(_shortSurnames);
                _shortSurnames = shortSurnames.ToArray();
            }
            catch (Exception ex)
            {
                ex.ReportCrash();
            }
        }

        public void SaveInDatabase()
        {
            GetSchedule();

            var groups = SaveGroupsInDatabase();
            var teachers = SaveTeachersInDatabase();
            SaveLessonsInDatabase(groups, teachers);

            #region Old saving in db
            //            _newLessons.ForEach(l =>
            //            {
            //                var group = groups.FirstOrDefault(g => g.Name == l.Group.Name);
            //                if (group == null)
            //                {
            //                    group = new StudentGroup {Name = l.Group.Name};
            //                    try {
            //                        _db.Groups.Add(group);
            //                        _db.SaveChanges();
            //                        groups.Add(group);
            //                    } catch (Exception ex) {
            //                        Logger.E(ex);
            //                        Logger.I(group.ToString());
            //                    }
            //                }
            //                l.Group = group;
            //                l.GroupId = group.Id;
            //
            //                var teacher = teachers.FirstOrDefault(t => t.Name == l.Teacher.Name);
            //                if (teacher == null)
            //                {
            //                    teacher = new StudentTeacher {Name = l.Teacher.Name ?? ""};
            //                    try { 
            //                        _db.Teachers.Add(teacher);
            //                        _db.SaveChanges();
            //                        teachers.Add(teacher);
            //                    } catch (Exception ex) {
            //                        Logger.E(ex);
            //                        Logger.I(group.ToString());
            //                        Logger.I(teacher.ToString());
            //                    }
            //                }
            //                l.Teacher = teacher;
            //                l.TeacherId = teacher.Id;
            //
            //                var lesson = new StudentLesson
            //                {
            //                    Name = l.Name,
            //                    Number = l.Number,
            //                    NumberOfWeek = l.NumberOfWeek,
            //                    Cabinet = l.Cabinet,
            //                    DayOfWeek = l.DayOfWeek,
            //                    GroupId = l.GroupId,
            //                    TeacherId = l.TeacherId
            //                };
            //                try { 
            //                    _db.Lessons.Add(lesson);
            //                    _db.SaveChanges();
            //                } catch (Exception ex) {
            //                    Logger.E(ex);
            //                    Logger.I(group.ToString());
            //                    Logger.I(teacher.ToString());
            //                    Logger.I(lesson.ToString());
            //                }
            //            });
            #endregion
        }

        private void SaveLessonsInDatabase(List<StudentGroup> groups, List<StudentTeacher> teachers)
        {
            _db.Database.ExecuteSqlCommand("DELETE FROM [StudentLessons]");

            groups.ForEach(g =>
            {
                var groupLessons = _newLessons.Where(l => l.Group.Name == g.Name).ToList();
                foreach (var l in groupLessons)
                {
                    var teacher = teachers.First(t => t.Name == l.Teacher.Name);
                    l.Teacher = teacher;
                    l.TeacherId = teacher.Id;

                    l.Group = g;
                    l.GroupId = g.Id;
                }
                try
                {
                    _db.Lessons.AddRange(groupLessons);
                    _db.SaveChanges();
                    Logger.I($"Added group: {g.Name} [Id:{g.Id}]");
                }
                catch (Exception ex)
                {
                    Logger.E(ex);
                }
            });
            Logger.SubmitInfoMessages();
        }

        private List<StudentTeacher> SaveTeachersInDatabase()
        {
            var teachers = _db.Teachers.ToList();
            _newTeachers.ForEach(newTeacher =>
            {
                var teacher = teachers.FirstOrDefault(t => t.Name == newTeacher.Name);
                if (teacher != null) return;

                teacher = new StudentTeacher {Name = newTeacher.Name};
                _db.Teachers.Add(teacher);
                teachers.Add(teacher);
            });
            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.E(ex);
            }
            return teachers;
        }

        private List<StudentGroup> SaveGroupsInDatabase()
        {
            var groups = _db.Groups.ToList();

            _newGroups.ForEach(newGroup =>
            {
                var group = groups.FirstOrDefault(g => g.Name == newGroup.Name);
                if (group != null) return;

                group = new StudentGroup {Name = newGroup.Name};
                _db.Groups.Add(group);
                groups.Add(group);
            });
            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.E(ex);
            }
            return groups;
        }

        private StudentLesson ParseLessonHtmlNode(StudentGroup group, string lessonInfo, int dayOfWeek, int lessonNumber)
        {
            var returnedLessonIfError = new StudentLesson
            {
                Number = lessonNumber + 1,
                DayOfWeek = (dayOfWeek + 1) >= 7 ? dayOfWeek - 5 : dayOfWeek + 1,
                NumberOfWeek = (dayOfWeek + 1) >= 7 ? 2 : 1,
                Name = lessonInfo,
                Group = group,
                GroupId = group.Id,
                Cabinet = ""
            };

            List<string> cellPieces;
            // Split lesson info to cell peaces. It contains lesson name, cabinets and teachers
            try
            {
                cellPieces = lessonInfo.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
                cellPieces = cellPieces.ConvertAll(word => word.EndsWith("-") ? word.Substring(0, word.Length - 1) : word);
                cellPieces.RemoveAll(word => word.Length == 0);
            }
            catch (Exception ex)
            {
                Logger.E(ex);
                return returnedLessonIfError;
            }

            // Delete from cell peaces strings equal "-" and cabinet. 
            // In cell peaces stay lesson name and teachers
            var cabinets = cellPieces.Where(p => p.Contains("а.")).ToList();
            foreach (var c in cabinets)
            {
                int index = cellPieces.IndexOf(c) - 1;
                if (cellPieces[index] == "-")
                    cellPieces.RemoveAt(index);
                cellPieces.Remove(c);
            }

            string lessonName = null;

            // Getting from cell teachers names
            var teachers = new List<string>();
            bool haveTeacher = true;
            // Cell peaces must contain lesson name and teacher
            // Find teachers name
            while (haveTeacher && cellPieces.Count > 3)
            {
                try
                {
                    int indexOfLast = cellPieces.Count - 1;
                    var patronymic = cellPieces[indexOfLast];
                    var name = cellPieces[indexOfLast - 1];
                    var surname = cellPieces[indexOfLast - 2];
                    if (patronymic.Length == 1 && name.Length == 1 &&
                        (surname.Length >= 3 || _shortSurnames.Contains(surname)))
                    {
                        string teacherName = surname[0] + surname.ToLower().Substring(1) + " " + name.ToUpper() + "." +
                                             patronymic.ToUpper();
                        teachers.Add(teacherName);
                        cellPieces.RemoveRange(indexOfLast - 2, 3);
                    }
                    else
                    {
                        haveTeacher = false;
                    }
                }
                catch (Exception ex)
                {
                    Logger.E(ex);
                    break;
                }
            }
            // If teachers not found, use another algorithm. 
            // First find lesson name, then teacher name
            try
            {
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
                    cellPieces =
                        cellPieces.ConvertAll(
                            word => (word.Length == 1) ? word + "." : word[0] + word.ToLowerInvariant().Substring(1));
                    var teacherName = cellPieces.DefaultIfEmpty()
                        .Aggregate((word1, word2) => word1 + " " + word2)
                        ?.Replace("-", "");
                    teachers.Add(teacherName);
                }
            }
            catch (Exception ex)
            {
                Logger.E(ex);
                return returnedLessonIfError;
            }

            lessonName = lessonName ?? cellPieces.Aggregate((word, nextWord) => word + " " + nextWord);
            var lesson = new StudentLesson
            {
                Number = lessonNumber + 1,
                DayOfWeek = (dayOfWeek + 1) >= 7 ? dayOfWeek - 5 : dayOfWeek + 1,
                NumberOfWeek = (dayOfWeek + 1) >= 7 ? 2 : 1,
                Name = lessonName,
                Group = group,
                GroupId = group.Id,
                Cabinet = cabinets.DefaultIfEmpty().Aggregate((word1, word2) => word1 + ", " + word2)
            };
            var teacher = new StudentTeacher
            {
                Name = teachers.DefaultIfEmpty().Aggregate((word1, word2) => word1 + ", " + word2) ?? "-"
            };
            lesson.Teacher = teacher;
            _newTeachers.Add(teacher);

            return lesson;
        }
    }
}