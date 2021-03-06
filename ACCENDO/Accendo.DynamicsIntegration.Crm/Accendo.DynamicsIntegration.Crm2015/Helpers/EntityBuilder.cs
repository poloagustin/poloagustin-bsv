﻿using Accendo.DynamicsIntegration.Crm2015.CustomAttributes;
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
        public static List<Entity> BuildEntityList(IEnumerable objects, Enums.MappingType mappingType)
        {
            List<Entity> entities = new List<Entity>();

            if (objects == null)
                return new List<Entity>();

            foreach (var obj in objects)
            {
                entities.Add(BuildEntity(obj, mappingType));
            }

            return entities;
        }

        public static Entity BuildEntity(object obj, Enums.MappingType mappingType, EntityMetadata entityMetadata = null)
        {
            if (obj == null)
                return null;

            Entity entity = new Entity();


            entity.LogicalName = obj.GetType().Name;

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
                    BuildEntityLogicalNameEntity(obj, entity, properties);
                    break;
                case Accendo.DynamicsIntegration.Crm2015.Enums.MappingType.None:
                default:
                    BuildReflectionEntity(obj, entity, properties);
                    break;
            }

            var isCustomCrmAttribute = obj.GetType().GetCustomAttributes(true).Any(x => x.GetType() == typeof(CustomCrmDtoAttribute));
            if (!isCustomCrmAttribute)
            {
                foreach (var property in properties)
                {
                    var propValue = property.GetValue(obj, null);

                    if (propValue != null)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            entity.Attributes.Add(property.Name, (string)propValue);
                        }
                        else if (property.PropertyType == typeof(Picklist))
                        {
                            if (((Picklist)propValue).Value >= 0)
                                entity.Attributes.Add(property.Name, new OptionSetValue(((Picklist)propValue).Value));
                        }
                        else if (property.PropertyType == typeof(Lookup))
                        {
                            if (((Lookup)propValue).Id != Guid.Empty)
                            {
                                entity.Attributes.Add(property.Name, new EntityReference(
                                    ((Lookup)propValue).LogicalName,
                                    ((Lookup)propValue).Id));
                            }
                            else
                            {
                                entity.Attributes.Add(property.Name, null);
                            }
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            if (((DateTime)propValue).Year > 1900)
                                entity.Attributes.Add(property.Name, ((DateTime)propValue));
                        }
                        else if (property.PropertyType == typeof(Guid))
                        {
                            if (((Guid)propValue) != Guid.Empty)
                                entity.Attributes.Add(property.Name, (Guid)propValue);
                        }
                        else if (property.PropertyType == typeof(decimal))
                        {
                            entity.Attributes.Add(property.Name, (decimal)propValue);
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            entity.Attributes.Add(property.Name, (int)propValue);
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            entity.Attributes.Add(property.Name, (bool)propValue);
                        }
                        else if (property.PropertyType == typeof(decimal?))
                        {
                            entity.Attributes.Add(property.Name, (decimal)propValue);
                        }
                        else if (property.PropertyType == typeof(int?))
                        {
                            entity.Attributes.Add(property.Name, (int)propValue);
                        }
                        else if (property.PropertyType == typeof(bool?))
                        {
                            entity.Attributes.Add(property.Name, (bool)propValue);
                        }
                        else if (property.PropertyType == typeof(Crm2015.Money))
                        {
                            entity.Attributes.Add(property.Name, new Microsoft.Xrm.Sdk.Money(((Crm2015.Money)propValue).Value));
                        }
                    }
                }
            }
            else
            {
                foreach (var property in properties)
                {
                    var propValue = property.GetValue(obj, null);
                    var attrs = property.GetCustomAttributes(true);
                    var attrName = attrs.Any(x => x.GetType() == typeof(CustomCrmDtoPropertyAttribute)) ? ((CustomCrmDtoPropertyAttribute)attrs.First(x => x.GetType() == typeof(CustomCrmDtoPropertyAttribute))).AttributeName : string.Empty;
                    if (propValue != null && !string.IsNullOrEmpty(attrName))
                    {
                        if (attrName.GetType() == typeof(string))
                        {
                            entity.Attributes.Add(property.Name, (string)propValue);
                        }
                        else if (attrName.GetType() == typeof(Picklist))
                        {
                            if (((Picklist)propValue).Value >= 0)
                                entity.Attributes.Add(property.Name, new OptionSetValue(((Picklist)propValue).Value));
                        }
                        else if (property.PropertyType == typeof(Lookup))
                        {
                            if (((Lookup)propValue).Id != Guid.Empty)
                                entity.Attributes.Add(property.Name, new EntityReference(
                                    ((Lookup)propValue).LogicalName,
                                    ((Lookup)propValue).Id));
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            if (((DateTime)propValue).Year > 1900)
                                entity.Attributes.Add(property.Name, ((DateTime)propValue));
                        }
                        else if (property.PropertyType == typeof(Guid))
                        {
                            if (((Guid)propValue) != Guid.Empty)
                                entity.Attributes.Add(property.Name, (Guid)propValue);
                        }
                        else if (property.PropertyType == typeof(decimal))
                        {
                            entity.Attributes.Add(property.Name, (decimal)propValue);
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            entity.Attributes.Add(property.Name, (int)propValue);
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            entity.Attributes.Add(property.Name, (bool)propValue);
                        }
                        else if (property.PropertyType == typeof(decimal?))
                        {
                            entity.Attributes.Add(property.Name, (decimal)propValue);
                        }
                        else if (property.PropertyType == typeof(int?))
                        {
                            entity.Attributes.Add(property.Name, (int)propValue);
                        }
                        else if (property.PropertyType == typeof(bool?))
                        {
                            entity.Attributes.Add(property.Name, (bool)propValue);
                        }
                        else if (property.PropertyType == typeof(Crm2015.Money))
                        {
                            entity.Attributes.Add(property.Name, new Microsoft.Xrm.Sdk.Money(((Crm2015.Money)propValue).Value));
                        }
                    }
                }
            }

            return entity;
        }

        private static void BuildReflectionEntity(object obj, Entity entity, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (entity.Contains(property.Name))
                {
                    MapCustomCrmDtoEntityProperty(property.GetValue(obj), entity, property.Name);
                }
            }
        }

        private static void BuildEntityLogicalNameEntity(object obj, Entity entity, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
            var filteredProperties = from p in properties
                                     where p.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).Any()
                                     select p;

            foreach (var property in filteredProperties)
            {
                var attributeLogicalNameProperty = (AttributeLogicalNameAttribute)property.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).First();

                if (entity.Contains(attributeLogicalNameProperty.LogicalName))
                {
                    MapLogicalNameEntityProperty(property.GetValue(obj), entity, attributeLogicalNameProperty.LogicalName);
                }
            }
        }

        private static void MapLogicalNameEntityProperty(object value, Entity entity, string logicalName)
        {
            entity[logicalName] = value;
        }

        private static void BuildCustomCrmDtoEntity(object obj, Entity entity, IEnumerable<System.Reflection.PropertyInfo> properties)
        {
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

                        if (entityMetadata != null)
    {
                var attributesToRetrieve = entityMetadata.Attributes.Where(x => x.IsValidForRead ?? false);
    }


            foreach (var property in filteredProperties)
            {
                var attributeLogicalNameProperty = (AttributeLogicalNameAttribute)property.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).First();

                if (entity.Contains(attributeLogicalNameProperty.LogicalName))
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
