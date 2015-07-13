using Supervielle.DataMigration.DataAccess.Crm.Interfaces;
using Supervielle.DataMigration.Domain.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.DataAccess.Crm
{
public    class EstadoCuentaDao : MasterDataDao<bsv_estado_de_cuenta>, IEstadoCuentaDao
    {
        protected override string codeAttribute
        {
            get { return "bsv_estado_de_cuentaid"; }
        }

        protected override string idAttribute
        {
            get { return "bsv_name"; }
        }

        protected override string nameAttribute
        {
            get { return "bsv_name"; }
        }
    }
}
