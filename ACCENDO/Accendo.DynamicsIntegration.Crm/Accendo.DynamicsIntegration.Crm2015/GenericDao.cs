using Accendo.DynamicsIntegration.Crm2015.CustomAttributes;
using Accendo.DynamicsIntegration.Crm2015.Enums;
using Accendo.DynamicsIntegration.Crm2015.Exceptions;
using Accendo.DynamicsIntegration.Crm2015.Helpers;
using Accendo.DynamicsIntegration.Crm2015.Interfaces;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;


namespace Accendo.DynamicsIntegration.Crm2015
{
    public abstract class GenericDao<T> : IGenericDao<T> where T : new()
    {
        protected CrmFetchXml currentFetch;
        protected CrmHelper helper;
        protected static string entityLogicalName;
        private volatile static object lockObj = new object();
        protected MappingType mappingType;
        protected List<CrmAttributeXml> crmAttributes;
        protected static EntityMetadata entityMetadata;

        public GenericDao()
        {
            this.currentFetch = new CrmFetchXml(this.entityName, this.idAttribute);
            this.helper = CrmHelperSingleton.Instance;
            this.mappingType = GetMappingType();
            entityMetadata = GetEntityMetadata();
            this.crmAttributes = GetTypeAttributes();
        }

        private EntityMetadata GetEntityMetadata()
        {
            var request = new RetrieveEntityRequest();
            request.EntityFilters = EntityFilters.All;
            request.LogicalName = this.entityName;
            request.RetrieveAsIfPublished = true;

            var response = (RetrieveEntityResponse)this.helper.Execute(request);

            return response.EntityMetadata;
        }

        private MappingType GetMappingType()
        {
            var type = typeof(T);
            var customAttributes = type.GetCustomAttributes(true);

            if (customAttributes.Any(x => x.GetType() == typeof(CustomCrmDtoAttribute)))
            {
                return MappingType.CustomCrmDto;
            }
            else if (customAttributes.Any(x => x.GetType() == typeof(EntityLogicalNameAttribute)))
            {
                return MappingType.EntityLogicalName;
            }
            else
            {
                return MappingType.None;
            }
        }

        protected virtual string entityName
        {

            get
            {
                lock (lockObj)
                {
                    var type = typeof(T);

                    var attrs = typeof(T).GetCustomAttributes(true);

                    if (type.BaseType == typeof(Entity) && attrs.Any(x => x.GetType() == typeof(EntityLogicalNameAttribute)))
                    {
                        entityLogicalName = ((EntityLogicalNameAttribute)attrs.First(x => x.GetType() == typeof(EntityLogicalNameAttribute))).LogicalName;
                    }
                    else if (attrs.Any(x => x.GetType() == typeof(CustomCrmDtoAttribute)))
                    {
                        entityLogicalName = ((CustomCrmDtoAttribute)attrs.First(x => x.GetType() == typeof(CustomCrmDtoAttribute))).EntityName;
                    }
                    else
                    {
                        entityLogicalName = typeof(T).Name;
                    }
                }

                return entityLogicalName;
            }
        }

        protected abstract string idAttribute { get; }

        protected abstract string nameAttribute { get; }

        private static PropertyInfo idProperty;

        protected PropertyInfo IdProperty
        {

            get
            {
                lock (lockObj)
                {
                    if (idProperty == null)
                    {
                        switch (mappingType)
                        {
                            case MappingType.CustomCrmDto:
                                var customDtoProperties = typeof(T).GetProperties().Where(x => x.GetCustomAttributes<CustomCrmDtoPropertyAttribute>().Any());
                                idProperty = customDtoProperties.First(x => x.GetCustomAttribute<CustomCrmDtoPropertyAttribute>().AttributeName == idAttribute);
                                break;
                            case MappingType.EntityLogicalName:
                                var logicalNameProperties = typeof(T).GetProperties().Where(x => x.GetCustomAttributes<AttributeLogicalNameAttribute>().Any());
                                idProperty = logicalNameProperties.First(x => x.GetCustomAttribute<AttributeLogicalNameAttribute>().LogicalName == idAttribute);
                                break;
                            case MappingType.None:
                            default:
                                idProperty = typeof(T).GetProperties().First(x => x.GetType() == typeof(Guid));
                                break;
                        }
                    }
                }

                return idProperty;
            }
        }

        public List<CrmAttributeXml> GetTypeAttributes()
        {
            if (this.crmAttributes != null)
            {
                return this.crmAttributes;
            }

            var properties = typeof(T).GetProperties();
            var nonCollectionsNonVirtualProperties = properties.Where(p =>
                    (
                        p.PropertyType.GetInterface(typeof(IEnumerable).Name) == null ||
                        p.PropertyType == typeof(string)
                    ) &&
                    !p.GetCustomAttributes(typeof(VirtualAttribute), false).Any()
                );

            if (this.mappingType == MappingType.CustomCrmDto)
            {
                var customCrmProperties =
                    from p in nonCollectionsNonVirtualProperties
                    where p.GetCustomAttributes(typeof(CustomCrmDtoPropertyAttribute), false).Any()
                    select (CustomCrmDtoPropertyAttribute)p.GetCustomAttributes(typeof(CustomCrmDtoPropertyAttribute), false).First();
                return (from cdp in customCrmProperties
                        select new CrmAttributeXml(cdp.AttributeName)).ToList();
            }
            else if (this.mappingType == MappingType.EntityLogicalName)
            {
                var entityLogicalNameProperties =
                    from p in nonCollectionsNonVirtualProperties
                    where p.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).Any()
                    select (AttributeLogicalNameAttribute)p.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).First();
                var entityLogicalNamePropertiesReadable = entityLogicalNameProperties.Where(x => entityMetadata.Attributes.Any(y => x.LogicalName == y.LogicalName && (y.IsValidForRead ?? false)));
                return (from eln in entityLogicalNamePropertiesReadable
                        select new CrmAttributeXml(eln.LogicalName)).ToList();
            }
            else
            {
                return (from p in nonCollectionsNonVirtualProperties
                        select new CrmAttributeXml(p.Name)).ToList();
            }
        }

        public void DeleteAllEntities(IEnumerable<Entity> entities)
        {
            foreach (var item in entities)
            {
                this.helper.Service.Delete(item.LogicalName, item.Id);
            }
        }

        public void DeleteMany(IEnumerable<T> objs)
        {
            this.DoBulkActionPaginated(objs, BulkAction.Delete);
        }

        public void DeleteMany(IEnumerable<EntityReference> entityReferences)
        {
            var requests = entityReferences.Select(x =>
            {
                var deleteRequest = new DeleteRequest();
                deleteRequest.Target = x;
                return deleteRequest;
            });

            this.ExecuteManyRequests(requests);
        }

        public void DeleteManyEntities(IEnumerable<Entity> entities)
        {
        }

        public IEnumerable<Entity> DoBulkAction(IEnumerable<Entity> data, BulkAction action)
        {
            var result = new List<Entity>();
            try
            {

                ExecuteMultipleRequest multipleRequest = new ExecuteMultipleRequest()
                {
                    // Assign settings that define execution behavior: continue on error, return responses.
                    Settings = new ExecuteMultipleSettings()
                    {
                        ContinueOnError = false,
                        ReturnResponses = true
                    },

                    // Create an empty organization request collection.
                    Requests = new OrganizationRequestCollection()
                };

                foreach (Entity item in data)
                {
                    switch (action)
                    {
                        case BulkAction.Create:
                            CreateRequest createRequest = new CreateRequest();
                            createRequest.Target = item;
                            multipleRequest.Requests.Add(createRequest);
                            break;

                        case BulkAction.Update:
                            UpdateRequest updateRequest = new UpdateRequest();
                            updateRequest.Target = item;
                            multipleRequest.Requests.Add(updateRequest);
                            break;

                        case BulkAction.Delete:
                            DeleteRequest deleteRequest = new DeleteRequest();
                            deleteRequest.Target = new EntityReference(item.LogicalName, item.Id);
                            multipleRequest.Requests.Add(deleteRequest);
                            break;

                        default:
                            break;
                    }
                }

                var response = (ExecuteMultipleResponse)this.helper.Service.Execute(multipleRequest);
                if (response.IsFaulted)
                {
                    foreach (var responseItem in response.Responses)
                    {
                        if (responseItem.Fault != null)
                        {
                            var ex = new FaultException<OrganizationServiceFault>(responseItem.Fault);
                            throw ex;
                        }
                    }
                }
                else
                {
                    if (action == BulkAction.Create)
                    {
                        foreach (var item in response.Responses)
                        {
                            if (item.Fault == null)
                            {
                                var entity = data.ElementAt(item.RequestIndex);
                                var res = new Entity(entity.LogicalName);
                                res.Attributes = entity.Attributes;
                                res.Id = (Guid)item.Response.Results["id"];

                                result.Add(res);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public IEnumerable<T> DoBulkActionPaginated(IEnumerable<T> data, BulkAction action)
        {
            IList<Entity> page = new List<Entity>();
            List<Entity> pageResult = new List<Entity>();

            for (int i = 0; i < data.Count(); i++)
            {
                var entity = new Entity();
                if (action == BulkAction.Delete)
                {
                    entity = new Entity(this.entityName) { Id = (Guid)typeof(T).GetProperty(this.idAttribute).GetValue(data.ElementAt(i), null) };
                }
                else
                {
                    entity = this.ObjectToEntity(data.ElementAt(i), action);
                    if (action == BulkAction.Create)
                    {
                        entity.Id = Guid.Empty;
                        if (entity.Attributes.Contains(idAttribute))
                        {
                            entity.Attributes.Remove(idAttribute);
                        }
                    }
                }

                page.Add(entity);

                if ((i + 1) % 500 == 0)
                {
                    pageResult.AddRange(DoBulkAction(page, action));
                    page.Clear();
                }
            }

            if (page.Count > 0)
            {
                pageResult.AddRange(DoBulkAction(page, action));
                //WriteLog("Action: '" + action.ToString() + "' sobre " + page.Count.ToString() + " registros...");
                page = null;

            }

            data = this.SetInsertedIdsToObjects(data, pageResult);

            return data;
        }

        public virtual T EntityToObject(Entity entity)
        {
            return EntityBuilder.Build<T>(entity, this.mappingType);
        }

        public IEnumerable<Entity> ExecuteFetch()
        {
            var fetch = this.currentFetch.GetFetchXml();
            var collection = this.helper.Service.RetrieveMultiple(new FetchExpression(fetch));
            var entities = collection.Entities;
            return entities;
        }

        public IEnumerable<Entity> ExecuteFetchPaginated()
        {
            var entities = new List<Entity>();

            int fetchCount = 5000;
            int pageNumber = 1;
            var attrs = GetTypeAttributes();

            if (this.currentFetch.Attributes.Count == 0)
            {
                this.currentFetch.SetAttributes(attrs);
            }
            this.currentFetch.Count = fetchCount;
            EntityCollection returnCollection = new EntityCollection() { MoreRecords = true };
            while (returnCollection.MoreRecords)
            {
                this.currentFetch.Page = pageNumber;
                string fetchXml = this.currentFetch.GetFetchXml();

                returnCollection = this.helper.Service.RetrieveMultiple(new FetchExpression(fetchXml));

                entities.AddRange(returnCollection.Entities);

                pageNumber++;
            }

            return entities;
        }

        public bool ExistsByName(string name)
        {
            this.currentFetch.Reset();
            this.currentFetch.Page = 1;
            this.currentFetch.Count = 1;
            this.currentFetch.Filters.Add(
                new CrmFilterXml(
                    CrmFilterTypeXml.And,
                    new CrmConditionXml(this.nameAttribute, CrmConditionTypeXml.Eq, name)
                )
            );

            var entities = this.ExecuteFetch();
            return entities.Any();
        }

        public virtual IEnumerable<T> GetAll()
        {
            var returnList = new List<T>();
            this.currentFetch.Reset();

            try
            {
                var entities = this.ExecuteFetchPaginated();//this.crmHelper.SearchEntityByFetch(this.currentFetch.GetFetch());

                foreach (var item in entities)
                {
                    returnList.Add(this.EntityToObject(item));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return returnList;
        }

        public virtual T GetById(Guid id)
        {
            T obj;

            try
            {
                var entity = this.helper.Service.Retrieve(this.entityName, id, new ColumnSet(true));
                obj = this.EntityToObject(entity);
                //var entities = this.crmHelper.SearchEntityByAttribute(
                //    entityName,
                //    new string[] { idAttribute },
                //    new string[] { GetConditionOperatorAsString(CrmConditionTypeXml.Eq) },
                //    new string[] { id.ToString() });

                //if (entities.Entities.Count > 1)
                //{
                //    throw new Exception("There are more entities than expected.");
                //}
                //else if (entities.Entities.Count < 1)
                //{
                //    throw new Exception("The given id has not been found within the entity.");
                //}
                //else
                //{
                //    obj = this.EntityToObject(entities.Entities[0]);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return obj;
        }

        public T GetFirstByAttributeValue(string attribute, string value, CrmConditionTypeXml conditionType = CrmConditionTypeXml.Eq)
        {
            this.currentFetch.Reset();
            this.currentFetch.Filters.Add(
                new CrmFilterXml(
                    CrmFilterTypeXml.And,
                    new CrmConditionXml(
                        attribute,
                        conditionType,
                        value
                    )
                )
            );
            this.currentFetch.Page = 1;
            this.currentFetch.Count = 1;

            var entity = this.ExecuteFetch().First();
            return EntityBuilder.Build<T>(entity, this.mappingType);
        }

        public Entity GetFirstEntityByAttributeValue(string attribute, string value, CrmConditionTypeXml conditionType = CrmConditionTypeXml.Eq)
        {
            this.currentFetch.Reset();
            this.currentFetch.Filters.Add(
                new CrmFilterXml(
                    CrmFilterTypeXml.And,
                    new CrmConditionXml(
                        attribute,
                        conditionType,
                        value
                    )
                )
            );
            this.currentFetch.Page = 1;
            this.currentFetch.Count = 1;

            var entities = this.ExecuteFetch();

            if (entities.Any())
            {
                return entities.First();
            }
            else
            {
                return null;
            }
        }

        public Lookup GetLookupByField(string internalIdAttribute, string id)
        {
            var entity = this.GetFirstEntityByAttributeValue(internalIdAttribute, id);
            if (entity == null)
            {
                var newEntity = new Entity(this.entityName);
                newEntity[internalIdAttribute] = id;
                newEntity[this.nameAttribute] = id;
                newEntity.Id = this.helper.Service.Create(newEntity);

                entity = newEntity;
            }
            return new Lookup(entity.Id, entity.Contains(nameAttribute) ? (string)entity[nameAttribute] : string.Empty, entity.LogicalName);
        }

        public virtual IEnumerable<T> GetObjectsFromEntitiesByCurrentFetch()
        {
            var objs = new List<T>();

            try
            {
                var a = this.currentFetch.GetFetchXml();

                var entityCollection = this.ExecuteFetchPaginated();

                foreach (var item in entityCollection)
                {
                    objs.Add(this.EntityToObject(item));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception while " + "GetObjectsFromEntitiesByCurrentFetch()" + " - Detail : " + ex.Message + ex.StackTrace);
            }

            return objs;
        }

        public virtual Entity ObjectToEntity(T obj, BulkAction bulkAction)
        {
            return EntityBuilder.BuildEntity(obj, this.mappingType, bulkAction, entityMetadata);
        }

        public virtual T Save(T obj)
        {
            var entity = this.ObjectToEntity(obj, BulkAction.Create);
            entity.Id = Guid.Empty;

            try
            {
                entity.Id = this.helper.Service.Create(entity);
                typeof(T).GetProperties().Single(x => x.Name == this.idAttribute).SetValue(obj, entity.Id, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return obj;
        }

        public IEnumerable<T> SaveAll(IEnumerable<T> objs)
        {
            var collection = new EntityCollection();
            try
            {
                foreach (var obj in objs)
                {
                    this.Save(obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objs;
        }

        public void SaveAllEntities(IEnumerable<Entity> notes)
        {
            foreach (var note in notes)
            {
                this.helper.Service.Create(note);
            }
        }

        public virtual Entity SaveEntity(Entity entity)
        {
            entity.Id = Guid.Empty;

            try
            {
                entity.Id = this.helper.Service.Create(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entity;
        }

        public IEnumerable<T> SaveMany(IEnumerable<T> objs)
        {
            return this.DoBulkActionPaginated(objs, BulkAction.Create);
        }

        public IEnumerable<Entity> SaveManyEntities(IEnumerable<Entity> entities)
        {
            return this.DoEntitiesBulkActionPaginated(entities, BulkAction.Create);
        }

        public void SetStateCode(Guid id, StateCode stateCode, StatusCode statusCode)
        {
            var request = new SetStateRequest();
            request.EntityMoniker = new EntityReference(this.entityName, id);
            request.State = new OptionSetValue((int)stateCode);
            request.Status = new OptionSetValue((int)statusCode);
            this.helper.Service.Execute(request);
        }

        public void SetStateCodeMany(IEnumerable<T> objs, StateCode stateCode, StatusCode statusCode)
        {
            var objsList = objs.ToList();

            var executeMultipleRequest = new ExecuteMultipleRequest();
            executeMultipleRequest.Requests = new OrganizationRequestCollection();
            executeMultipleRequest.Settings = new ExecuteMultipleSettings();
            executeMultipleRequest.Settings.ContinueOnError = false;
            executeMultipleRequest.Settings.ReturnResponses = false;

            var property = typeof(T).GetProperties().Single(x => x.Name == this.idAttribute);
            var state = new OptionSetValue((int)stateCode);
            var status = new OptionSetValue((int)statusCode);

            for (int i = 0; i < objsList.Count; i++)
            {
                var setStateRequest = new SetStateRequest();
                setStateRequest.EntityMoniker = new EntityReference(this.entityName, (Guid)property.GetValue(objsList[i], null));
                setStateRequest.State = state;
                setStateRequest.Status = status;

                executeMultipleRequest.Requests.Add(setStateRequest);

                if ((i + 1) % 500 == 0)
                {
                    this.helper.Service.Execute(executeMultipleRequest);
                    executeMultipleRequest.Requests = new OrganizationRequestCollection();
                }
            }

            if (executeMultipleRequest.Requests.Count > 0)
            {
                this.helper.Service.Execute(executeMultipleRequest);
            }
        }

        public T Single()
        {
            this.currentFetch.Reset();
            this.currentFetch.Page = 1;
            this.currentFetch.Count = 1;
            var entities = this.ExecuteFetch();
            if (entities.Any())
            {
                return EntityBuilder.Build<T>(entities.First(), this.mappingType);
            }
            else
            {
                return new T();
            }
        }

        public void Update(T obj)
        {
            var entity = this.ObjectToEntity(obj, BulkAction.Update);
            this.helper.Service.Update(entity);
        }

        public void UpdateMany(IEnumerable<T> objs)
        {
            this.DoBulkActionPaginated(objs, BulkAction.Update);
        }

        public void UpdateManyEntities(List<Entity> entitiesToUpdate)
        {
            this.DoEntitiesBulkActionPaginated(entitiesToUpdate, BulkAction.Update);
        }

        protected IEnumerable<T> SetInsertedIdsToObjects(IEnumerable<T> data, IEnumerable<Entity> pageResult)
        {
            for (int i = 0; i < pageResult.Count(); i++)
            {
                IdProperty.SetValue(data.ElementAt(i), pageResult.ElementAt(i).Id, null);
            }

            return data;
        }

        private IEnumerable<Entity> DoEntitiesBulkActionPaginated(IEnumerable<Entity> data, BulkAction action)
        {
            IList<Entity> page = new List<Entity>();
            List<Entity> pageResult = new List<Entity>();

            for (int i = 0; i < data.Count(); i++)
            {
                var targetEntity = data.ElementAt(i);

                if (action == BulkAction.Create)
                {
                    targetEntity.Id = Guid.Empty;
                    if (targetEntity.Attributes.Contains(idAttribute))
                    {
                        targetEntity.Attributes.Remove(idAttribute);
                    }
                }

                page.Add(targetEntity);

                if ((i + 1) % 500 == 0)
                {
                    pageResult.AddRange(DoBulkAction(page, action));
                    page.Clear();
                }
            }

            if (page.Count > 0)
            {
                pageResult.AddRange(DoBulkAction(page, action));
                page = null;
            }
            return pageResult;
        }

        public Lookup ConvertToLookup(T obj)
        {
            var propertyInfo = typeof(T).GetProperties();
            var lookup = new Lookup((Guid)propertyInfo.Single(x => x.Name == this.idAttribute).GetValue(obj), string.Empty, this.entityName);
            return lookup;
        }

        public IEnumerable<T> GetFiltered(IEnumerable<FilterExpression> filters)
        {
            var result = new List<T>();
            var query = new QueryExpression(this.entityName);
            query.ColumnSet = GetColumnSet();

            double filterLimit = 2000;
            var filterCount = filters.Count();
            var loops = Math.Ceiling(filterCount / filterLimit);
            IEnumerable<FilterExpression> currentLoopFilters = null;

            for (int i = 0; i < loops; i++)
            {
                int startElementIndex = i * (int)filterLimit;
                int endElementIndex = startElementIndex + (int)filterLimit;

                if (endElementIndex > filterCount)
                {
                    currentLoopFilters = filters.Skip(startElementIndex).Take(filterCount - startElementIndex);
                }
                else
                {
                    currentLoopFilters = filters.Skip(startElementIndex).Take(endElementIndex);
                }

                query.Criteria = new FilterExpression(LogicalOperator.Or);

                foreach (var filter in currentLoopFilters)
                {
                    query.Criteria.AddFilter(filter);
                }

                var currentLoopResult = RetrieveAllRecordsFromQuery(query);

                foreach (var currentLoopItem in currentLoopResult)
                {
                    result.Add(EntityToObject(currentLoopItem));
                }
            }

            return result;
        }

        protected IEnumerable<Entity> RetrieveAllRecordsFromQuery(QueryExpression query)
        {
            var entities = new List<Entity>();
            var pageIndex = 1;
            var pageLimit = 5000;
            var entityCollection = new EntityCollection() { MoreRecords = true };
            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = pageLimit;

            while (entityCollection.MoreRecords)
            {
                query.PageInfo.PageNumber = pageIndex;
                entityCollection = this.helper.Service.RetrieveMultiple(query);
                entities.AddRange(entityCollection.Entities.ToList());
                pageIndex++;
            }

            return entities;
        }

        protected IEnumerable<Entity> RetrieveAllRecordsFromQuery(QueryByAttribute query)
        {
            var entities = new List<Entity>();
            var pageIndex = 1;
            var pageLimit = 5000;
            var entityCollection = new EntityCollection() { MoreRecords = true };
            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = pageLimit;

            while (entityCollection.MoreRecords)
            {
                query.PageInfo.PageNumber = pageIndex;
                entityCollection = this.helper.Service.RetrieveMultiple(query);
                entities.AddRange(entityCollection.Entities.ToList());
                pageIndex++;
            }

            return entities;
        }

        protected ColumnSet GetColumnSet()
        {
            return new ColumnSet(GetTypeAttributes().Select(x => x.Name).ToArray());
        }

        public EntityReference GetEntityReference(Entity entity)
        {
            return new EntityReference(entity.LogicalName, entity.Contains(this.idAttribute) ? (Guid)entity[this.idAttribute] : entity.Id);
        }

        protected IEnumerable<OrganizationResponse> ExecuteManyRequests(IEnumerable<OrganizationRequest> requests, bool singleTransaction = false)
        {
            var responses = new List<OrganizationResponse>();
            var requestCount = requests.Count();
            var pageLimit = 500;
            var i = 0;
            var page = new List<OrganizationRequest>();

            foreach (var request in requests)
            {
                page.Add(request);

                if ((i + 1) % pageLimit == 0)
                {
                    responses.AddRange(ExecuteMultipleRequests(page, singleTransaction));
                    page.Clear();
                }
            }

            if (page.Any())
            {
                responses.AddRange(ExecuteMultipleRequests(page, singleTransaction));
            }

            return responses;
        }

        private List<OrganizationResponse> ExecuteMultipleRequests(List<OrganizationRequest> page, bool singleTransaction)
        {
            if (singleTransaction)
            {
                var executeTransactionRequest = new ExecuteTransactionRequest();
                executeTransactionRequest.ReturnResponses = true;
                executeTransactionRequest.Requests = new OrganizationRequestCollection();
                foreach (var request in page)
                {
                    executeTransactionRequest.Requests.Add(request);
                }

                var executeTransactionResponse = (ExecuteTransactionResponse)this.helper.Execute(executeTransactionRequest);

                return executeTransactionResponse.Responses.ToList();
            }
            else
            {
                var executeMultipleRequest = new ExecuteMultipleRequest();
                executeMultipleRequest.Settings = new ExecuteMultipleSettings();
                executeMultipleRequest.Settings.ContinueOnError = false;
                executeMultipleRequest.Settings.ReturnResponses = true;
                executeMultipleRequest.Requests = new OrganizationRequestCollection();

                foreach (var request in page)
                {
                    executeMultipleRequest.Requests.Add(request);
                }

                var executeMultipleResponse = (ExecuteMultipleResponse)this.helper.Execute(executeMultipleRequest);

                if (executeMultipleResponse.IsFaulted)
                {
                    throw new ExecuteMultipleException(executeMultipleResponse.Responses.Select(x => x.Fault).ToList());
                }

                return executeMultipleResponse.Responses.Select(x => x.Response).ToList();
            }
        }
    }
}
