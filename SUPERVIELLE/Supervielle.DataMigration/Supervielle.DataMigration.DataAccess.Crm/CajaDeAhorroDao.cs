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
    public class CuentaDao : GenericDao<bsv_cuentas>, ICuentaDao
    {
        protected override string idAttribute
        {
            get { return "bsv_cuentasid"; }
        }

        protected override string nameAttribute
        {
            get { return "bsv_numero_de_cuentas"; }
        }
    }
}
