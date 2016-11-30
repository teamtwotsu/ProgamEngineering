using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.ViewModels
{
    public class PageViewModel
    {
        public page page { get; set; }
        public pageCategory pageCat { get; set; }
        public string webRoot { get; set; }
    }
}
