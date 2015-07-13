using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class CrmFilterXml
    {
        public List<CrmConditionXml> Conditions { get; set; }

        public List<CrmFilterXml> Filters { get; set; }

        public CrmFilterTypeXml Type { get; set; }

        public CrmFilterXml(CrmFilterTypeXml type, List<CrmConditionXml> conditions, List<CrmFilterXml> filters)
        {
            this.Conditions = conditions;
            this.Type = type;
            this.Filters = filters;
        }

        public CrmFilterXml(CrmFilterTypeXml type, List<CrmConditionXml> conditions)
        {
            this.Type = type;
            this.Conditions = conditions;
            this.Filters = new List<CrmFilterXml>();
        }

        public CrmFilterXml(CrmFilterTypeXml type, CrmConditionXml condition)
        {
            this.Type = type;
            this.Conditions = new List<CrmConditionXml>() { condition };
            //this.Conditions.ToList().Add(condition);
            this.Filters = new List<CrmFilterXml>();
        }

        public CrmFilterXml()
        {

        }

        public void AddCondition(CrmConditionXml condition)
        {
            this.Conditions.Add(condition);
        }

        public XElement GetFilter()
        {
            var filtro = new XElement("filter", new XAttribute("type", this.GetFilterTypeAsString()));

            //foreach (var item in this.Conditions)
            //{
            //    filtro.Add(item.GetCondition());  
            //}

            return filtro;
        }

        private string GetFilterTypeAsString()
        {
            switch (this.Type)
            {
                case CrmFilterTypeXml.And:
                    return "and";
                case CrmFilterTypeXml.Or:
                    return "or";
                default:
                    return "and";
            }
        }

        public void AddFilter(CrmFilterXml filter)
        {
            this.Filters.Add(filter);
        }
    }
}
