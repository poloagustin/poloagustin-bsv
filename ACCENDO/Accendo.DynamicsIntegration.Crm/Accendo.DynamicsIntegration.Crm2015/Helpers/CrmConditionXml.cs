using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class CrmConditionXml
    {
        public IEnumerable<string> Values { get; set; }
        public string Value { get; set; }
        public CrmConditionTypeXml Operator { get; set; }
        public string Attribute { get; set; }
        private static string createdByAttribute = "createdby";

        public CrmConditionXml(string attribute, CrmConditionTypeXml type, string value)
        {
            this.Attribute = attribute;
            this.Operator = type;
            this.Value = value;
            this.Values = new List<string>();
        }

        public CrmConditionXml(string attribute, CrmConditionTypeXml type, object value)
        {
            this.Attribute = attribute;
            this.Operator = type;
            this.Value = value.ToString();
            this.Values = new List<string>();
        }

        public CrmConditionXml(string attribute, CrmConditionTypeXml type, IEnumerable<object> values)
        {
            this.Attribute = attribute;
            this.Operator = type;
            this.Value = string.Empty;
            this.Values = values.Select(x => x.ToString());
        }

        public CrmConditionXml(string attribute, CrmConditionTypeXml type, IEnumerable<string> values)
        {
            this.Attribute = attribute;
            this.Operator = type;
            this.Value = string.Empty;
            this.Values = values;
        }

        public static CrmConditionXml CreatedByCurrentUser
        {
            get
            {
                var cond = new CrmConditionXml(createdByAttribute, CrmConditionTypeXml.EqUserid, string.Empty);
                return cond;
            }
        }
    }
}
