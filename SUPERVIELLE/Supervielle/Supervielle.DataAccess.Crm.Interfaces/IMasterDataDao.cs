using Accendo.DynamicsIntegration.Crm2015.Interfaces;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Crm.Interfaces
{
    public interface IMasterDataDao<T> : IGenericDao<T> where T : new()
    {
        T GetObjectByCode(object code);

        EntityReference GetEntityReferenceByCode(object code);
    }
}
