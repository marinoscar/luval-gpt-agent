using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Luval.GPT.MeetingNotes
{
    public class HtmlWriter : IDisposable
    {
        private FileInfo _file;
        private HtmlDocument _document;
        private HtmlNode _body;

        public HtmlWriter(FileInfo file, string pageTitle)
        {
            _file = file;
            _document = new HtmlDocument();
            _document.Load(InitialHtml(pageTitle));
            _body = _document.DocumentNode.SelectSingleNode("body");

        }


        private static string InitialHtml(string pageTitle = "Report")
        {
            return @"
<!doctype html>
<html lang=""en"">
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <title>{title}</title>
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM"" crossorigin=""anonymous"">
    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"" integrity=""sha384-geWF76RCwLtnZ8qwWowPQNguL3RmwHVBC9FhGdlKrxdiJJigb/j/68SIy3Te4Bkz"" crossorigin=""anonymous""></script>
  </head>
  <body>
  </body>
</html>
".Replace("{title}", pageTitle);
        }

        public void AddParragraph(string text)
        {
            var node = HtmlNode.CreateNode("<p></p>");
            node.InnerHtml = text;
            _body.AppendChild(node);
        }

        public void AddHeading(string text, int num = 1)
        {
            var node = HtmlNode.CreateNode($"<h{num}></h{num}>");
            node.InnerHtml = HttpUtility.HtmlEncode(text);
            _body.AppendChild(node);
        }

        public void AddUnOrderedList(IEnumerable<string> bullets)
        {
            var node = HtmlNode.CreateNode("<ul></ul>");
            foreach (var item in bullets)
            {
                var li = HtmlNode.CreateNode("<li></li>");
                li.InnerHtml = HttpUtility.HtmlEncode(item);
                node.AppendChild(li);
            }
            _body.AppendChild(node);
        }

        public void Save()
        {
            _document.Save(_file.FullName);
        }


        public void Dispose()
        {
            _file = null;
            _document = null;
        }
    }
}
