using Supervielle.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.BusinessLogic.Intefaces
{
    public interface IProductMigrationService
    {
        void Migrate(List<AvailableValue> values);
    }
}
