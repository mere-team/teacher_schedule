using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TeacherSchedule;
using TeacherSchedule.Models;

namespace Schedule.Controllers
{
    public class TeacherScheduleController : Controller
    {
        // GET: TeacherSchedule
        public void Update()
        {
            string remoteUri = "http://www.ulstu.ru/schedule/teachers/%a8%ad.%ef%a7..xls";
#warning Указать правильный путь
            string fileName = "E:/foreign_languages.xls";
            var webClient = new WebClient();
            webClient.DownloadFile(remoteUri, fileName);
            var file = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read);
            var parser = new ScheduleParser(file);

            while (parser.ReadNextRow())
            {
                string row = parser.ReadRow();

                if (!row.Contains("расписан"))
                    continue;

                var teacher = parser.GetTeacherShedule();
            }
        }
    }
}