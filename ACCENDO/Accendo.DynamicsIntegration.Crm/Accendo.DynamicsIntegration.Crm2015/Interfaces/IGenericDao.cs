using Accendo.DynamicsIntegration.Crm2015.Enums;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Interfaces
{
    public interface IGenericDao<T> where T : new()
    {
        void DeleteAllEntities(IEnumerable<Entity> entities);
        void DeleteMany(IEnumerable<T> objs);
        void DeleteMany(IEnumerable<EntityReference> entityReferences);
        void DeleteManyEntities(IEnumerable<Entity> entities);
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        T Save(T obj);
        IEnumerable<T> SaveAll(IEnumerable<T> objs);
        void SaveAllEntities(IEnumerable<Entity> notes);
        Entity SaveEntity(Entity entity);
        IEnumerable<T> SaveMany(IEnumerable<T> objs);
        IEnumerable<Entity> SaveManyEntities(IEnumerable<Entity> entities);
        void UpdateMany(IEnumerable<T> objs);
        void UpdateManyEntities(List<Entity> entitiesToUpdate);
        void SetStateCode(Guid id, StateCode stateCode, StatusCode statusCode);
        void SetStateCodeMany(IEnumerable<T> objs, StateCode stateCode, StatusCode statusCode);
        void Update(T obj);
        IEnumerable<T> GetFiltered(IEnumerable<FilterExpression> filters);
        EntityReference GetEntityReference(Entity x);
    }
}
