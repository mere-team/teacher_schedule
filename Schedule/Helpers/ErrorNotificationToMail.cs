using System;
using System.Text;

namespace Schedule.Helpers
{
    public static class ErrorNotificationToMail
    {
        private static string _StmpServer = "smtp.google.com";
        private static string _Email = "we.are.mere.team@gmail.com";
        private static string _Password = "liljohnlolita";


        public static void Warninig(string title, string message)
        {
            title = "WARNING: " + title;
            Mail.SendMail(_StmpServer, _Email, _Password, _Email, title, message);
        }

        public static void Error(Exception ex)
        {
            string title = "ERROR: " + ex.Message;
            var message = new StringBuilder();
            message.Append("Message:\r\n" + ex.Message + "\r\n\r\n");
            message.Append("StackTrace:\r\n" + ex.StackTrace + "\r\n\r\n");
            if (ex.InnerException != null)
                message.Append("InnerException.Message:\r\n" + ex.InnerException.Message + "\r\n\r\n");

            Mail.SendMail(_StmpServer, _Email, _Password,/* _Email*/"max.mrtnv@gmail.com", title, message.ToString());
        }
    }
}