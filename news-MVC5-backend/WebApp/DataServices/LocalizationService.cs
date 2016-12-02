using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.DataServices
{
    public class LocalizationService
    {
        public List<localization> GetLocalizations()
        {
            return new DBEntityDataContext().localizations.OrderBy(p => p.key).ToList();
        }

        public string Get(string key)
        {
            string lang = "ru";
            if (HttpContext.Current.Session["_Culture"] != null && ((CultureInfo)HttpContext.Current.Session["_Culture"]).Name == "en")
                lang = "en";

                var loca =  new DBEntityDataContext().localizations.FirstOrDefault(p => p.key == key);

            if (loca != null)
            {
                if (lang == "ru")
                    return loca.ru;
                else
                    return loca.en;

            }

            return key;
        }

        public string GetCurrentLocal()
        {
            string currentLocal = "ru";
            if (HttpContext.Current.Session["_Culture"] != null
                && ((CultureInfo)HttpContext.Current.Session["_Culture"]).Name == "en")
                currentLocal = "en";

            return currentLocal;
        }

    }
}