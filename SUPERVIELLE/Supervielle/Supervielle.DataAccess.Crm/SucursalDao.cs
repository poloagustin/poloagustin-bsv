﻿using Accendo.DynamicsIntegration.Crm2015;
using Supervielle.DataAccess.Crm.Interfaces;
using Supervielle.Domain.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Crm
{
    public class SucursalDao : MasterDataDao<bsv_sucursales>, ISucursalDao
    {
        protected override string idAttribute
        {
            get { return "bsv_sucursalesid"; }
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
