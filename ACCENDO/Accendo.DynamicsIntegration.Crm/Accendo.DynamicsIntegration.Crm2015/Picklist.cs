using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015
{
    [Serializable]
    public class Picklist
    {
        public int Value { get; set; }
        public string[] Labels { get; set; }

        public Picklist()
        {
        }

        public Picklist(int value)
        {
            this.Value = value;
        }
    }
}
