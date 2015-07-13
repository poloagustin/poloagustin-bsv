using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class VirtualAttribute : Attribute
    {

        public string Name { get; set; }

        public VirtualAttribute(string name)
        {
            this.Name = name;
        }
    }
}
