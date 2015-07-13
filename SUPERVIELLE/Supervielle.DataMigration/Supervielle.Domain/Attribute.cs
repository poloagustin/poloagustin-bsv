using Supervielle.DataMigration.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Supervielle.DataMigration.Domain
{
    [DataContract]
    public class Attribute
    {
        [DataMember(Name="column")]
        public string Column { get; set; }
        [DataMember(Name = "property")]
        public string Property { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "index")]
        public int Index { get; set; }
        public OriginType OriginType
        {
            get
            {
                return MapOriginType(this.Type);
            }
        }

        private static OriginType MapOriginType(string type)
        {
            switch (type)
            {
                case "Char":
                    return OriginType.Char;
                case "Date":
                    return OriginType.Date;
                case "Decimal":
                    return OriginType.Decimal;
                case "Int32":
                    return OriginType.Int32;
                case "Byte":
                    return OriginType.Byte;
                case "Short":
                    return OriginType.Short;
                case "String":
                default:
                    return OriginType.String;
            }
        }
    }
}
