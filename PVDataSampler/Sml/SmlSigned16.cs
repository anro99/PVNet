using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlSigned16 : SmlBase
    {
        private SmlTypeLengthField m_tl = new SmlTypeLengthField();
        private Int16? m_value = null;
        private int m_nbBytesRead;

        public SmlSigned16()
        {
            m_state = State.WaitForTL;
        }

        public Int16? Value => m_value;

        private enum State
        {
            Done,
            Failed,
            WaitForTL,
            WaitForValueByte,
        }

        private State m_state;


        public override ParseResult Parse(byte a_byte)
        {
            switch (m_state)
            {
                case State.Done:
                    m_state = State.Failed;
                    return ParseResult.Failed;

                case State.WaitForTL:
                    switch (m_tl.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            switch (m_tl.Type)
                            {
                                case SmlType.Optional:
                                    m_value = null;
                                    m_state = State.Done;
                                    return ParseResult.Done;
                                case SmlType.Signed16:
                                    m_value = 0;
                                    m_nbBytesRead = 0;
                                    m_state = State.WaitForValueByte;
                                    return ParseResult.MoreBytesNeeded;
                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }
                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForValueByte:
                    m_value = (Int16)((m_value << 8) | a_byte);
                    m_nbBytesRead++;
                    if (m_nbBytesRead == m_tl.ValueLength)
                    {
                        m_state = State.Done;
                        return ParseResult.Done;
                    }
                    return ParseResult.MoreBytesNeeded;


                default:
                    return ParseResult.Failed;
            }
        }
    }
}

