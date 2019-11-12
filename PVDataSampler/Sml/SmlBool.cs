using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlBool : SmlSimpleValue<bool>
    {
        public SmlBool(SmlTypeLengthField a_smlTypeLengthField)
            : base(a_smlTypeLengthField)
        {
        }

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.Boolean;

        protected override bool InitialValue => false;

        protected override bool AddNextByte(bool a_currentValue, byte a_nextByte)
        {
            return a_nextByte != 0x00;
        }
    }
}
