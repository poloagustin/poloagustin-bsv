using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015
{
    [Serializable]
    public class Money
    {
        public decimal Value { get; set; }

        public Money(decimal value)
        {
            this.Value = value;
        }
    }
}
