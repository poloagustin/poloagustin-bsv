using Supervielle.DataMigration.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.BusinessLogic.Intefaces
{
    public interface IOneToOneMigrationService
    {
        void Migrate(IEnumerable<AvailableValue> availableValues);
    }
}
