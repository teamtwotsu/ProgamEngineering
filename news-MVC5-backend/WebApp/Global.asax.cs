using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // здесь устанавливаем язык!
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {

            //Очень важно проверять готовность объекта сессии
            if (HttpContext.Current.Session != null)
            {

                CultureInfo ci = null;
                //store.Models.cultureClass.getCultureString();
                try { ci = (CultureInfo)this.Session["_Culture"]; } catch { }

                //Вначале проверяем, что в сессии нет значения
                //и устанавливаем значение по умолчанию
                //это происходит при первом запросе пользователя
                if (ci == null)
                {
                    //Устанавливает значение по умолчанию - базовый русский
                    string langName = "ru"; // en

                    //Пытаемся получить значения с HTTP заголовка
                    if (HttpContext.Current.Request.UserLanguages != null && HttpContext.Current.Request.UserLanguages.Length != 0)
                    {
                        //Получаем список 
                        langName = HttpContext.Current.Request.UserLanguages[0].Substring(0, 2);
                    }



                    ci = new CultureInfo(langName);
                    this.Session["_Culture"] = ci;

                }


                //CultureInfo ci = new CultureInfo(store.Models.cultureClass.getCultureString());
                //Устанавливаем культуру для каждого запроса
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            }
        }
    }
}
