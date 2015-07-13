using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.Exceptions
{
    public class ExecuteMultipleException : Exception
    {
        public List<OrganizationServiceFault> Faults { get; set; }

        public ExecuteMultipleException(List<OrganizationServiceFault> faults)
            : base(GetMessage(faults))
        {
            this.Faults = faults;
        }

        private static string GetMessage(List<OrganizationServiceFault> faults)
        {
            var builder = new StringBuilder();
            builder.Append("Ha ocurrido un error. Lea la propiedad Faults para analizar el detalle.");
            return builder.ToString();
        }
    }
}
