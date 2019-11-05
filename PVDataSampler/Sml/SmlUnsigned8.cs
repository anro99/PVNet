using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlUnsigned8 : SmlBase
    {
        private SmlTypeLengthField m_tl = new SmlTypeLengthField();
        private byte? m_value = null;

        public SmlUnsigned8()
        {
            m_state = State.WaitForTL;
        }

        public byte? Value => m_value;

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
                                case SmlType.Unsigned8:
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
                    m_value = a_byte;
                    m_state = State.Done;
                    return ParseResult.Done;

                default:
                    return ParseResult.Failed;
            }
        }
    }
}
