using System;
using System.Threading;
using System.Web;
using Schedule.Controllers;
using Schedule.Parsers;

namespace Schedule.Modules
{
    public class ScheduleUpdate : IHttpModule
    {
        private const long Interval = 3600000; //every hour
        private static Timer _timer;
        private static readonly object Synclock = new object();
        private static bool _sent;

        public void Init(HttpApplication context)
        {
            _timer = new Timer(Update, null, 0, Interval);
        }

        public void Dispose()
        {
        }

        private static void Update(object state)
        {
            lock (Synclock)
            {
                var lastUpdate = DateParser.GetLastDate();

                if (!UpdateToDay(lastUpdate) && _sent == false)
                {
                    try
                    {
                        new ScheduleController().UpdateStudentSchedule();
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else
                {
                    _sent = false;
                }
            }
        }

        private static bool UpdateToDay(DateTime lastUpdate)
        {
            var now = DateTime.Now;

            return now.Day == lastUpdate.Day && now.Month == lastUpdate.Month && now.Year == lastUpdate.Year;
        }
    }
}