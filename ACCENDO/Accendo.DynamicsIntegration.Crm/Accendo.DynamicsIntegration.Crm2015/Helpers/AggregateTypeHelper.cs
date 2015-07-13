using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class AggregateTypeHelper
    {
        public static string GetValue(AggregateType aggregateType)
        {
            switch (aggregateType)
            {
                case AggregateType.Count:
                    return "count";
                case AggregateType.CountColumn:
                    return "countcolumn";
                case AggregateType.Sum:
                    return "sum";
                case AggregateType.Avg:
                    return "avg";
                case AggregateType.Min:
                    return "min";
                case AggregateType.Max:
                    return "max";
                default:
                    return string.Empty;
            }
        }
    }
}
