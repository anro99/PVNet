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
        SmlOpenResponse m_openResponse;
        Encoding m_encoding;

        public SmlFile()
        {
        }

        private enum State
        {
            Done,
            Failed,
            WaitForOpenReponse,
            WaitForNextMessage,
        }

        private State m_state;

        public override ParseResult BeginPopulate()
        {
            m_state = State.WaitForOpenReponse;
            m_actMessage = new SmlMessage();
            return m_actMessage.BeginPopulate();
        }

        public override ParseResult ContinuePopulate(byte a_byte)
        {
            switch (m_state)
            {
                case State.Done:
                    m_state = State.Failed;
                    return ParseResult.Failed;

                case State.WaitForOpenReponse:
                    switch (m_actMessage.ContinuePopulate(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;

                        case ParseResult.Done:
                            m_messages.Add(m_actMessage.EndPopulate() as SmlMessage);
                            if (m_actMessage.MessageType != SmlMessageType.OpenResponse ||
                                (null==(m_openResponse = m_actMessage.MessageBody as SmlOpenResponse)))
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_encoding = m_openResponse.Encoding;

                            m_actMessage = new SmlMessage(m_encoding);
                            switch(m_actMessage.BeginPopulate())
                            {
                                case ParseResult.MoreBytesNeeded:
                                    m_state = State.WaitForNextMessage;
                                    return ParseResult.MoreBytesNeeded;

                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForNextMessage:
                    switch (m_actMessage.ContinuePopulate(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;

                        case ParseResult.Done:
                            m_messages.Add(m_actMessage.EndPopulate() as SmlMessage);
                            if (m_actMessage.MessageType == SmlMessageType.CloseResponse)
                            {
                                m_state = State.Done;
                                return ParseResult.Done;
                            }

                            m_actMessage = new SmlMessage();
                            switch (m_actMessage.BeginPopulate())
                            {
                                case ParseResult.MoreBytesNeeded:
                                    m_state = State.WaitForNextMessage;
                                    return ParseResult.MoreBytesNeeded;

                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }


                default:
                    return ParseResult.Failed;
            }
        }

        public override SmlBase EndPopulate()
        {
            return m_state == State.Done ? this : null;
        }
    }
}
