using Accendo.DynamicsIntegration.Crm2015.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supervielle.Domain.Crm;
using Microsoft.Xrm.Sdk;

namespace Supervielle.DataAccess.Crm.Interfaces
{
    public interface IProvinciaDao : IMasterDataDao<bsv_provincias>
    {
    }
}
