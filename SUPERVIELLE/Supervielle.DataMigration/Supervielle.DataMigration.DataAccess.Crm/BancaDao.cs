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
    public class BancaDao : MasterDataDao<bsv_banca>, IBancaDao
    {
        protected override string idAttribute
        {
            get { return "bsv_bancaid"; }
        }

        protected override string nameAttribute
        {
            get { return "bsv_codigo"; }
        }

        protected override string codeAttribute
        {
            get { return "bsv_codigo"; }
        }
    }
}
