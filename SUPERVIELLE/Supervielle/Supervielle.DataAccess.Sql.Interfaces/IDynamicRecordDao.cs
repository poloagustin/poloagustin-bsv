using Supervielle.Domain.Enums;
using Supervielle.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Sql.Interfaces
{
    public interface IDynamicRecordDao : IGenericDao<DynamicRecord>
    {
        IEnumerable<DynamicRecord> RetrieveRecords(string tableName, Dictionary<string, OriginType> fields);
    }
}
