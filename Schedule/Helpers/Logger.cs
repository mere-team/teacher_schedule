using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Schedule.Helpers
{
    public static class Logger
    {
        private static readonly StringBuilder Sb = new StringBuilder();

        public static void I(string info) =>
            Sb.AppendLine("INFO: " + info).AppendLine();

        public static void E(string error) =>
            Sb.AppendLine("ERROR: " + error).AppendLine();

        public static void E(Exception e)
        {
            Sb.AppendLine("ERROR: " + e.Message);
            if (e.InnerException != null)
                Sb.AppendLine("ERROR INNER: " + e.InnerException.Message);
            Sb.AppendLine();
        }

    public static string LogMessages => 
            Sb.ToString();

        public static void ClearMessages() => Sb.Clear();
    }
}