using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace Schedule.Helpers
{
    public class ExcelDocumentDownloader : IDisposable
    {
        private string _TeacherScheduleUrl = "http://www.ulstu.ru/schedule/teachers/";
        private int _CurrentPosition;
        private WebClient _Client;
        private string[] _Urls;

        public ExcelDocumentDownloader()
        {
            _Urls = GetUrls();
            this.FilterUrls();
            _Client = new WebClient();
        }

        public bool DownloadNextDocument(out FileStream document)
        {
            document = null;
            if (_CurrentPosition == _Urls.Length)
                return false;

            string server_path = HttpContext.Current.Server.MapPath("");
            var temp = server_path.Split('\\').Last();
            server_path = server_path.Replace(temp, "");
            string file_name = server_path + _CurrentPosition.ToString() + ".xls";

            _Client.DownloadFile(_TeacherScheduleUrl + _Urls[_CurrentPosition], file_name);
            document = File.Open(file_name, FileMode.Open, FileAccess.Read);

            _CurrentPosition++;
            return true;
        }

        private string[] GetUrls()
        {
            var client = new WebClient();
            string html = client.DownloadString(_TeacherScheduleUrl);

            Regex tagA = new Regex(@"(?inx)
                <a \s [^>]*
                    href \s* = \s*
                        (?<q> ['""] )
                            (?<url> [^""]+ )
                        \k<q>
                [^>]* >"
            );

            var matches = tagA.Matches(html);
            var urls = new string[matches.Count];

            for(int i = 0; i < urls.Length; i++)
            {
                urls[i] = matches[i].Groups["url"].ToString();
            }

            return urls;
        }

        private void FilterUrls()
        {
            _Urls = _Urls.Where(u => u.EndsWith(".xls") || u.EndsWith(".xlsx")).ToArray();
        }

        public void Dispose()
        {
            _Client.Dispose();
        }
    }
}