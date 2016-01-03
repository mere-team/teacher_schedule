using Schedule.Helpers;
using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using TeacherSchedule;

namespace Schedule.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule
        public string Update()
        {
            try
            {
                using (var downloader = new ExcelDocumentDownloader())
                {
                    FileStream doc;
                    while(downloader.DownloadNextDocument(out doc))
                    { 
                        var parser = new ScheduleParser(doc);
                        parser.GetTeachersSchedules();
                        parser.SaveDataInDatabase();
                        parser.Dispose();
                        doc.Dispose();
                        System.IO.File.Delete(doc.Name);
                    }
                }
            }
            catch (Exception ex) {
                string title = "ERROR: " + ex.Message + "\r\n";
                var message = new StringBuilder();
                message.Append("Message:\r\n" + ex.Message + "\r\n\r\n");
                message.Append("StackTrace:\r\n" + ex.StackTrace + "\r\n\r\n");
                if (ex.InnerException != null)
                    message.Append("InnerException.Message:\r\n" + ex.InnerException.Message + "\r\n\r\n");

                return title + message.ToString();
            }

            return "Данные обновлены";
        }
    }
}