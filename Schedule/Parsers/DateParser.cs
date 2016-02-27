using System;
using System.Net;

namespace Schedule.Parsers
{
    public static class DateParser
    {
        private static readonly string _studentScheduleUrl = "http://www.ulstu.ru/schedule/students/raspisan.htm";
        private static readonly string _teacherScheduleUrl = "http://www.ulstu.ru/main/view/article/200";


        public static DateTime GetLastUpdateStudentSchedule()
        {
            var myHttpWebRequest = (HttpWebRequest) WebRequest.Create(_studentScheduleUrl);
            var myHttpWebResponse = (HttpWebResponse) myHttpWebRequest.GetResponse();

            var dayUpdate = myHttpWebResponse.LastModified;

            myHttpWebResponse.Close();

            return dayUpdate;
        }

        public static DateTime GetLastUpdateTeacherSchedule()
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(_teacherScheduleUrl);
            var myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            var dayUpdate = myHttpWebResponse.LastModified;

            myHttpWebResponse.Close();

            return dayUpdate;
        }
    }
}