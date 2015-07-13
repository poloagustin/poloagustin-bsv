using Accendo.DynamicsIntegration.Crm2015.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public class FetchXmlHelper
    {
        public static IEnumerable<XElement> GetFilterXml(IEnumerable<CrmFilterXml> sourceFilters)
        {
            var filters = new List<XElement>();

            foreach (var filter in sourceFilters)
            {
                var attributes = new List<XAttribute>()
                {
                    new XAttribute("type", CrmFilterXmlHelper.GetValue(filter.Type))
                };
                var elements = GetFilterXml(filter.Filters).Concat(GetConditionXml(filter.Conditions));

                filters.Add(new XElement("filter", attributes, elements));
            }
            return filters;
        }

        public static IEnumerable<XElement> GetConditionXml(IEnumerable<CrmConditionXml> sourceConditions)
        {
            var conditions = new List<XElement>();
            foreach (var condition in sourceConditions)
            {
                var attributes = new List<XAttribute>()
                    {
                        new XAttribute("attribute", condition.Attribute),
                        new XAttribute("operator", CrmConditionXmlHelper.GetValue(condition.Operator)),
                    };
                var elements = new List<XElement>();

                if (condition.Operator != CrmConditionTypeXml.IsNull &&
                    condition.Operator != CrmConditionTypeXml.NotNull)
                {
                    if (condition.Operator == CrmConditionTypeXml.In ||
                        condition.Operator == CrmConditionTypeXml.NotIn)
                    {
                        elements = condition.Values.Select(x => new XElement("value", x)).ToList();
                    }
                    else
                    {
                        attributes.Add(new XAttribute("value", condition.Value));
                    }
                }

                conditions.Add(new XElement("condition", attributes, elements));
            }
            return conditions;
        }

        public static IEnumerable<XElement> GetLinkEntityXml(IEnumerable<CrmLinkEntityXml> sourceLinkEntities)
        {
            List<XElement> linkEntities = new List<XElement>();

            foreach (var le in sourceLinkEntities)
            {
                var attrAttributes = new List<XAttribute>()
                {
                    new XAttribute("name", le.Name),
                    new XAttribute("to", le.To),
                    new XAttribute("from", le.From),
                    new XAttribute("alias", le.Alias),
                    new XAttribute("link-type", LinkTypeHelper.GetValue(le.LinkType)),
                };
                var linkEntity = new XElement(
                    "link-entity",
                    attrAttributes,
                    GetElements(le.Attributes, le.LinkEntities, le.Filters, le.OrderBys)
                );
                linkEntities.Add(linkEntity);
            }
            return linkEntities;
        }

        public static IEnumerable<XElement> GetOrderByXml(IEnumerable<CrmOrderByXml> sourceOrderBys)
        {
            List<XElement> orderBys = new List<XElement>();

            foreach (var orderBy in sourceOrderBys)
            {
                var attributes = new List<XAttribute>()
                {
                    new XAttribute("attribute", orderBy.Attribute),
                    new XAttribute("descending", orderBy.Descending),
                };

                orderBys.Add(new XElement("order", attributes));
            }

            return orderBys;
        }

        public static IEnumerable<XElement> GetAttributeXml(IEnumerable<CrmAttributeXml> sourceAttributes)
        {
            List<XElement> attributes = new List<XElement>();
            var containsAggregation = sourceAttributes.Any(attr => attr.Aggregate != AggregateType.None || attr.GroupBy);

            foreach (var attr in sourceAttributes)
            {
                var attrAttributes = new List<XAttribute>()
                {
                    new XAttribute("name", attr.Name),
                };

                if (!string.IsNullOrEmpty(attr.Alias))
                {
                    attrAttributes.Add(new XAttribute("alias", string.IsNullOrEmpty(attr.Alias) ? attr.Name : attr.Alias));

                }

                if (containsAggregation)
                {
                    if (attr.Aggregate != AggregateType.None)
                    {
                        attrAttributes.Add(new XAttribute("aggregate", AggregateTypeHelper.GetValue(attr.Aggregate)));
                    }
                    else
                    {
                        attrAttributes.Add(new XAttribute("groupby", true.ToString()));
                    }
                }
                if (attr.DateGrouping != DateGroupingType.None)
                {
                    attrAttributes.Add(new XAttribute("dategrouping", DateGroupingTypeHelper.GetValue(attr.DateGrouping)));
                }
                attributes.Add(new XElement("attribute", attrAttributes));
            }

            return attributes;
        }

        public static IEnumerable<XElement> GetElements(
            IEnumerable<CrmAttributeXml> attributes,
            IEnumerable<CrmLinkEntityXml> linkEntities,
            IEnumerable<CrmFilterXml> filters,
            IEnumerable<CrmOrderByXml> orderBys)
        {
            var orderBysXml = GetOrderByXml(orderBys ?? new List<CrmOrderByXml>());
            var linkEntitiesXml = GetLinkEntityXml(linkEntities ?? new List<CrmLinkEntityXml>());
            var filtersXml = FetchXmlHelper.GetFilterXml(filters ?? new List<CrmFilterXml>());
            var attributesXml = FetchXmlHelper.GetAttributeXml(attributes ?? new List<CrmAttributeXml>());

            IEnumerable<XElement> elements = new List<XElement>();
            if (attributes != null)
            {
                elements = elements.Concat(attributesXml);
            }
            if (linkEntities != null)
            {
                elements = elements.Concat(linkEntitiesXml);
            }
            if (filters != null)
            {
                elements = elements.Concat(filtersXml);
            }
            if (orderBys != null)
            {
                elements = elements.Concat(orderBysXml);
            }

            return elements;
        }
    }
}
