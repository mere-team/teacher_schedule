using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using System.Text;
using TeacherSchedule.Models;

namespace Schedule.Parsers
{
    public class StudentScheduleParser
    {
        private ScheduleContext db = new ScheduleContext();

        public void GetGroups(string container)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(container);
            var rows = doc.DocumentNode.SelectNodes("//tr");
            for (int i = 1; i < rows.Count; i++)
            {
                if ((rows[i] != null))
                {
                    foreach (var cell in rows[i].SelectNodes(".//td/a"))
                    {
                        
                        if ((cell.InnerText != "") && (cell.InnerText != null))
                        {
                            //GI.NameGr = cell.InnerText.Trim();
                            //GI.LinkGR = "http://www.ulstu.ru/schedule/students/" + cell.Attributes["href"].Value;

                        }
                        
                    }
                }
            }
        }

        public void GetDateExam(string container)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(container);
            HtmlNodeCollection NoAltElements = doc.DocumentNode.SelectNodes("//li/a");
            if (NoAltElements != null)
            {
                foreach (var text in NoAltElements)
                {
                    
                    //DEI.Name = text.InnerText.Trim();
                    //DEI.date = text.Attributes["title"].Value;
                    
                }

            }
        }

        public void GetSchedule(string container)
        {
            System.Net.WebClient web = new System.Net.WebClient();
            web.Encoding = Encoding.GetEncoding("windows-1251");
            container = web.DownloadString("http://www.ulstu.ru/schedule/students/130.htm");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(container);
            HtmlNodeCollection NoAltElements = doc.DocumentNode.SelectNodes("//p");
            string namegroup;
            if (NoAltElements != null)
            {
                namegroup = NoAltElements[1].ParentNode.ChildNodes[0].InnerText.Trim();
            }

            var rows = doc.DocumentNode.SelectNodes("//tr");
            for (int i = 2; i < rows.Count; i++)
            {
                if ((rows[i] != null))
                {
                    var cols = rows[i].SelectNodes(".//td/font");
                    for (int j = 1; j < cols.Count; j++)
                    {
                        var cell = cols[j];
                        Lesson lesson = new Lesson();
                        lesson.Number = j;
                        lesson.DayOfWeek = (i >= 8) ? i - 7 : i - 1;
                        lesson.NumberOfWeek = (i >= 8) ? 2 : 1;
                        lesson.Name = cell.InnerText.Split(new char[] {' '}).FirstOrDefault();
                        //GI = new Group_info();
                        //if ((cell.InnerText != "") && (cell.InnerText != null))
                        //{
                        //    GI.NameGr = cell.InnerText.Trim();
                        //    GI.LinkGR = "http://www.ulstu.ru/schedule/students/" + cell.Attributes["href"].Value;

                        //}
                        //listGR.Add(GI);
                    }
                }
            }
        }
    }
}