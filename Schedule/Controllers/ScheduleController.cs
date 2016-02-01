using Schedule.Helpers;
using System;
using System.Data.Entity;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Schedule.Models;
using Schedule.Models.Student_Schedule_Models;
using Schedule.Parsers;

namespace Schedule.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule
        public string UpdateTeacherSchedule()
        {
            try
            {
                using (var downloader = new ExcelDocumentDownloader())
                {
                    FileStream doc;
                    while(downloader.DownloadNextDocument(out doc))
                    { 
                        var parser = new TeacherScheduleParser(doc);
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

                return title + message;
            }

            return "Данные расписания преподавателей обновлены";
        }

        public string UpdateStudentSchedule()
        {
            try
            {
                var parser = new StudentScheduleParser();
                parser.SaveInDatabase();
            }
            catch (Exception ex)
            {
                Logger.E(ex);
            }

            string log = Logger.LogMessages;
            Logger.ClearMessages();
            return "Данные расписания студентов обновлены<br><br>" + log;
        }

        public string RecreateDatabase()
        {
            var db = new ScheduleContext();

            db.Database.Delete();
            db.Database.CreateIfNotExists();
            db.Database.Initialize(true);
            Database.SetInitializer(new DropCreateDatabaseAlways<ScheduleContext>());
            Database.SetInitializer(new DropCreateDatabaseAlways<ApplicationDbContext>());
            Database.SetInitializer(new DropCreateDatabaseAlways<StudentScheduleContext>());
            // Drop migration history table if not works

            return "БД создана заново";
        }
    }
}