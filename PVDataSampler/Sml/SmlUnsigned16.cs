using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlUnsigned16 : SmlSimpleValue<UInt16>
    {
        public SmlUnsigned16(SmlTypeLengthField a_smlTypeLengthField)
            : base(a_smlTypeLengthField)
        {
        }

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.Unsigned16;

        protected override UInt16 InitialValue => 0;

        protected override UInt16 AddNextByte(UInt16 a_currentValue, byte a_nextByte)
        {
            return (UInt16)((a_currentValue << 8) | a_nextByte);
        }
    }
}

