using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal enum SmlType
    {
        Optional,
        String,
        Boolean,
        Signed8,
        Signed16,
        Signed32,
        Signed64,
        Unsigned8,
        Unsigned16,
        Unsigned32,
        Unsigned64,
        List,
        Unknown
    }

    internal class SmlTypeLengthField : SmlBase
    {
        private SmlType m_type = SmlType.Unknown;
        private int m_length = 0;
        private int m_nbTlBytes = 0;

        public SmlType Type => m_type;
        public int FieldLength => m_length;
        public int ValueLength => m_length - m_nbTlBytes;
        public int NbListElements => m_length;


        private enum State
        {
            Done,
            Failed,
            WaitForFirstByte,
            WaitForNextByte
        }

        private State m_state = State.WaitForFirstByte;

        public override ParseResult Parse(byte a_byte)
        {
            bool moreBytesNeeded;
            switch(m_state)
            {
                case State.WaitForFirstByte:
                    moreBytesNeeded = (a_byte & 0x80) == 0x80;
                    switch (a_byte & 0x70)
                    {
                        case 0x00:
                            if (((a_byte & 0x0F) == 0x01) && !moreBytesNeeded)
                                m_type = SmlType.Optional;
                            else
                                m_type = SmlType.String;
                            break;
                        case 0x40:
                            m_type = SmlType.Boolean;
                            if (moreBytesNeeded || ((a_byte & 0x0F) != 0x02))
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            break;
                        case 0x50:
                            if (moreBytesNeeded)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            switch (a_byte & 0x0F)
                            {
                                case 0x02:
                                    m_type = SmlType.Signed8;
                                    break;
                                case 0x03:
                                    m_type = SmlType.Signed16;
                                    break;
                                case 0x05:
                                    m_type = SmlType.Signed32;
                                    break;
                                case 0x09:
                                    m_type = SmlType.Signed64;
                                    break;
                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }
                            break;
                        case 0x60:
                            if (moreBytesNeeded)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            switch (a_byte & 0x0F)
                            {
                                case 0x02:
                                    m_type = SmlType.Unsigned8;
                                    break;
                                case 0x03:
                                    m_type = SmlType.Unsigned16;
                                    break;
                                case 0x05:
                                    m_type = SmlType.Unsigned32;
                                    break;
                                case 0x09:
                                    m_type = SmlType.Unsigned64;
                                    break;
                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }
                            break;
                        case 0x70:
                            m_type = SmlType.List;
                            break;
                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }
                    m_nbTlBytes = 1;
                    m_length = (a_byte & 0x0F);
                    m_state = moreBytesNeeded ? State.WaitForNextByte : State.Done;
                    return moreBytesNeeded ? ParseResult.MoreBytesNeeded : ParseResult.Done;

                case State.WaitForNextByte:
                    moreBytesNeeded = (a_byte & 0x80) == 0x80;
                    switch (a_byte & 0x70)
                    {
                        case 0x00:
                            m_nbTlBytes++;
                            m_length = (m_length << 4) | (a_byte & 0x0F);
                            m_state = moreBytesNeeded ? State.WaitForNextByte : State.Done;
                            return moreBytesNeeded ? ParseResult.MoreBytesNeeded : ParseResult.Done;
                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                default:
                    m_state = State.Failed;
                    return ParseResult.Failed;
            }
        }
    }
}
