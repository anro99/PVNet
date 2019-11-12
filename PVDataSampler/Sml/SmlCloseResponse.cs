using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlCloseResponse : SmlMessageBody
    {
        private SmlBase m_signature;

        private SmlCloseResponse(SmlBase a_signature)
            : base()
        {
            m_signature = a_signature;
        }

        internal static SmlMessageBody Create(SmlBase a_baseNode)
        {
            var list = a_baseNode as SmlList;
            if (list == null || list.Length != 1)
                return null;

            var signature = list.GetElement(0);
            return new SmlCloseResponse(signature);
        }
    }
}
