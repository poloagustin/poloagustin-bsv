using Supervielle.DataMigration.Domain.Crm;
using Supervielle.DataMigration.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.BusinessLogic.Comparers
{
    public class CuentaComparer
    {
        public static bool CompareCrm(bsv_cuentas crmCuenta, string nroCuenta, Guid moduloId, Guid monedaId, Guid tipoOperacionId)
        {
            return crmCuenta.bsv_numero_de_cuenta == nroCuenta && crmCuenta.bsv_operacion_moneda.Id == monedaId && crmCuenta.bsv_operacion_mdoulo.Id == moduloId && crmCuenta.bsv_operacion_tipo_operacion.Id == tipoOperacionId;
        }
    }
}
