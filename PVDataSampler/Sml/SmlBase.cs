using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal enum ParseResult
    {
        MoreBytesNeeded,
        Done,
        Failed
    }


    internal abstract class SmlBase
    {
        public abstract ParseResult Parse(byte a_byte);
    }
}
