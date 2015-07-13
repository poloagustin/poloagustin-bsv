using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.Domain.Enums
{
    public enum DestinationType
    {
        String,
        Int,
        Decimal,
        EntityReference,
        OptionSetValue,
        Money,
        DateTime,
    }
}
