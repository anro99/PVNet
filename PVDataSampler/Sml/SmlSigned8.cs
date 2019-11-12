using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlSigned8 : SmlSimpleValue<sbyte?>
    {
        public SmlSigned8()
            : base()
        {
        }

        public SmlSigned8(SmlTypeLengthField a_smlTypeLengthField)
            : base(a_smlTypeLengthField)
        {
        }

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.Signed8;

        protected override sbyte? InitialValue => 0;

        protected override sbyte? AddNextByte(sbyte? a_currentValue, byte a_nextByte)
        {
            return (sbyte?)(a_nextByte);
        }
    }
}
