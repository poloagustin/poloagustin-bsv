using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class DateGroupingTypeHelper
    {
        public static string GetValue(DateGroupingType dateGroupingType)
        {
            switch (dateGroupingType)
            {
                case DateGroupingType.Day:
                    return "day";
                case DateGroupingType.Week:
                    return "week";
                case DateGroupingType.Month:
                    return "month";
                case DateGroupingType.Quarter:
                    return "quarter";
                case DateGroupingType.Year:
                    return "year";
                case DateGroupingType.FiscalPeriod:
                    return "fiscal-period";
                case DateGroupingType.FiscalYear:
                    return "fiscal-year";
                default:
                    return string.Empty;
            }
        }
    }
}
