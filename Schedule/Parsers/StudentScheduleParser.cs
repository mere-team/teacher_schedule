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
        //private readonly StudentScheduleContext _db = new StudentScheduleContext();

        private readonly List<StudentGroup> _groups = new List<StudentGroup>();
        private readonly List<StudentLesson> _lessons = new List<StudentLesson>();
        private readonly List<string> _scheduleUrls = new List<string>();
        //private readonly string _examsDatesUrl = "http://www.ulstu.ru/main/view/article/12443";

        private readonly string _groupsScheduleUrl = "http://www.ulstu.ru/schedule/students/raspisan.htm";

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

                        var cell = col;
                        var cellPieces =
                            cell.InnerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var lessonName = cellPieces.First() + cellPieces.Skip(1)
                            .TakeWhile(word => word.Length < 4).DefaultIfEmpty()
                            .Aggregate((word, nextWord) => word + " " + nextWord);

                        var lesson = new StudentLesson
                        {
                            Number = j + 1,
                            DayOfWeek = i >= 7 ? i - 6 : i,
                            NumberOfWeek = i >= 7 ? 2 : 1,
                            Name = lessonName,
                            Group = group,
                            GroupId = group.Id,
                            Cabinet = cellPieces.LastOrDefault()
                        };
                        cellPieces.Remove(lesson.Name);
                        cellPieces.Remove(lesson.Cabinet);
                        var teacher = new StudentTeacher
                        {
                            Name = cellPieces.DefaultIfEmpty().Aggregate((word1, word2) => word1 + " " + word2)
                        };
                        lesson.Teacher = teacher;

                        _lessons.Add(lesson);
                    });
                });
            }

            return _lessons;
        }
    }
}