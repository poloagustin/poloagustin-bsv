using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModuloSoa.Dto
{
    public class contact
    {
        public Guid id { get; set; }

        public object bsv_no_de_documento { get; set; }
        public object bsv_tipo_de_documento { get; set; }
        public string bsv_cuil { get; set; }

    }
}