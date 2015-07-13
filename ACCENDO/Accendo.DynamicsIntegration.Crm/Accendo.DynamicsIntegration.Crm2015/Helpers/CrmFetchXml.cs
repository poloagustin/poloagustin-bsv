using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class CrmFetchXml
    {
        public List<CrmFilterXml> Filters { get; set; }
        public List<CrmLinkEntityXml> LinkEntities { get; set; }
        public List<CrmOrderByXml> OrderBys { get; set; }
        public List<CrmAttributeXml> Attributes { get; set; }
        public string EntityName { get; set; }
        public string PagingCookie { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public bool Aggregate { get; set; }

        private bool needCount;
        private string idAttributeName;
        public bool Distinct;

        public CrmFetchXml(string entityName, string idAttributeName = "", bool needCount = false)
        {
            this.EntityName = entityName;
            this.idAttributeName = idAttributeName;
            this.Page = 0;
            this.Count = 0;
            this.needCount = needCount;
            this.PagingCookie = string.Empty;
            this.Attributes = new List<CrmAttributeXml>();
            this.Filters = new List<CrmFilterXml>();
            this.LinkEntities = new List<CrmLinkEntityXml>();
            this.OrderBys = new List<CrmOrderByXml>();
        }

        public string GetFetchXml()
        {
            #region SetHeader
            var fetchAttributes = new List<XAttribute>()
            {
                new XAttribute("distinct", false),
                new XAttribute("mapping", "logical"),
                new XAttribute("output-format", "xml-platform"),
                new XAttribute("version", "1.0")
            };
            if (this.Page != 0)
            {
                fetchAttributes.Add(new XAttribute("page", this.Page.ToString()));
            }
            if (this.Count != 0)
            {
                fetchAttributes.Add(new XAttribute("count", this.Count.ToString()));
            }
            if (this.Aggregate)
            {
                fetchAttributes.Add(new XAttribute("aggregate", true));
            }
            if (!string.IsNullOrEmpty(this.PagingCookie))
            {
                fetchAttributes.Add(new XAttribute("paging-cookie", this.PagingCookie));
            }

            var fetch = new XElement("fetch", fetchAttributes);
            #endregion

            fetch.Add(this.GetEntityXml());
            return fetch.ToString();
        }

        private XElement GetEntityXml()
        {
            var orderBys = FetchXmlHelper.GetOrderByXml(this.OrderBys);
            var linkEntities = FetchXmlHelper.GetLinkEntityXml(this.LinkEntities);
            var filters = FetchXmlHelper.GetFilterXml(this.Filters);
            var attributes = FetchXmlHelper.GetAttributeXml(this.Attributes);

            IEnumerable<XElement> elements = new List<XElement>();
            if (attributes != null)
            {
                elements = elements.Concat(attributes);
            }
            if (linkEntities != null)
            {
                elements = elements.Concat(linkEntities);
            }
            if (filters != null)
            {
                elements = elements.Concat(filters);
            }
            if (orderBys != null)
            {
                elements = elements.Concat(orderBys);
            }

            var entity = new XElement("entity", new XAttribute("name", this.EntityName), elements);
            return entity;

        }

        public void Reset()
        {
            if (this.Attributes != null)
                this.Attributes.Clear();

            if (this.Filters != null)
                this.Filters.Clear();

            if (this.LinkEntities != null)
                this.LinkEntities.Clear();

            if (this.OrderBys != null)
                this.OrderBys.Clear();
            this.Page = 0;
            this.Count = 0;
            this.PagingCookie = string.Empty;
        }

        public void SetPage(int i, int limit)
        {
            this.Page = i;
            this.Count = limit;
        }

        public void SetAttributes(List<CrmAttributeXml> attrs)
        {
            this.Attributes = attrs;
        }
    }
}
