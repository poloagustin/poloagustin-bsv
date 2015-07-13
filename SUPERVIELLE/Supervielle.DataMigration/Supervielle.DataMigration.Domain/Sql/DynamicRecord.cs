using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.Domain.Sql
{
    public class DynamicRecord : BaseRecord
    {
        public Dictionary<string, object> Values { get; set; }
    }
}
