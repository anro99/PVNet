using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlUnsigned64 : SmlSimpleValue<UInt64>
    {
        public SmlUnsigned64(SmlTypeLengthField a_smlTypeLengthField)
            : base(a_smlTypeLengthField)
        {
        }

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.Unsigned64;

        protected override UInt64 InitialValue => 0;

        protected override UInt64 AddNextByte(UInt64 a_currentValue, byte a_nextByte)
        {
            return (UInt64)((a_currentValue << 8) | a_nextByte);
        }
    }
}
