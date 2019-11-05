using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlString : SmlBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static Encoding DefaultEncoding;

        private Encoding m_encoding;
        private SmlTypeLengthField m_tl = new SmlTypeLengthField();
        private int m_nbRead;
        private string m_string;
        private byte[] m_bytes;

        static SmlString()
        {
            DefaultEncoding = Encoding.GetEncoding("iso-8859-15");
        }

        public SmlString(Encoding a_encoding)
            : this()
        {
            if (a_encoding != null)
                m_encoding = a_encoding;
        }

        public SmlString()
        {
            m_encoding = DefaultEncoding;
            m_nbRead = 0;
            m_state = State.WaitForTL;
        }

        public string Value => m_string;

        private enum State
        {
            Done,
            Failed,
            WaitForTL,
            WaitForStringByte,
        }

        private State m_state;


        public override ParseResult Parse(byte a_byte)
        {
            try
            {
                switch (m_state)
                {
                    case State.Done:
                        m_state = State.Failed;
                        return ParseResult.Failed;

                    case State.WaitForTL:
                        switch(m_tl.Parse(a_byte))
                        {
                            case ParseResult.MoreBytesNeeded:
                                return ParseResult.MoreBytesNeeded;
                            case ParseResult.Done:
                                switch(m_tl.Type)
                                {
                                    case SmlType.Optional:
                                        m_string = null;
                                        m_state = State.Done;
                                        return ParseResult.Done;
                                    case SmlType.String:
                                        m_nbRead = 0;
                                        m_bytes = new byte[m_tl.ValueLength];
                                        m_state = State.WaitForStringByte;
                                        return ParseResult.MoreBytesNeeded;
                                    default:
                                        m_state = State.Failed;
                                        return ParseResult.Failed;
                                }
                            default:
                                m_state = State.Failed;
                                return ParseResult.Failed;
                        }

                    case State.WaitForStringByte:
                        m_bytes[m_nbRead] = a_byte;
                        m_nbRead++;
                        if (m_nbRead == m_tl.ValueLength)
                        {
                            m_string = m_encoding.GetString(m_bytes);
                            m_bytes = null;
                            m_state = State.Done;
                            return ParseResult.Done;
                        }
                        return ParseResult.MoreBytesNeeded;

                    default:
                        return ParseResult.Failed;
                }
            }
            catch ( Exception ex)
            {
                logger.Error(ex);
                m_state = State.Failed;
                return ParseResult.Failed;
            }
        }
    }
}
