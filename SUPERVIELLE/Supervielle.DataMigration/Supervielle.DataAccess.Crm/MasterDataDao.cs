using Accendo.DynamicsIntegration.Crm2015;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Supervielle.DataMigration.DataAccess.Crm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.DataAccess.Crm
{
    public abstract class MasterDataDao<T> : GenericDao<T>, IMasterDataDao<T> where T : new()
    {
        protected abstract string codeAttribute { get; }

        public virtual T GetObjectByCode(object code)
        {
            var entity = this.GetEntityByCode(code);
            return base.EntityToObject(entity);
        }

        public virtual Entity GetEntityByCode(object code)
        {
            var masterDataElement = MasterDataCache.Instance.GetItem(this.entityName, x => x.Contains(this.codeAttribute) && x[this.codeAttribute].Equals(code));

            if (masterDataElement != null)
            {
                return masterDataElement;
            }
            else
            {
                var query = new QueryByAttribute(this.entityName);
                query.AddAttributeValue(this.codeAttribute, code);
                query.ColumnSet = GetColumnSet();
                query.PageInfo = new PagingInfo();
                query.PageInfo.Count = 1;
                query.PageInfo.PageNumber = 1;

                var entities = base.RetrieveAllRecordsFromQuery(query);

                if (entities.Any())
                {
                    var entity = entities.First();
                    MasterDataCache.Instance.AddItem(this.entityName, entity);
                    return entity;
                }
                else
                {
                    throw new Exception("No existen un registro en la entidad '" + this.entityName + "' con el campo '" + this.codeAttribute + "' = '" + code + "'");
                }
            }
        }

        public EntityReference GetEntityReferenceByCode(object code)
        {
            var entity = this.GetEntityByCode(code);
            var entityReference = new EntityReference();
            entityReference.Id = entity.Id;
            entityReference.LogicalName = entity.LogicalName;
            entityReference.Name = entity.Attributes.Contains(this.nameAttribute) ? (string)entity[this.nameAttribute] : string.Empty;

            return entityReference;
        }
    }
}
