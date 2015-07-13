using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class CrmLinkEntityXml
    {
        /// <summary>
        /// Destination Entity Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Source Entity Attribute Name
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// Destination Entity Attribute Name
        /// </summary>
        public string From { get; set; }
        public string Alias { get; set; }
        public LinkType LinkType { get; set; }
        public bool Visible { get; set; }
        public bool Intersect { get; set; }
        public List<CrmFilterXml> Filters { get; set; }
        public List<CrmLinkEntityXml> LinkEntities { get; set; }
        public List<CrmOrderByXml> OrderBys { get; set; }
        public List<CrmAttributeXml> Attributes { get; set; }
        public bool AllAttributes { get; set; }
        public CrmLinkEntityXml(
            string sourceAttributeName,
            string destinationAttributeName,
            string destinationEntityName,
            string alias = "",
            bool includeFields = true,
            List<CrmAttributeXml> attributes = null,
            List<CrmLinkEntityXml> linkedEntities = null,
            List<CrmFilterXml> filters = null,
            List<CrmOrderByXml> orderBys = null,
            LinkType linkType = LinkType.Inner)
        {
            this.To = sourceAttributeName;
            this.From = destinationAttributeName;
            this.Name = destinationEntityName;
            this.Alias = string.IsNullOrEmpty(alias) ? destinationEntityName : alias;
            //this.includeFields = includeFields;
            this.Attributes = attributes ?? new List<CrmAttributeXml>();
            this.LinkEntities = linkedEntities ?? new List<CrmLinkEntityXml>();
            this.Filters = filters ?? new List<CrmFilterXml>();
            this.OrderBys = orderBys ?? new List<CrmOrderByXml>();
            this.LinkType = linkType;
            this.AllAttributes = false;
        }
    }
}
