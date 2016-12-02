using System;
using System.Web;

namespace WebApp.Extentions
{
    public class cookie
    {
        // Session lost while redirecting from payment site - MVC
        // http://stackoverflow.com/questions/21908920/session-lost-while-redirecting-from-payment-site-mvc?lq=1
        
        // Cookies with and without the Domain Specified (browser inconsistency)
        // http://stackoverflow.com/questions/10751813/cookies-with-and-without-the-domain-specified-browser-inconsistency

        // Chrome losing cookies
        // http://stackoverflow.com/questions/18608362/chrome-losing-cookies

        // сохранить куку
        public static void saveCookie(string CookieName, string CookieValue, int saveDays = 356)
        {
            HttpCookie cookie = new HttpCookie(CookieName, CookieValue);
            //задаем срок хранения файла куки на машине пользователя
            //Текущее время на машине клиента
            DateTime dateTime = DateTime.Now;

            //задаем промежуток времени (часов), который будет храниться файл куки
            TimeSpan span = new TimeSpan(saveDays * 24, 0, 0);

            //используем свойство Expires для задания времени хранения файла куки
            cookie.Expires = dateTime.Add(span);

            //локально не выставляем
            if (!HttpContext.Current.Request.Url.Host.Contains("localhost") 
                && !HttpContext.Current.Request.Url.Host.Contains("127.0.0.1"))
                cookie.Domain = HttpContext.Current.Request.Url.Host;
            
            //Добавляем файл куки
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                //  HttpContext.Current.Response.SetCookie(cookie);
                HttpContext.Current.Response.SetCookie(cookie);
            }
            else
                HttpContext.Current.Response.AppendCookie(cookie);
        }

        // прочитать куку
        public static string readCookie(string CookieName)
        {
            string CookieValue = "";
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                HttpCookie aCookie = HttpContext.Current.Request.Cookies[CookieName];
                CookieValue = HttpContext.Current.Server.HtmlEncode(aCookie.Value);
            }

            return CookieValue;
        }

        
    }
}