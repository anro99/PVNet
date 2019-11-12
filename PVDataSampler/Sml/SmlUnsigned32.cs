using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlUnsigned32 : SmlSimpleValue<UInt32>
    {
        public SmlUnsigned32(SmlTypeLengthField a_smlTypeLengthField)
            : base(a_smlTypeLengthField)
        {
        }

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.Unsigned32;

        protected override UInt32 InitialValue => 0;

        protected override UInt32 AddNextByte(UInt32 a_currentValue, byte a_nextByte)
        {
            return (UInt32)((a_currentValue << 8) | a_nextByte);
        }
    }
}

