using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace WebApp.Extentions
{
    public static class sanitizer
    {
        public static string[] htmlWhiteList = {
                                            "a", "u", "b", "strong", "i", "br", "hr ", "br", "h1", "h2", "h3", "h4", "h5", "h6", "span",
                                            "div", "blockquote", "em", "sub", "sup", "s", "font", "ul", "li", "ol", "p", "#text",
                                            "table", "tbody", "tr", "td", "th", "dl", "dt", "dd",  "img" /* h-mmm */, "fieldset", "legend", "iframe"
                                        };

        public static string[] htmlAttributeWhiteList = { "class", "style", "src", "href", "id", "color", "size", "width", "cellspacing", "cellpadding", "border" };

        public enum sanitizeMode
        {
            html,
            email,
            text
        }

        /// <summary>
        /// удаление всех html тегов
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string StripHtmlTags(string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";

            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            return HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);
        }

        /// <summary>
        /// удаление двойных пробелов
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string StripDoubleSpacesAndTrim(string html)
        {
            return string.IsNullOrEmpty(html)
                ? string.Empty
                : Regex.Replace(html, @"\s+", " ").Trim();
        }

        /// <summary>
        /// удаление двойных \r\n
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string StripDoubleRN(string html)
        {
            return string.IsNullOrEmpty(html)
                ? string.Empty
                : Regex
                    .Replace(html, @"(\\r\\n )+", "\r\n ")
                    .Replace("\r\n \r\n", "\r\n")
                    .TrimEnd('\r', '\n')
                    .TrimStart('\r', '\n');
        }

        // санитаризация (чистка) HTML
        public static string SanitizeHtml
            (string html, sanitizeMode sanitizeMode = sanitizeMode.html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            html = html ?? "";

            string[] elementWhitelist = htmlWhiteList;
            string[] attributeWhiteList = htmlAttributeWhiteList;

            if (sanitizeMode == sanitizeMode.email)
            {
                elementWhitelist = new string[] {
                       "#text", "a", "u", "b", "strong", "i", "hr ", "br", "span", "p"};

                attributeWhiteList = new string[] { "src", "href", "color" };
            }
            else if (sanitizeMode == sanitizeMode.text)
            {
                elementWhitelist = new string[] { "#text" };
                attributeWhiteList = new string[] { };
            }

            IList<HtmlNode> hnc = doc.DocumentNode.Descendants().ToList();



            //remove non-white list nodes
            for (int i = hnc.Count - 1; i >= 0; i--)
            {
                HtmlNode htmlNode = hnc[i];
                if (!elementWhitelist.Contains(htmlNode.Name.ToLower()))
                {
                    htmlNode.Remove();
                    continue;
                }

                for (int att = htmlNode.Attributes.Count - 1; att >= 0; att--)
                {
                    HtmlAttribute attribute = htmlNode.Attributes[att];
                    //remove any attribute that is not in the white list (such as event handlers)
                    if (!attributeWhiteList.Contains(attribute.Name.ToLower()))
                    {
                        attribute.Remove();
                    }

                    //strip any "style" attributes that contain the word "expression"
                    if (attribute.Value.ToLower().Contains("expression") && attribute.Name.ToLower() == "style")
                    {
                        attribute.Value = string.Empty;
                    }


                    if (attribute.Name.ToLower() == "src" || attribute.Name.ToLower() == "href")
                    {
                        //strip if the link starts with anything other than http (such as jscript, javascript, vbscript, mailto, ftp, etc...)
                        // или локальные ресурсы
                        // [add] или якорь
                        if (!(attribute.Value.StartsWith("http") || attribute.Value.StartsWith("#")
                            || attribute.Value.StartsWith("/"))) attribute.Value = "#";
                    }
                }
            }
            return doc.DocumentNode.WriteTo();
        }

        public static string ConvertHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            StringWriter sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
        }

        public static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            outText.Write("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }
        }


        private static void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }
    }
}
