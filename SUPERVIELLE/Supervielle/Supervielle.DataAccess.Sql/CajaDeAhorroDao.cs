using Supervielle.DataAccess.Sql.Interfaces;
using Supervielle.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Sql
{
    public class CajaDeAhorroDao : GenericDao<CajaDeAhorro>, ICajaDeAhorroDao
    {
        protected override string tableName
        {
            get { return "sta_crm_dca_caja_ahorro"; }
        }
    }
}
