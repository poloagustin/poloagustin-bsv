using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class CustomCrmDtoPropertyAttribute : Attribute
    {
        public CustomCrmDtoPropertyAttribute(string attributeName, bool isAliasedValue = false)
        {
            this.attributeName = attributeName;
            this.isAliasedValue = isAliasedValue;
        }

        private string attributeName;

        public string AttributeName
        {
            get { return attributeName; }
            set { attributeName = value; }
        }

        private bool isAliasedValue;

        public bool IsAliasedValue
        {
            get { return isAliasedValue; }
            set { isAliasedValue = value; }
        }

    }
}
