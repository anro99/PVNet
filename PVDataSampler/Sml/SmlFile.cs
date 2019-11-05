using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlFile : SmlBase
    {
        private List<SmlMessage> m_messages = new List<SmlMessage>();
        SmlMessage m_actMessage = null;

        public SmlFile()
        {
            m_actMessage = new SmlMessage();
            m_state = State.WaitForOpenReponse;
        }

        private enum State
        {
            Done,
            Failed,
            WaitForOpenReponse,
            WaitForNextMessage,
        }

        private State m_state;

        public override ParseResult Parse(byte a_byte)
        {
            switch(m_state)
            {
                case State.Done:
                    m_state = State.Failed;
                    return ParseResult.Failed;

                case State.WaitForOpenReponse:
                    switch(m_actMessage.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;

                        case ParseResult.Done:
                            m_state = State.WaitForNextMessage;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                default:
                    return ParseResult.Failed;
            }
        }
    }
}
