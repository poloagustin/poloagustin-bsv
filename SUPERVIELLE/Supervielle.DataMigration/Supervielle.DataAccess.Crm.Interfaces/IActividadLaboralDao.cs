using Accendo.DynamicsIntegration.Crm2015.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supervielle.DataMigration.Domain.Crm;
using Microsoft.Xrm.Sdk;

namespace Supervielle.DataMigration.DataAccess.Crm.Interfaces
{
    public interface IActividadLaboralDao : IMasterDataDao<bsv_actividades_economicas>
    {
    }
}
