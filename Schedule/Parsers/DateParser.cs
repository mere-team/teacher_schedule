using System;
using System.Net;

namespace Schedule.Parsers
{
    public static class DateParser
    {
        private static readonly string _scheduleUrl = "http://www.ulstu.ru/schedule/students/raspisan.htm";


        public static DateTime GetLastDate()
        {
            var myHttpWebRequest = (HttpWebRequest) WebRequest.Create(_scheduleUrl);
            var myHttpWebResponse = (HttpWebResponse) myHttpWebRequest.GetResponse();

            var dayUpdate = myHttpWebResponse.LastModified;

            myHttpWebResponse.Close();

            return dayUpdate;
        }
    }
}