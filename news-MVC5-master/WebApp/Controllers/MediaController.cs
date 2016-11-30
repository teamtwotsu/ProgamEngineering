using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    public class MediaController : Controller
    {

        // GET: Media
        public ActionResult Index()
        {
            return View();
        }

        // ckeditor
        public void uploadnow(HttpPostedFileWrapper upload)
        {
            if (upload != null)
            {
                string ImageName = upload.FileName;
                string path = System.IO.Path.Combine(Server.MapPath("~/Data/Images/Uploads"), ImageName);
                upload.SaveAs(path);
            }
        }

        // ckeditor
        public ActionResult uploadPartial()
        {
            var appData = Server.MapPath("~/Data/Images/uploads");
            var images = Directory.GetFiles(appData).Select(x => new CKEditorImagesViewModel
            {
                Url = Url.Content("/Data/images/uploads/" + Path.GetFileName(x))
            });
            return View(images);
        }

       
    }
}