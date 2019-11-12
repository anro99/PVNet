using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlMessageBody
    {
        private SmlBase m_listNode;

        protected SmlMessageBody()
        {
            m_listNode = null;
        }

        protected SmlMessageBody(SmlBase a_baseNode)
        {
            m_listNode = a_baseNode;
        }

        internal static SmlMessageBody Create(SmlMessageType a_type, SmlBase a_baseNode)
        {
            switch(a_type)
            {
                case SmlMessageType.OpenResponse:
                    return SmlOpenResponse.Create(a_baseNode);
                case SmlMessageType.CloseResponse:
                    return SmlCloseResponse.Create(a_baseNode);
            }

            return new SmlMessageBody(a_baseNode);
        }
    }
}
