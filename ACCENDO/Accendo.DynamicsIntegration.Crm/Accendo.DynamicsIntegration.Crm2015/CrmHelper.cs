using Microsoft.Crm.Sdk.Samples;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015
{
    public class CrmHelper
    {
        private IOrganizationService service;

        public IOrganizationService Service
        {
            get { return service; }
            set { service = value; }
        }

        public CrmHelper()
        {
            this.Connect();
        }

        private void Connect()
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration();

            this.service = new OrganizationServiceProxy(serverConfig.OrganizationUri, serverConfig.HomeRealmUri, serverConfig.Credentials, serverConfig.DeviceCredentials);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return this.service.Execute(request);
        }

        public Guid Create(Entity entity)
        {
            return this.service.Create(entity);
        }

        public void Delete(string entityName, Guid id)
        {
            this.service.Delete(entityName, id);
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            this.service.Associate(entityName, entityId, relationship, relatedEntities);
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            this.service.Disassociate(entityName, entityId, relationship, relatedEntities);
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return this.service.Retrieve(entityName, id, columnSet);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return this.service.RetrieveMultiple(query);
        }
        
        public void Update(Entity entity)
        {
            this.service.Update(entity);
        }
    }
}
