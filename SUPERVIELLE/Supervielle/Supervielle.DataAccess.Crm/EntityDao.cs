using Accendo.DynamicsIntegration.Crm2015;
using Microsoft.Xrm.Sdk;
using Supervielle.DataAccess.Crm.Interfaces;
using Supervielle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Crm
{
    public class EntityDao : GenericDao<Entity>, IEntityDao
    {
        private string _entityName;
        private string _idAttribute;
        private string _nameAttribute;
        private IDictionary<string, DestinationType> attributes;

        protected override string idAttribute
        {
            get { return this._idAttribute; }
        }

        protected override string nameAttribute
        {
            get { return this._nameAttribute; }
        }

        public EntityDao(string entityName = "", string idAttribute = "", string nameAttribute = "", IDictionary<string, DestinationType> attributes = null)
        {
            this._idAttribute = idAttribute;
            this._entityName = entityName;
            this._nameAttribute = nameAttribute;
            this.attributes = attributes;
        }

        public object RetrieveExistingRecords(IEnumerable<Microsoft.Xrm.Sdk.Query.FilterExpression> filters)
        {
            throw new NotImplementedException();
        }
    }
}
