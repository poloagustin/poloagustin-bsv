using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015.CustomAttributes
{
    /// <summary>
    /// This attribute is used for determine that a DTO will be converted to the given Type 
    /// by setting all properties that have a CustomCrmDtoPropertyAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class CustomCrmDtoAttribute : Attribute
    {
        /// <param name="entityName">The name of the root entity</param>
        public CustomCrmDtoAttribute(string entityName)
        {
            this.entityName = entityName;
        }

        private string entityName;

        public string EntityName
        {
            get { return entityName; }
            set { entityName = value; }
        }

    }
}
