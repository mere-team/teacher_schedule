using System;
using System.Text;

namespace Schedule.Helpers
{
    public static class Logger
    {
        private static readonly StringBuilder Sb = new StringBuilder();
        private static int _infoMessageCounter;

        public static void I(string info)
        {
            Sb.AppendLine("INFO: " + info).AppendLine();
            if (_infoMessageCounter++ > 50)
            {
                SubmitInfoMessages();
                _infoMessageCounter = 0;
            }
        }

        public static void E(string error) =>
            Sb.AppendLine("ERROR: " + error).AppendLine();

        public static void E(Exception e)
        {
            Sb.AppendLine("ERROR: " + e.Message);
            if (e.InnerException != null)
                Sb.AppendLine("ERROR INNER: " + e.InnerException.Message);
            Sb.AppendLine();
            e.ReportCrash();
        }

        public static void SubmitInfoMessages()
        {
            var subject = DateTime.Now + "[INFO MESSAGE]";
            Mail.SendMail(subject, Sb.ToString());
            ClearMessages();
        }

        public static string LogMessages => 
            Sb.ToString();

        public static void ClearMessages() => Sb.Clear();
    }
}