using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class CrmOrderByXml
    {
        public CrmOrderByXml(string p, CrmOrderByType crmOrderByType)
        {
            this.Attribute = p;
            this.Descending = crmOrderByType == CrmOrderByType.Ascending ? false : true;
        }
        public string Attribute { get; set; }
        public bool Descending { get; set; }
    }
}
