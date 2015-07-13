using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Supervielle.Domain
{
    [DataContract]
    public class AvailableValue
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "configuration")]
        public AvailableValueConfiguration Configuration { get; set; }
    }
}
