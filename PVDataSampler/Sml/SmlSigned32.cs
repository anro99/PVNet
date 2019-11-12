using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlSigned32 : SmlSimpleValue<Int32?>
    {
        public SmlSigned32()
            : base()
        {
        }

        public SmlSigned32(SmlTypeLengthField a_smlTypeLengthField)
            : base(a_smlTypeLengthField)
        {
        }

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.Signed32;

        protected override Int32? InitialValue => 0;

        protected override Int32? AddNextByte(Int32? a_currentValue, byte a_nextByte)
        {
            return (Int32?)((a_currentValue << 8) | a_nextByte);
        }
    }
}
