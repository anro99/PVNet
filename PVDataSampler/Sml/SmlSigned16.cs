using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlSigned16 : SmlSimpleValue<Int16?>
    {
        public SmlSigned16()
            : base()
        {
        }

        public SmlSigned16(SmlTypeLengthField a_smlTypeLengthField)
            : base(a_smlTypeLengthField)
        {
        }

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.Signed16;

        protected override Int16? InitialValue => 0;

        protected override Int16? AddNextByte(Int16? a_currentValue, byte a_nextByte)
        {
            return (Int16?)((a_currentValue << 8) | a_nextByte);
        }
    }
}