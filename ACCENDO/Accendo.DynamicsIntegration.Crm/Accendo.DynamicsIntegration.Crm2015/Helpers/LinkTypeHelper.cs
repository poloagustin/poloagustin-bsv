using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class LinkTypeHelper
    {
        public static string GetValue(LinkType linkType)
        {
            switch (linkType)
            {
                case LinkType.Inner:
                    return "inner";
                case LinkType.Outer:
                    return "outer";
                default:
                    return string.Empty;
            }
        }
    }
}
