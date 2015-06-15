using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TeacherScheduleParser;

namespace SheduleServer.Controllers
{
    public class TeacherScheduleController : ApiController
    {
        public JsonResult Get(int id_teacher)
        {
            //download
            //!!Вынести в отдельный метод
            string remoteUri = "http://www.ulstu.ru/schedule/teachers/%a8%ad.%ef%a7..xls";
            string fileName = "foreign_languages.xls";
            var webClient = new WebClient();
            var file = File.Open(fileName, FileMode.Open, FileAccess.Read);
            webClient.DownloadFile(remoteUri, fileName);

            //parser
            ScheduleParser parser = null;
            try
            {
                parser = new ScheduleParser(file);
            }
            catch(NotExcelDocumentFormatException ex)
            {

            }
            catch(Exception ex)
            {

            }

            //Generate json
            JsonResult json = new JsonResult();
            try
            {
                json.Data = parser.GetTeacherShedule();
            }
            catch(Exception ex)
            {

            }

            return json;
        }
    }
}