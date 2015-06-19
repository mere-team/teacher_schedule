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
        private FileStream[] docs;

        public ExcelDocumentDownloader() { }

        public FileStream[] DownloadDocuments()
        {
            var urls = GetUrls();
            urls = GetFiltredUrls(urls);
            docs = new FileStream[urls.Length];
            var client = new WebClient();

            string path_prefix = HttpContext.Current.Server.MapPath(@"Content\Documents\");
            for (int i = 0; i < docs.Length; i++)
            {
                string file_name = path_prefix + i.ToString() + ".xls";
                client.DownloadFile(_TeacherScheduleUrl + urls[i], file_name);
                docs[i] = File.Open(file_name, FileMode.Open, FileAccess.Read);
            }

            return docs;
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

        private string[] GetFiltredUrls(string[] urls)
        {
            var filtredUrls = urls.Where(u => u.EndsWith(".xls") || u.EndsWith(".xlsx"));
            return filtredUrls.ToArray();
        }

        public void Dispose()
        {
            foreach (FileStream doc in docs)
            {
                doc?.Dispose();
                if (File.Exists(doc.Name))
                    File.Delete(doc.Name);
            }
        }
    }
}