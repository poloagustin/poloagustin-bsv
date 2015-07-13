using Accendo.DynamicsIntegration.Crm2015;
using Supervielle.DataMigration.DataAccess.Crm.Interfaces;
using Supervielle.DataMigration.Domain.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.DataAccess.Crm
{
    public class AccountDao : GenericDao<Account>, IAccountDao
    {
        protected override string idAttribute
        {
            get { return "accountid"; }
        }

        protected override string nameAttribute
        {
            get { return "name"; }
        }
    }
}
