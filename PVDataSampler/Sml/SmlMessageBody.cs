using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlMessageBody : SmlBase
    {
        SmlTypeLengthField m_tl = new SmlTypeLengthField();
        SmlUnsigned32 m_choice;
        SmlBase m_message;

        public SmlMessageBody()
        {
            m_state = State.WaitForTl;
        }

        private enum State
        {
            Done,
            Failed,
            WaitForTl,
            WaitForChoice,
            WaitForMessage,
        }

        private State m_state;

        public override ParseResult Parse(byte a_byte)
        {
            switch (m_state)
            {
                case State.Done:
                    m_state = State.Failed;
                    return ParseResult.Failed;

                case State.WaitForTl:
                    switch (m_tl.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            switch (m_tl.Type)
                            {
                                case SmlType.List:
                                    if (m_tl.NbListElements != 2)
                                    {
                                        m_state = State.Failed;
                                        return ParseResult.Failed;
                                    }
                                    m_choice = new SmlUnsigned32();
                                    m_state = State.WaitForChoice;
                                    return ParseResult.MoreBytesNeeded;

                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForChoice:
                    switch (m_choice.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_choice.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            switch(m_choice.Value)
                            {
                                case 0x00000101:  //PuplicOpenResponse
                                    m_message = new SmlPublicOpenResponse();
                                    break;
                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }
                            m_state = State.WaitForMessage;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForMessage:
                    switch (m_message.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            m_state = State.Done;
                            return ParseResult.Done;

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
