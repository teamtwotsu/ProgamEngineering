using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class ResultModel
    {
        public int ID { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string messageDetails { get; set; }

        // ctor
        public ResultModel()
        {
            success = true;
        }
    }
}
