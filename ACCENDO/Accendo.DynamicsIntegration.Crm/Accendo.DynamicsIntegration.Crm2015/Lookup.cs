using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015
{
    [Serializable]
    public class Lookup
    {
        //private global::Tenaris.Fise.Domain.fise_forecast_bu_details historicForecastBuDetail;
        //private string p1;
        //private string p2;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LogicalName { get; set; }

        public Lookup()
        {
        }

        public Lookup(Guid id, string name, string logicalName)
        {
            this.Id = id;
            this.Name = name;
            this.LogicalName = logicalName;
        }

        //public Lookup(global::Tenaris.Fise.Domain.fise_forecast_bu_details historicForecastBuDetail, string p1, string p2)
        //{
        //    // TODO: Complete member initialization
        //    this.historicForecastBuDetail = historicForecastBuDetail;
        //    this.p1 = p1;
        //    this.p2 = p2;
        //}
    }
}
