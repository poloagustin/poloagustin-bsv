using Supervielle.DataAccess.Sql.Interfaces;
using Supervielle.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Sql
{
    public class RelacionCuentaPersonaDao : GenericDao<RelacionCuentaPersona>, IRelacionCuentaPersonaDao
    {
        protected override string tableName
        {
            get { return "sta_crm_dcl_rel_cuenta_persona"; }
        }
    }
}
