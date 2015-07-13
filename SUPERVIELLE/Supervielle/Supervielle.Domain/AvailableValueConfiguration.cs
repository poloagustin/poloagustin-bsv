using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Supervielle.Domain
{
    [DataContract]
    public class AvailableValueConfiguration
    {
        [DataMember(Name = "originTable")]
        public string OriginTable { get; set; }
        [DataMember(Name = "destinationEntity")]
        public string DestinationEntity { get; set; }
        [DataMember(Name = "fields")]
        public List<AvailableValueConfigurationField> Fields { get; set; }
    }
}
