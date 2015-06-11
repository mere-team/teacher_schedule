using Excel;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace TeacherSheduleParser
{
    public class SheduleParser
    {
        private IExcelDataReader _Reader;
        private DataSet _Data;

        private bool _IsNotEnd = true;

        public SheduleParser(FileStream stream)
        {
            string fileType = stream.Name.Split('.').Last();
            if (fileType != "xls" && fileType != "xlsx")
                throw new Exception("File have not correct format");

            if (fileType == "xls")
                _Reader = ExcelReaderFactory.CreateBinaryReader(stream);
            else
                _Reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            _Data = _Reader.AsDataSet();
        }

        public string ReadRow()
        {
            if (!_IsNotEnd)
                return null;

            var row = new StringBuilder();
            for (int i = 0; i < _Reader.FieldCount; i++)
            {
                row.Append(_Reader.GetString(i));
            }

            _IsNotEnd = _Reader.Read();

            return row.ToString();
        }
    }
}
