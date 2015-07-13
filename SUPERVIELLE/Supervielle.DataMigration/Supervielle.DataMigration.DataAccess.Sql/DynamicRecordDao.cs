using Supervielle.DataMigration.DataAccess.Sql.Helpers;
using Supervielle.DataMigration.DataAccess.Sql.Interfaces;
using Supervielle.DataMigration.Domain;
using Supervielle.DataMigration.Domain.Enums;
using Supervielle.DataMigration.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.DataAccess.Sql
{
    public class DynamicRecordDao : GenericDao<DynamicRecord>, IDynamicRecordDao
    {
        private string _tableName;
        private string _idAttribute;

        protected override string tableName
        {
            get { return this._tableName; }
        }

        protected string idAttribute
        {
            get { return this._idAttribute; }
        }

        public DynamicRecordDao(string tableName = "", string idAttribute = "", SqlMapping attributes = null)
            : base(attributes)
        {
            this._idAttribute = idAttribute;
            this._tableName = tableName;
        }

        public IEnumerable<DynamicRecord> RetrieveRecords(string tableName, Dictionary<string, OriginType> fields)
        {
            throw new NotImplementedException();
        }
    }
}
