using Accendo.DynamicsIntegration.Crm2015;
using Microsoft.Xrm.Sdk.Query;
using Supervielle.DataMigration.DataAccess.Crm.Interfaces;
using Supervielle.DataMigration.Domain.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.DataAccess.Crm
{
    public class ContactDao : GenericDao<Contact>, IContactDao
    {
        protected override string idAttribute
        {
            get { return "contactid"; }
        }

        protected override string nameAttribute
        {
            get { return "name"; }
        }
    }
}
