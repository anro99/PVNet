using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlMessage : SmlBase
    {
        SmlTypeLengthField m_tl = new SmlTypeLengthField();
        SmlString m_transactionId;
        SmlUnsigned8 m_groupNo;
        SmlUnsigned8 m_abortOnError;
        SmlMessageBody m_body;
        SmlUnsigned16 m_crc;

        public SmlMessage()
        {
            m_state = State.WaitForTl;
        }


        private enum State
        {
            Done,
            Failed,
            WaitForTl,
            WaitForTransactionId,
            WaitForGroupNo,
            WaitForAbortOnError,
            WaitForMessageBody,
            WaitForCrc,
            WaitForEndOfMessage,
        }

        private State m_state;

        public override ParseResult Parse(byte a_byte)
        {
            switch(m_state)
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
                            switch(m_tl.Type)
                            {
                                case SmlType.List:
                                    if (m_tl.NbListElements != 6)
                                    {
                                        m_state = State.Failed;
                                        return ParseResult.Failed;
                                    }
                                    m_transactionId = new SmlString();
                                    m_state = State.WaitForTransactionId;
                                    return ParseResult.MoreBytesNeeded;

                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForTransactionId:
                    switch (m_transactionId.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_transactionId.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_groupNo = new SmlUnsigned8();
                            m_state = State.WaitForGroupNo;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForGroupNo:
                    switch (m_groupNo.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_groupNo.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_abortOnError = new SmlUnsigned8();
                            m_state = State.WaitForAbortOnError;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForAbortOnError:
                    switch (m_abortOnError.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_abortOnError.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_body = new SmlMessageBody();
                            m_state = State.WaitForMessageBody;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForMessageBody:
                    switch (m_body.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            m_crc = new SmlUnsigned16();
                            m_state = State.WaitForCrc;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForCrc:
                    switch (m_crc.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_abortOnError.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_state = State.WaitForEndOfMessage;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForEndOfMessage:
                    if (a_byte != 0x00)
                    {
                        m_state = State.Failed;
                        return ParseResult.Failed;
                    }
                    m_state = State.Done;
                    return ParseResult.Done;

                default:
                    return ParseResult.Failed;
            }
        }
    }
}
