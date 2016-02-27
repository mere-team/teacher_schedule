using System;
using System.Net;
using System.Net.Mail;

namespace Schedule.Helpers
{
    public static class CrashReport
    {
        public static void ReportCrash(this Exception exception)
        {
            
            string subject = DateTime.Now + " [" + exception.Message + "]";
            Exception ex = exception;
            string body = "Exception Message:\n" + ex.Message + "\n\n" 
                + "Exception Data:\n" + ex.Data + "\n\n" 
                + "StackTrace:\n" + ex.StackTrace + "\n\n"
                + "Source:\n" + ex.Source;

            Mail.SendMail(subject, body);
        }
    }
}