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
    public class RelacionCuentaPersonaFisicaDao : GenericDao<bsv_relacion_fisica_cuentas>, IRelacionCuentaPersonaFisicaDao
    {
        protected override string idAttribute
        {
            get { return "bsv_relacion_fisica_cuentasid"; }
        }

        protected override string nameAttribute
        {
            get { return "bsv_name"; }
        }
    }
}
