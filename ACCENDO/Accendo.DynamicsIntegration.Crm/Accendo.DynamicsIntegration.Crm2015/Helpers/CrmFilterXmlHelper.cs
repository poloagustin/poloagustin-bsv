using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class CrmFilterXmlHelper
    {
        public static string GetValue(CrmFilterTypeXml type)
        {
            switch (type)
            {
                case CrmFilterTypeXml.And:
                    return "and";
                case CrmFilterTypeXml.Or:
                    return "or";
                default:
                    return string.Empty;
            }
        }
    }
}
