using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class CrmAttributeXml
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public AggregateType Aggregate { get; set; }
        public bool GroupBy { get; set; }
        public DateGroupingType DateGrouping { get; set; }
        public CrmAttributeXml(
            string name = "",
            string alias = "",
            AggregateType aggregate = AggregateType.None,
            bool groupBy = false,
            DateGroupingType dateGrouping = DateGroupingType.None
            )
        {
            this.Name = name;
            this.Alias = alias;
            this.Aggregate = aggregate;
            this.GroupBy = groupBy;
            this.DateGrouping = dateGrouping;
        }
    }
}
