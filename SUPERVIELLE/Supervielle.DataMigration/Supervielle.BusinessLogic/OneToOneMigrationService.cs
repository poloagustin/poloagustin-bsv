using Supervielle.DataMigration.BusinessLogic.Intefaces;
using Supervielle.DataMigration.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supervielle.DataMigration.DataAccess.Sql;
using Supervielle.DataMigration.DataAccess.Sql.Interfaces;
using Supervielle.DataMigration.DataAccess.Crm.Interfaces;
using Supervielle.DataMigration.DataAccess.Crm;
using Microsoft.Xrm.Sdk.Query;
using Supervielle.DataMigration.Domain.Enums;

namespace Supervielle.DataMigration.BusinessLogic
{
    public class OneToOneMigrationService : IOneToOneMigrationService
    {
        private IDynamicRecordDao dynamicRecordDao;
        private IEntityDao entityDao;

        public OneToOneMigrationService()
        {
            this.dynamicRecordDao = new DynamicRecordDao();
            this.entityDao = new EntityDao();

        }

        public void Migrate(IEnumerable<AvailableValue> availableValues)
        {
            foreach (var availableValue in availableValues)
            {
                Migrate(availableValue.Configuration);
            }
        }

        private void Migrate(AvailableValueConfiguration availableValueConfiguration)
        {
            // Retrieve all records from Origin
            var originRecords = this.dynamicRecordDao.RetrieveRecords(
                availableValueConfiguration.OriginTable, 
                availableValueConfiguration.Fields.ToDictionary(x => x.OriginColumn, y => y.OriginType));

            // Get records to Update
            var compoundKey = availableValueConfiguration.Fields.Where(x => x.IsKey).Select(x => x.DestinationAttribute);
            var filters = GetConditionExpressionCollection(originRecords, compoundKey, availableValueConfiguration);

            var recordsToUpdate = this.entityDao.RetrieveExistingRecords(filters);
        }

        private IEnumerable<FilterExpression> GetConditionExpressionCollection(
            IEnumerable<Domain.Sql.DynamicRecord> originRecords, 
            IEnumerable<string> compoundKey, 
            AvailableValueConfiguration availableValueConfiguration)
        {
            var filters = new List<FilterExpression>();

            foreach (var record in originRecords)
            {
                var filter = new FilterExpression(LogicalOperator.And);
                
                foreach (var destinationAttribute in compoundKey)
                {
                    var currentField =availableValueConfiguration.Fields.First(x => x.DestinationAttribute == destinationAttribute); 
                    
                    if (record.Values[currentField.OriginColumn] == null)
                    {
                        filter.AddCondition(new ConditionExpression(destinationAttribute, ConditionOperator.Null));
                    }
                    else if (currentField.DestinationType == DestinationType.DateTime)
                    {
                        filter.AddCondition(new ConditionExpression(destinationAttribute, ConditionOperator.On, record.Values[currentField.OriginColumn]));
                    }
                    else if (currentField.DestinationType == DestinationType.EntityReference)
                    {
                        filter.AddCondition(new ConditionExpression(destinationAttribute, ConditionOperator.On, null));
                    }
                    else
                    {
                        filter.AddCondition(new ConditionExpression(destinationAttribute, ConditionOperator.Equal, record.Values[currentField.OriginColumn]));
                    }
                }

                filters.Add(filter);
            }

            return filters;
        }
    }
}
