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
        private readonly string _teacherScheduleUrl = "http://www.ulstu.ru/schedule/teachers/";
        private int _currentPosition;
        private readonly WebClient _client;
        private string[] _urls;

        public ExcelDocumentDownloader()
        {
            _urls = GetUrls();
            FilterUrls();
            _client = new WebClient();
        }

        public bool DownloadNextDocument(out FileStream document)
        {
            document = null;
            if (_currentPosition == _urls.Length)
                return false;

            string serverPath = HttpContext.Current.Server.MapPath("") + "+";
            var temp = serverPath.Split('\\').Last();
            serverPath = serverPath.Replace(temp, "");
            string fileName;    

            // if file busy by another process, change file name, and try again
            string name = _currentPosition.ToString();
            while (true)
            {
                fileName = serverPath + name + ".xls";
                try {
                    _client.DownloadFile(_teacherScheduleUrl + _urls[_currentPosition], fileName);
                    break;
                }
                catch (Exception ex) {
                    name += _currentPosition;
                }
            }
            document = File.Open(fileName, FileMode.Open, FileAccess.Read);

            _currentPosition++;
            return true;
        }

        private string[] GetUrls()
        {
            var client = new WebClient();
            string html = client.DownloadString(_teacherScheduleUrl);

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
            _urls = _urls.Where(u => u.EndsWith(".xls") || u.EndsWith(".xlsx")).ToArray();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}