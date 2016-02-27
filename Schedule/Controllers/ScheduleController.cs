using Schedule.Helpers;
using System;
using System.Data.Entity;
using System.IO;
using System.Web.Mvc;
using Schedule.Models;
using Schedule.Models.Student_Schedule_Models;
using Schedule.Parsers;

namespace Schedule.Controllers
{
    public class ScheduleController : Controller
    {
        private const string STUDENT = "studentLastDateUpdate.txt";
        private const string TEACHER = "teacherLastDateUpdate.txt";


        // GET: Schedule
        public string UpdateTeacherSchedule()
        {
            if (HasChanges(TEACHER))
            {
                try
                {
                    using (var downloader = new ExcelDocumentDownloader())
                    {
                        FileStream doc;
                        while (downloader.DownloadNextDocument(out doc))
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
                catch (Exception ex)
                {
                    Logger.E(ex);
                    return ex.Message;
                }

                UpdateDate(TEACHER);
            }

            return "Данные расписания преподавателей обновлены";
        }

        public string UpdateStudentSchedule()
        {
            if (HasChanges(STUDENT))
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

                UpdateDate(STUDENT);
            }

            string log = Logger.LogMessages;
            Logger.ClearMessages();
            return "Данные расписания студентов обновлены<br><br>" + log;
        }

        private bool HasChanges(string fileName)
        {
            if (!System.IO.File.Exists(WebApiApplication.ServerPath + fileName))
                return true;

            string result;
            using (var sr = new StreamReader(WebApiApplication.ServerPath + fileName))
            {
                result = sr.ReadToEnd();
            }
            DateTime date;
            if (DateTime.TryParse(result, out date))
            {
                var lastUpdate = new DateTime();
                if (fileName == TEACHER)
                    lastUpdate = DateParser.GetLastUpdateTeacherSchedule();
                else if (fileName == STUDENT)
                    lastUpdate = DateParser.GetLastUpdateStudentSchedule();

                if (lastUpdate != date)
                    return true;
            }

            return false;
        }

        private void UpdateDate(string fileName)
        {
            using (var sw = new StreamWriter(WebApiApplication.ServerPath + fileName, false))
            {
                sw.WriteLine(DateParser.GetLastUpdateStudentSchedule());
            }
        }

        public string RecreateDatabase()
        {
            var db = new ScheduleContext();

            db.Database.Delete();
            // Drop all tables by hand if not works
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