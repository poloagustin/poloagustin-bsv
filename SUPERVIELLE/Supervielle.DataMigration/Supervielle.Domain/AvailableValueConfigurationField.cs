using Supervielle.DataMigration.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Supervielle.DataMigration.Domain
{
    [DataContract]
    public class AvailableValueConfigurationField
    {
        [DataMember(Name = "originColumn")]
        public string OriginColumn { get; set; }
        [DataMember(Name = "originType")]
        public OriginType OriginType { get; set; }
        [DataMember(Name = "destinationAttribute")]
        public string DestinationAttribute { get; set; }
        [DataMember(Name = "destinationType")]
        public DestinationType DestinationType { get; set; }
        [DataMember(Name = "isKey")]
        public bool IsKey { get; set; }
    }
}
