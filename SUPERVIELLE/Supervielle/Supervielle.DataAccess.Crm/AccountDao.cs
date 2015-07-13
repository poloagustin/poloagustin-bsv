using Accendo.DynamicsIntegration.Crm2015;
using Supervielle.DataAccess.Crm.Interfaces;
using Supervielle.Domain.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Crm
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
