using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlUnsigned8 : SmlSimpleValue<byte>
    {
        public SmlUnsigned8(SmlTypeLengthField a_smlTypeLengthField)
            : base(a_smlTypeLengthField)
        {
        }

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.Unsigned8;

        protected override byte InitialValue => 0;

        protected override byte AddNextByte(byte a_currentValue, byte a_nextByte)
        {
            return a_nextByte;
        }
    }
}
