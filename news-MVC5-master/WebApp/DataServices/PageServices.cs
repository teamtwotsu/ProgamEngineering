
using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using WebApp.Models;
//using EFGetStarted.AspNetCore.ExistingDb.Models;
//using Microsoft.Extensions.Configuration;

namespace WebApp.DataServices
{
    public class PageServices
    {
        //https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext
        //public ApplicationDbContext context { get; set; }

        public List<pageCategory> GetPageCategories()
        {
            var context = new DBEntityDataContext();
            return context.pageCategories.ToList();
        }

        public List<page> GetPages(int? id)
        {
            var context = new DBEntityDataContext();
            if (id == null)
                return context.pages.OrderByDescending(p => p.isTop).ToList();
            else 
                return context.pages.Where(p => p.pagCategory_ID == (int)id).OrderByDescending(p => p.isTop).ToList();
        }

        public page GetPage(int id)
        {
            var context = new DBEntityDataContext();
            return context.pages.FirstOrDefault(p => p.ID == id);
        }

        // ctor
        public PageServices()
        {
            //var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            //string conn = ConfigurationManager.AppSettings["conn"];
            //optionsBuilder.UseSqlServer(conn);

            //context = new ApplicationDbContext(optionsBuilder.Options);
        }

        public ResultModel SavePage(
            int ID, 
            string header, 
            string html, 
            int category_ID,
            bool isTop)
        {
            ResultModel rm = new ResultModel();

            var context = new DBEntityDataContext();
            page page = new page();
            if (ID != 0)
                page = context.pages.FirstOrDefault(p => p.ID == ID);

            page.header = header;
            page.html = html;
            page.pagCategory_ID = category_ID;
            page.isTop = isTop;
            page.created = DateTime.Now;

            if (ID == 0)
                context.pages.InsertOnSubmit(page);

            try {
                context.SubmitChanges();
                rm.ID = page.ID;
                rm.message = "Сохранили страницу";
            }
            catch (Exception ex)
            {
                rm.message = "Ошибка сохранения страницы: " + ex.Message;
            }

            return rm;
        }

        public ResultModel DeletePage(int ID)
        {
            ResultModel rm = new ResultModel();
            var context = new DBEntityDataContext();
            page page = context.pages.FirstOrDefault(p => p.ID == ID);

            try
            {
                context.pages.DeleteOnSubmit(page);
                context.SubmitChanges();
                rm.ID = page.ID;
                rm.message = "Удалили страницу";
            }
            catch (Exception ex)
            {
                rm.message = "Ошибка удаления страницы: " + ex.Message;
            }

            return rm;
        }

        public ResultModel SavePageImage(
            int ID,
            string image)
        {
            ResultModel rm = new ResultModel();
            var context = new DBEntityDataContext();
            page page = context.pages.FirstOrDefault(p => p.ID == ID);

            page.image = image;

            try
            {
                context.SubmitChanges();
                rm.ID = page.ID;
                rm.message = "Сохранили картинку";
            }
            catch (Exception ex)
            {
                rm.message = "Ошибка сохранения картинки: " + ex.Message;
            }

            return rm;
        }
    }
}
