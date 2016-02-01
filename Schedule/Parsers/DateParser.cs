using HtmlAgilityPack;

namespace Schedule.Parsers
{
    public class DateParser
    {
        private readonly string _examsDatesUrl = "http://www.ulstu.ru/main/view/article/12443";

        private void GetDateExam()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(_examsDatesUrl);
            var noAltElements = doc.DocumentNode.SelectNodes("//li/a");
            if (noAltElements != null)
            {
                foreach (var text in noAltElements)
                {
                    //DEI.Name = text.InnerText.Trim();
                    //DEI.date = text.Attributes["title"].Value;
                }
            }
        }
    }
}