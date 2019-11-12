using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlSigned64 : SmlSimpleValue<Int64>
    {
        public SmlSigned64(SmlTypeLengthField a_smlTypeLengthField)
            : base(a_smlTypeLengthField)
        {
        }

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.Signed64;

        protected override Int64 InitialValue => 0;

        protected override Int64 AddNextByte(Int64 a_currentValue, byte a_nextByte)
        {
            return (Int64)((a_currentValue << 8) | a_nextByte);
        }
    }
}
