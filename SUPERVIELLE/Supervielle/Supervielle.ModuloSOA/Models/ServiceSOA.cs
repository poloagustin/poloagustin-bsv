using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModuloSoa.Models
{
    public class ServiceSOA
    {
        public string nombre { get; set; }
        public string url { get; set; }
        public string action { get; set; }
        public string page { get; set; }

    }

    public class Token
    {
        public string user { get; set; }
        public string pass { get; set; }
        public string encoding { get; set; }
        public string created { get; set; }
    }
}