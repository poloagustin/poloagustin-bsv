using Accendo.DynamicsIntegration.Crm2015.CustomAttributes;
using Accendo.DynamicsIntegration.Crm2015.Enums;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Helpers
{
    public static class EntityBuilder
    {
        public static List<Entity> BuildEntityList(IEnumerable objects, Enums.MappingType mappingType, BulkAction bulkAction)
        {
            List<Entity> entities = new List<Entity>();

            if (objects == null)
                return new List<Entity>();

            foreach (var obj in objects)
            {
                entities.Add(BuildEntity(obj, mappingType, bulkAction));
            }

            return entities;
        }

        public static Entity BuildEntity(object obj, Enums.MappingType mappingType, BulkAction bulkAction, EntityMetadata entityMetadata = null)
        {
            if (obj == null)
                return null;

            Entity entity = new Entity();

            var properties = obj.GetType().GetProperties()
                .Where(p =>
                    (
                        p.PropertyType.GetInterface(typeof(IEnumerable).Name) == null ||
                        p.PropertyType == typeof(string)
                    ) &&
                    !p.GetCustomAttributes(typeof(VirtualAttribute), false).Any()
                );

            switch (mappingType)
            {
                case Accendo.DynamicsIntegration.Crm2015.Enums.MappingType.CustomCrmDto:
                    BuildCustomCrmDtoEntity(obj, entity, properties);
                    break;
                case Accendo.DynamicsIntegration.Crm2015.Enums.MappingType.EntityLogicalName:
                    BuildEntityLogicalNameEntity(obj, entity, properties, bulkAction, entityMetadata);
                    break;
                case Accendo.DynamicsIntegration.Crm2015.Enums.MappingType.None:
                default:
                    BuildReflectionEntity(obj, entity, properties);
                    break;
            }

            return entity;
        }

        private static void BuildReflectionEntity(object obj, Entity entity, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
            entity.LogicalName = obj.GetType().Name;

            foreach (var property in properties)
            {
                if (entity.Contains(property.Name))
                {
                    MapCustomCrmDtoEntityProperty(property.GetValue(obj), entity, property.Name);
                }
            }
        }

        private static void BuildEntityLogicalNameEntity(object obj, Entity entity, IEnumerable<System.Reflection.PropertyInfo> properties, BulkAction bulkAction, EntityMetadata entityMetadata = null)
        {
            entity.LogicalName = ((EntityLogicalNameAttribute)obj.GetType().GetCustomAttributes(typeof(EntityLogicalNameAttribute), false).First()).LogicalName;

            var filteredProperties = from p in properties
                                     where p.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).Any()
                                     select p;

            IEnumerable<AttributeMetadata> attributesToRetrieve = new List<AttributeMetadata>();

            if (entityMetadata != null)
            {
                if (bulkAction == BulkAction.Create)
                {
                    attributesToRetrieve = entityMetadata.Attributes.Where(x => x.IsValidForCreate ?? false);
                }
                else if (bulkAction == BulkAction.Update)
                {
                    attributesToRetrieve = entityMetadata.Attributes.Where(x => x.IsValidForUpdate ?? false);
                }
            }

            foreach (var property in filteredProperties)
            {
                var attributeLogicalNameProperty = (AttributeLogicalNameAttribute)property.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).First();
                var attributeMetadata = attributesToRetrieve.FirstOrDefault(x => x.LogicalName == attributeLogicalNameProperty.LogicalName);

                if (!entity.Contains(attributeLogicalNameProperty.LogicalName) && (!attributesToRetrieve.Any() || attributeMetadata != null))
                {
                    MapLogicalNameEntityProperty(property.GetValue(obj), entity, attributeLogicalNameProperty.LogicalName, bulkAction, attributeMetadata);
                }
            }
        }

        private static void MapLogicalNameEntityProperty(object value, Entity entity, string logicalName, BulkAction bulkAction, AttributeMetadata attributeMetadata = null)
        {
            if (value != null)
            {
                entity[logicalName] = value; 
            }
            else if (bulkAction == BulkAction.Update && (attributeMetadata.RequiredLevel.Value == AttributeRequiredLevel.None || attributeMetadata.RequiredLevel.Value == AttributeRequiredLevel.Recommended))
            {
                entity[logicalName] = null;
            }
        }

        private static void BuildCustomCrmDtoEntity(object obj, Entity entity, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
            entity.LogicalName = ((CustomCrmDtoAttribute)obj.GetType().GetCustomAttributes(typeof(CustomCrmDtoAttribute), false).First()).EntityName;

            var filteredProperties = from p in properties
                                     where p.GetCustomAttributes(typeof(CustomCrmDtoPropertyAttribute), false).Any()
                                     select p;

            foreach (var property in filteredProperties)
            {
                var customCrmProperty = (CustomCrmDtoPropertyAttribute)property.GetCustomAttributes(typeof(CustomCrmDtoPropertyAttribute), false).First();

                if (entity.Contains(customCrmProperty.AttributeName))
                {
                    MapCustomCrmDtoEntityProperty(property.GetValue(obj), entity, customCrmProperty.AttributeName);
                }
            }
        }

        private static void MapCustomCrmDtoEntityProperty(object value, Entity entity, string attributeName)
        {
            if (value is Lookup)
            {
                var lookup = (Lookup)value;
                entity[attributeName] = new EntityReference(lookup.LogicalName, lookup.Id);
            }
            else if (value is Picklist)
            {
                var picklist = (Picklist)value;
                entity[attributeName] = new OptionSetValue(picklist.Value);
            }
            else if (value is Money)
            {
                var money = (Money)value;
                entity[attributeName] = new Microsoft.Xrm.Sdk.Money(money.Value);
            }
            else if (
                value is DateTime ||
                value is string ||
                value is decimal ||
                value is int ||
                value is bool)
            {
                entity[attributeName] = value;
            }
            else if (value is DateTime?)
            {
                var dt = (DateTime?)value;
                if (dt.HasValue)
                {
                    entity[attributeName] = dt.Value;
                }
                else
                {
                    entity[attributeName] = null;
                }
            }
            else if (value is decimal?)
            {
                var dec = (decimal?)value;
                if (dec.HasValue)
                {
                    entity[attributeName] = dec.Value;
                }
                else
                {
                    entity[attributeName] = null;
                }
            }
            else if (value is int?)
            {
                var i = (int?)value;
                if (i.HasValue)
                {
                    entity[attributeName] = i.Value;
                }
                else
                {
                    entity[attributeName] = null;
                }
            }
            else if (value is bool?)
            {
                var b = (bool?)value;
                if (b.HasValue)
                {
                    entity[attributeName] = b.Value;
                }
                else
                {
                    entity[attributeName] = null;
                }
            }
        }

        public static List<T> BuildList<T>(EntityCollection entities, Enums.MappingType mappingType) where T : new()
        {
            List<T> lst = new List<T>();

            if (entities == null)
                return new List<T>();

            foreach (Entity entity in entities.Entities)
            {
                lst.Add(Build<T>(entity, mappingType));
            }

            return lst;
        }

        public static T Build<T>(Entity entity, Enums.MappingType mappingType, EntityMetadata entityMetadata = null) where T : new()
        {
            if (entity == null)
                return new T();

            T t = new T();

            var properties = typeof(T).GetProperties()
                .Where(p =>
                    (
                        p.PropertyType.GetInterface(typeof(IEnumerable).Name) == null ||
                        p.PropertyType == typeof(string)
                    ) &&
                    !p.GetCustomAttributes(typeof(VirtualAttribute), false).Any() &&
                    p.CanWrite && p.CanRead
                );

            switch (mappingType)
            {
                case Accendo.DynamicsIntegration.Crm2015.Enums.MappingType.CustomCrmDto:
                    BuildCustomCrmDto(t, entity, properties);
                    break;
                case Accendo.DynamicsIntegration.Crm2015.Enums.MappingType.EntityLogicalName:
                    BuildEntityLogicalName(t, entity, properties, entityMetadata);
                    break;
                case Accendo.DynamicsIntegration.Crm2015.Enums.MappingType.None:
                default:
                    BuildReflection(t, entity, properties);
                    break;
            }

            return t;
        }

        private static void BuildReflection(object t, Entity entity, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (entity.Contains(property.Name))
                {
                    MapCustomCrmDtoProperty(t, entity[property.Name], property);
                }
            }
        }

        private static void BuildCustomCrmDto<T>(T t, Entity entity, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
            var filteredProperties = from p in properties
                                     where p.GetCustomAttributes(typeof(CustomCrmDtoPropertyAttribute), false).Any()
                                     select p;

            foreach (var property in filteredProperties)
            {
                var customCrmProperty = (CustomCrmDtoPropertyAttribute)property.GetCustomAttributes(typeof(CustomCrmDtoPropertyAttribute), false).First();

                if (entity.Contains(customCrmProperty.AttributeName))
                {
                    MapCustomCrmDtoProperty(t, entity[customCrmProperty.AttributeName], property);
                }
            }
        }

        private static void BuildEntityLogicalName<T>(T t, Entity entity, IEnumerable<System.Reflection.PropertyInfo> properties, EntityMetadata entityMetadata = null)
        {
            var filteredProperties = from p in properties
                                     where p.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).Any()
                                     select p;
            IEnumerable<AttributeMetadata> attributesToRetrieve = new List<AttributeMetadata>();

            if (entityMetadata != null)
            {
                attributesToRetrieve = entityMetadata.Attributes.Where(x => x.IsValidForRead ?? false);
            }

            foreach (var property in filteredProperties)
            {
                var attributeLogicalNameProperty = (AttributeLogicalNameAttribute)property.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).First();

                if (entity.Contains(attributeLogicalNameProperty.LogicalName) && (!attributesToRetrieve.Any() || attributesToRetrieve.Any(x => x.LogicalName == attributeLogicalNameProperty.LogicalName)))
                {
                    MapAttributeLogicalNameProperty(t, entity[attributeLogicalNameProperty.LogicalName], property);
                }
            }
        }

        private static void MapAttributeLogicalNameProperty<T>(T t, object value, System.Reflection.PropertyInfo property)
        {
            property.SetValue(t, value);
        }

        private static void MapCustomCrmDtoProperty<T>(T t, object value, System.Reflection.PropertyInfo property)
        {
            if (value is EntityReference)
            {
                var entityReference = (EntityReference)value;
                property.SetValue(t, new Lookup(entityReference.Id, entityReference.Name, entityReference.LogicalName));
            }
            else if (value is OptionSetValue)
            {
                var optionSetValue = (OptionSetValue)value;
                property.SetValue(t, new Picklist(optionSetValue.Value));
            }
            else if (value is Microsoft.Xrm.Sdk.Money)
            {
                var money = (Microsoft.Xrm.Sdk.Money)value;
                property.SetValue(t, new Money(money.Value));
            }
            else if (
                value is DateTime ||
                value is string ||
                value is decimal ||
                value is int ||
                value is bool ||
                value is Guid ||
                value is decimal? ||
                value is int? ||
                value is bool?)
            {
                property.SetValue(t, value);
            }
        }

        public static DataTable BuildTable(EntityCollection collection, RetrieveEntityResponse metadata)
        {
            DataRow dr;
            DataTable dt = new DataTable(collection.EntityName);

            foreach (AttributeMetadata attribute in metadata.EntityMetadata.Attributes)
            {
                if (attribute.AttributeType.HasValue && attribute.AttributeType.Value == AttributeTypeCode.Decimal)
                    dt.Columns.Add(attribute.SchemaName.ToLower(), typeof(decimal));

                else if (attribute.AttributeType.HasValue && attribute.AttributeType.Value == AttributeTypeCode.Integer)
                    dt.Columns.Add(attribute.SchemaName.ToLower(), typeof(int));

                else if (attribute.AttributeType.HasValue && attribute.AttributeType.Value == AttributeTypeCode.Double)
                    dt.Columns.Add(attribute.SchemaName.ToLower(), typeof(double));

                else if (attribute.AttributeType.HasValue && attribute.AttributeType.Value == AttributeTypeCode.DateTime)
                    dt.Columns.Add(attribute.SchemaName.ToLower(), typeof(DateTime));

                else
                    dt.Columns.Add(attribute.SchemaName.ToLower(), typeof(string));
            }
            dt.AcceptChanges();

            foreach (Entity entity in collection.Entities)
            {
                dr = dt.NewRow();

                foreach (DataColumn column in dt.Columns)
                {
                    if (entity.Contains(column.ColumnName))
                    {
                        if (entity[column.ColumnName].GetType() == typeof(String))
                        {
                            dr[column.ColumnName] = Convert.ToString((string)entity[column.ColumnName]);
                        }
                        else if (entity[column.ColumnName].GetType() == typeof(OptionSetValue))
                        {
                            dr[column.ColumnName] = Convert.ToString(((OptionSetValue)entity[column.ColumnName]).Value);
                        }
                        else if (entity[column.ColumnName].GetType() == typeof(EntityReference))
                        {
                            dr[column.ColumnName] = Convert.ToString(((EntityReference)entity[column.ColumnName]).Id);
                        }
                        else if (entity[column.ColumnName].GetType() == typeof(DateTime))
                        {
                            dr[column.ColumnName] = (DateTime)entity[column.ColumnName]; //Convert.ToString((DateTime)entity[column.ColumnName]);
                        }
                        else if (entity[column.ColumnName].GetType() == typeof(Guid))
                        {
                            dr[column.ColumnName] = Convert.ToString((Guid)entity[column.ColumnName]);
                        }
                        else if (entity[column.ColumnName].GetType() == typeof(Decimal))
                        {
                            dr[column.ColumnName] = (Decimal)entity[column.ColumnName]; //Convert.ToString((Decimal)entity[column.ColumnName]);
                        }
                        else if (entity[column.ColumnName].GetType() == typeof(Double))
                        {
                            dr[column.ColumnName] = (Double)entity[column.ColumnName]; //Convert.ToString((Decimal)entity[column.ColumnName]);
                        }
                        else if (entity[column.ColumnName].GetType() == typeof(Int32))
                        {
                            dr[column.ColumnName] = (Int32)entity[column.ColumnName]; // Convert.ToString((Int32)entity[column.ColumnName]);
                        }
                        else if (entity[column.ColumnName].GetType() == typeof(bool))
                        {
                            dr[column.ColumnName] = Convert.ToString((bool)entity[column.ColumnName]);
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            dt.AcceptChanges();
            return dt;
        }
    }
}
