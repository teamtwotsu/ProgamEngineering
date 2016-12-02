using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.DataServices;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        LocalizationService ls = new LocalizationService();

        // GET: Admin
        public ActionResult Localizations()
        {
            List<localization> localizations = ls.GetLocalizations();
            ViewData["result"] = TempData["result"];
            return View(localizations);
        }

        public ActionResult DeleteLocalization(int id)
        {

            TempData["result"] = "";
            using (var db = new DBEntityDataContext())
            {

                var loca = db.localizations.FirstOrDefault(p => p.ID == id);
                if (loca != null)
                {
                    try {
                        db.localizations.DeleteOnSubmit(loca);
                        db.SubmitChanges();
                        TempData["result"] = ls.Get("SavedSuccessfully");// "Успешно удалили";
                    }
                    catch (Exception ex)
                    {
                        TempData["result"] = ls.Get("Error") + ". " + ex.Message; //"Ошибка удаления: " + ex.Message;
                    }

                }

            }

            return RedirectToAction("Localizations");
        }

        public ActionResult EditLocalization(int id)
        {
            localization loca = new localization();
            ViewData["result"] = TempData["result"];
            using (var db = new DBEntityDataContext())
            {

                if (id != 0)
                    loca = db.localizations.FirstOrDefault(p => p.ID == id);
            }

            return View(loca);
        }

        [HttpPost]
        public ActionResult EditLocalization(int id, FormCollection formValues)
        {

            TempData["result"] = "";

            localization loca = new localization();
            

            using (var db = new DBEntityDataContext())
            {

                if (id != 0)
                    loca = db.localizations.FirstOrDefault(p => p.ID == id);

                UpdateModel(loca);

                if (id == 0)
                    db.localizations.InsertOnSubmit(loca);

                try { db.SubmitChanges();
                    TempData["result"] = ls.Get("SavedSuccessfully");// "Сохранили";
                } catch (Exception ex) { TempData["result"] = "Error: " +  ex.Message; }


            }

            return RedirectToAction("EditLocalization", new { id = loca.ID });
        }
    }
}