using Accendo.DynamicsIntegration.Crm2015;
using Microsoft.Xrm.Sdk.Query;
using Supervielle.DataAccess.Crm.Interfaces;
using Supervielle.Domain.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Crm
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
