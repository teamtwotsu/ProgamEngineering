using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace WebApp.Extentions
{
    public class stringUtils
    {
        public static string RodPadezh(int i, string pyaty, string odnogo)
        {
            string result = pyaty;
            if (i > 0)
            {
                if (i.ToString().Length > 1
                    &&
                    i.ToString().Substring(i.ToString().Length - 2) == "11")
                { result = pyaty; }
                else if (i.ToString().Last() == '1')
                { result = odnogo; }
            }

            return result;
        }

        // получить наклонение слова
        // один товар, два товара, пять товаров
        public static string getWordsQtyCase(int qty, string odin, string dva, string pyat)
        {
            qty = qty % 100;
            if (qty > 9 && qty < 20)
                return pyat;

            qty = qty % 10;

            if (qty == 0)
                return pyat;
            else if (qty == 1)
                return odin;
            else if (qty > 1 && qty < 5)
                return dva;
            else if (qty > 4 && qty < 10)
                return pyat;
            else
                return "";
        }

        // капитализировать первую букву
        public static string capitalizeFirstLettersInPhraze(string phraze)
        {
            string resString = "";

            string[] phrazeParts = phraze.Split();
            foreach (var phrazePart in phrazeParts)
            {
                string tempPhrazePart = phrazePart.Trim();
                if (tempPhrazePart.Length > 0)
                {
                    tempPhrazePart = tempPhrazePart.Substring(0, 1).ToUpper() + tempPhrazePart.Substring(1).ToLower();
                    resString += (tempPhrazePart + " ");
                }
            }


            return resString.Trim();
        }

        // заменить символы, не являющиеся XML
        public static string replaceNonXML_symbols(string inputString)
        {
            string exportString = inputString
                .Replace(@"""", "&quot;")
                .Replace(@"&", "&amp;")
                .Replace(@">", "&gt;")
                .Replace(@"<", "&lt;")
                .Replace(@"'", "&apos;");

            return exportString;
        }

        // получить параметр из строки - GET / POST 
        public static string get_GetPostParameter(string sName)
        {
            string sValue = HttpContext.Current.Request.Form[sName];
            if (string.IsNullOrEmpty(sValue))
                sValue = HttpContext.Current.Request.QueryString[sName];

            if (string.IsNullOrEmpty(sValue))
                sValue = String.Empty;

            return sValue;

        }

        // получить строку из коллекции
        public static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }

        // сгенерировать пароль
        public static string GeneratePassword(int passswordLength = 8)
        {
            string pass = "";
            var r = new Random();
            while (pass.Length < passswordLength)
            {
                Char c = (char)r.Next(33, 125);
                if (Char.IsLetterOrDigit(c))
                    pass += c;
            }
            return pass;
        }

        public static void saveValueToWebConfig(string key, string value)
        {
            string strAppPath = HostingEnvironment.ApplicationVirtualPath;
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration(strAppPath);

            var keyValueConfigurationElement = configuration.AppSettings.Settings[key];

            if (keyValueConfigurationElement == null)
                configuration.AppSettings.Settings.Add(key, value);
            else
                keyValueConfigurationElement.Value = value;

            configuration.Save(ConfigurationSaveMode.Full);
        }

        // получить только цифры из строки
        public static string getOnlyNumbers(string inputString)
        {
            List<char> charList = (inputString ?? "").ToCharArray().ToList();
            string outputString = "";
            foreach (var charP in charList)
            {
                if (charP == '1' ||
                    charP == '2' ||
                    charP == '3' ||
                    charP == '4' ||
                    charP == '5' ||
                    charP == '6' ||
                    charP == '7' ||
                    charP == '8' ||
                    charP == '9' ||
                    charP == '0') outputString += charP;
            }

            return outputString;
        }

        // получить HEX строку 
        public static string getHexString(byte[] byteHash)
        {
            string hash = "";

            //формируем одну цельную строку из массива  
            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);

            return hash;
        }

        

        // обрезать строку - вернуть начало
        public static string truncate(string input, int length, string postfix)
        {
            if (input == null)
                input = "";

            if (input.Length <= length)
            {
                return input;
            }
            else
            {
                return input.Substring(0, length) + postfix;
            }
        }

    }
}
