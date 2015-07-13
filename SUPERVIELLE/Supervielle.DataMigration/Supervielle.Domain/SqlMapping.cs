using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.Domain
{
    [DataContract]
    public class SqlMapping
    {
        [DataMember(Name="table")]
        public string Table { get; set; }
        [DataMember(Name="entity")]
        public string Entity { get; set; }
        [DataMember(Name="attributes")]
        public List<Attribute> Attributes { get; set; }
    }
}
