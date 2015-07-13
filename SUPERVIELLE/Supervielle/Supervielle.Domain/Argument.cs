using Supervielle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.Domain
{
    [DataContract]
    public class Argument
    {
        [DataMember(Name = "argumentOption")]
        public string ArgumentOption { get; set; }
        [DataMember(Name="argumentType")]
        public ArgumentType ArgumentType { get; set; }
        [DataMember(Name="availableValues")]
        public List<AvailableValue> AvailableValues { get; set; }

        public Argument()
        {
            this.AvailableValues = new List<AvailableValue>();
        }

        public Argument(string argumentOption, ArgumentType argumentType)
        {
            this.ArgumentOption = argumentOption;
            this.ArgumentType = argumentType;
            this.AvailableValues = new List<AvailableValue>();
        }
    }
}
