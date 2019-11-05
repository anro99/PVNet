using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlPublicOpenResponse : SmlBase
    {
        SmlTypeLengthField m_tl = new SmlTypeLengthField();
        SmlString m_codePage;
        Encoding m_encoding;
        SmlString m_clientId;
        SmlString m_requestFileId;
        SmlString m_serverId;
        SmlTime m_time;
        SmlUnsigned8 m_version;

        public SmlPublicOpenResponse()
        {
            m_state = State.WaitForTl;
        }

        public Encoding Encoding => m_encoding;
        public string ClientId => m_clientId?.Value;
        public string RequestFileId => m_requestFileId?.Value;
        public string ServerId => m_serverId?.Value;

        private enum State
        {
            Done,
            Failed,

            WaitForTl,
            WaitForCodePage,
            WaitForClientId,
            WaitForRequestFileId,
            WaitForServerId,
            WaitForTime,
            WaitForVersion,
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
                                    if (m_tl.NbListElements != 6)
                                    {
                                        m_state = State.Failed;
                                        return ParseResult.Failed;
                                    }
                                    m_codePage = new SmlString();
                                    m_state = State.WaitForCodePage;
                                    return ParseResult.MoreBytesNeeded;

                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForCodePage:
                    switch (m_codePage.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_codePage.Value != null)
                                m_encoding = Encoding.GetEncoding(m_codePage.Value);
                            m_clientId = new SmlString(m_encoding);
                            m_state = State.WaitForClientId;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForClientId:
                    switch (m_clientId.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            m_requestFileId = new SmlString(m_encoding);
                            m_state = State.WaitForRequestFileId;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForRequestFileId:
                    switch (m_requestFileId.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_requestFileId.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_serverId = new SmlString(m_encoding);
                            m_state = State.WaitForServerId;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForServerId:
                    switch (m_serverId.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_serverId.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_time = new SmlTime();
                            m_state = State.WaitForTime;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForTime:
                    switch (m_time.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            m_version = new SmlUnsigned8();
                            m_state = State.WaitForVersion;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForVersion:
                    switch (m_version.Parse(a_byte))
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
