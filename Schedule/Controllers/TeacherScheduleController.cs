using Newtonsoft.Json;
using Schedule.Helpers;
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
            using (var downloader = new ExcelDocumentDownloader())
            {
                var docs = downloader.DownloadDocuments();
                foreach (var doc in docs)
                {
                    var parser = new ScheduleParser(doc);
                    parser.GetTeachersSchedules();
                    parser.SaveDataInDatabase();
                    parser.Dispose();
                }
            }
        }
    }
}