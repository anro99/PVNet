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
        public virtual bool IsOptional => false;

        public abstract ParseResult BeginPopulate();

        public abstract ParseResult ContinuePopulate(byte a_byte);

        public abstract SmlBase EndPopulate();
    }
}
