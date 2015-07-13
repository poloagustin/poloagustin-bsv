﻿using Supervielle.DataMigration.DataAccess.Sql.Interfaces;
using Supervielle.DataMigration.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.DataAccess.Sql
{
    public class PersonaJuridicaDao : GenericDao<PersonaJuridica>, IPersonaJuridicaDao
    {
        protected override string tableName
        {
            get { return "sta_crm_dcl_persona_juridica"; }
        }
    }
}