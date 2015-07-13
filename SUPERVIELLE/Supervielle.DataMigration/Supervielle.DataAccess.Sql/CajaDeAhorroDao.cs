using Supervielle.DataMigration.DataAccess.Sql.Interfaces;
using Supervielle.DataMigration.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.DataAccess.Sql
{
    public class CajaDeAhorroDao : GenericDao<CajaDeAhorro>, ICajaDeAhorroDao
    {
        protected override string tableName
        {
            get { return "sta_crm_dca_caja_ahorro"; }
        }
    }
}
