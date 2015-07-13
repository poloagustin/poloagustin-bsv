using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModuloSoa.Dto
{
    public class bsv_configuracinserviciossoa
    {
        public Guid id { get; set; }
        public string bsv_action {get;set;}
        public string bsv_name {get;set;}
        public string bsv_url {get;set;}
        public string bsv_xml { get; set; }
    }

    public class bsv_recepcionsoa
    {
        public Guid id { get; set; }
        public int bsv_ancho { get; set; }
        public Guid bsv_consultasoa { get; set; }
        public string bsv_label { get; set; }
        public string bsv_name { get; set; }
        public int bsv_orden { get; set; }       
    }

    public class bsv_enviosoa
    {
        public Guid id { get; set; }
        public string bsv_atributo { get; set; }
        public Guid bsv_consultasoa { get; set; }
        public string bsv_entityattribute { get; set; }       
        public string bsv_name { get; set; }
        public Microsoft.Xrm.Sdk.OptionSetValue bsv_tipoatributo { get; set; }

        public string valor { get; set; }
    }


}