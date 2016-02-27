using System;
using System.Threading;
using System.Web;
using Schedule.Controllers;
using Schedule.Helpers;

namespace Schedule.Modules
{
    public class ScheduleUpdater : IHttpModule
    {
        private const long Interval = 900000; //every 15 minutes
        private static readonly object Synclock = new object();

        public void Init(HttpApplication context)
        {
            new Timer(Update, null, 0, Interval);
        }

        public void Dispose() { }

        private static void Update(object state)
        {
            lock (Synclock)
            {
                try
                {
                    var controller = new ScheduleController();
                    Logger.I("Started student schedule update task: " + DateTime.Now);
                    controller.UpdateStudentSchedule();
                    Logger.I("Complete student schedule update task: " + DateTime.Now);
                    Logger.I("Started teacher schedule update task: " + DateTime.Now);
                    controller.UpdateTeacherSchedule();
                    Logger.I("Complete teacher schedule update task: " + DateTime.Now);
                }
                catch (Exception ex)
                {
                    Logger.E(ex);
                }
                finally
                {
                    Logger.SubmitInfoMessages();
                }
            }
        }
    }
}