using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enum
{
    public enum League
    {
        [EnumMember(Value = "CHALLENGER")]
        challenger,

        [EnumMember(Value = "GRANDMASTER")]
        grandmaster,

        [EnumMember(Value = "MASTER")]
        master
    }

    public enum Queue
    {
        [EnumMember(Value = "RANKED_SOLO_5x5")]
        RANKED_SOLO_5x5,

        [EnumMember(Value = "RANKED_FLEX_SR")]
        RANKED_FLEX_SR,

        [EnumMember(Value = "RANKED_FLEX_TT")]
        RANKED_FLEX_TT
    }
}