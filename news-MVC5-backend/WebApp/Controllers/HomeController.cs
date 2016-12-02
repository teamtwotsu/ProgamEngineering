using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApp.DataServices;
using WebApp.Models;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    
        public class HomeController : Controller
        {

            public HomeController()
            {
            }

        public ActionResult ChangeCulture(string lang, string returnUrl)
        {


            Session["_Culture"] = new CultureInfo(lang);

            return Redirect(returnUrl);
        }


            // список страниц
            public ActionResult Index(int? id)
            {
                
                PageServices pageServices = new PageServices();
                Models.ViewModels.IndexViewModel VM = new Models.ViewModels.IndexViewModel();
                VM.selectedCat_ID = id;
                VM.webRoot = Server.MapPath("~/");// _env.WebRootPath;

                VM.pageCategories = pageServices.GetPageCategories().ToList();
                VM.pages = pageServices.GetPages(id);

                return View(VM);
            }

            // список страниц
            public ActionResult Page(int id)
            {

                PageServices pageServices = new PageServices();
                PageViewModel VM = new PageViewModel();
                VM.page = pageServices.GetPage(id);
                VM.webRoot = Server.MapPath("~/");
            //VM.pageCat = pageServices.GetPageCategory(VM.page.PagCategoryId); ;


            return View(VM);
            }

            [Authorize]
            public ActionResult EditPage(int id)
            {
                PageServices pageServices = new PageServices();
                EditPageViewModel VM = new EditPageViewModel();
                VM.webRoot = Server.MapPath("~/");
            VM.page = pageServices.GetPage(id);
                if (VM.page == null)
                    VM.page = new page();

                ViewData["result"] = TempData["result"];
                return View(VM);
            }

            [Authorize]
            [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPage(
                FormCollection formValues,
                string add, string save, string delete)
            {
                int page_ID = Int32.Parse(formValues["ID"]);
                int category_ID = Int32.Parse(formValues["category_ID"]);
                bool isTop = formValues["isTop"] == "true";

                PageServices pageServices = new PageServices();
                string result = "";

                if (!String.IsNullOrEmpty(add))
                {
                    return RedirectToAction("EditPage", new { id = 0 });
                }
                else if (!String.IsNullOrEmpty(delete))
                {
                    ResultModel rm = pageServices.DeletePage(page_ID);
                    result = rm.message;

                    TempData["result"] = result;
                    return RedirectToAction("EditPage", new { id = 0 });
                }
                else
                {


                    ResultModel rm = pageServices.SavePage(page_ID,
                        formValues["header_ru"],
                        formValues["header_en"],
                        formValues["html_ru"],
                        formValues["html_en"],
                        category_ID, isTop);

                    result = rm.message;

                    string path = System.IO.Path.Combine(Server.MapPath("~/"), "Content\\Images\\QR");
                    if (!System.IO.File.Exists(System.IO.Path.Combine(path, rm.ID + ".png")))
                    {
                        #region QR-code

                        try
                        {
                            string URL = "http://" + HttpContext.Request.Url.Authority.ToString().TrimEnd('/')
                            + "/home/page/" + rm.ID;

                            var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chl={0}",
                                URL, 200, 200);
                            WebResponse response = default(WebResponse);
                            Stream remoteStream = default(Stream);
                            StreamReader readStream = default(StreamReader);
                            WebRequest request = WebRequest.Create(url);
                            response = request.GetResponse();
                            remoteStream = response.GetResponseStream();
                            readStream = new StreamReader(remoteStream);
                            System.Drawing.Image img = System.Drawing.Image.FromStream(remoteStream);

                            var uploads = Path.Combine(Server.MapPath("~/"), "Content\\Images\\QR");
                            img.Save(Path.Combine(uploads, rm.ID + ".png"));
                            response.Close();
                            remoteStream.Close();
                            readStream.Close();
                        }
                        catch (Exception ex)
                        {
                        }

                        #endregion
                    }
                    if (rm.ID != 0)
                    {
                        var uploads = Path.Combine(Server.MapPath("~/"), "Content\\Images\\PageImages");
                        foreach (string upload in Request.Files)
                        {
                        var file = Request.Files[upload];
                        if (file.ContentLength > 0)
                            {
                                Request.Files[upload].SaveAs(Path.Combine(uploads, file.FileName));

                                ResultModel rm1 = pageServices.SavePageImage(rm.ID, file.FileName);
                                    result += " | " + rm.message;
                            }
                        }
                    }

                    TempData["result"] = result;
                    return RedirectToAction("EditPage", new { id = rm.ID });
                }

            }

            public ActionResult About()
            {
                ViewData["Message"] = "Your application description page.";

                return View();
            }

            public ActionResult Contact()
            {
                ViewData["Message"] = "Your contact page.";

                return View();
            }

            public ActionResult Error()
            {
                return View();
            }
        }
}
