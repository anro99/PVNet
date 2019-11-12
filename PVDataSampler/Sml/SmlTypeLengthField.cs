using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal enum SmlFieldType
    {
        EndOfMessage,
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
        private SmlFieldType m_type = SmlFieldType.Unknown;
        private int m_length = 0;
        private int m_nbTlBytes = 0;
        private SmlBase m_parseResult = null;
        private Encoding m_encoding;

        private static SmlTypeLengthField s_optional = new SmlTypeLengthField() { m_type = SmlFieldType.Optional, m_state = State.Done, m_length = 1, m_nbTlBytes = 1};
        private static SmlTypeLengthField s_eom = new SmlTypeLengthField() { m_type = SmlFieldType.EndOfMessage, m_state = State.Done, m_length = 1, m_nbTlBytes = 1 };

        public SmlFieldType Type => m_type;
        public int FieldLength => m_length;
        public int ValueLength => m_length - m_nbTlBytes;
        public int NbListElements => m_length;

        public SmlTypeLengthField()
            : this(null)
        {

        }

        public SmlTypeLengthField(Encoding a_encoding)
            : base()
        {
            m_encoding = a_encoding;
        }

        public override bool IsOptional => m_type == SmlFieldType.Optional;


        private enum State
        {
            Done,
            Failed,
            WaitForFirstByte,
            WaitForNextByte
        }

        private State m_state = State.WaitForFirstByte;

        internal ParseResult Parse(byte a_byte)
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
                                m_type = SmlFieldType.Optional;
                            else if (((a_byte & 0x0F) == 0x00) && !moreBytesNeeded)
                                m_type = SmlFieldType.EndOfMessage;
                            else
                                m_type = SmlFieldType.String;
                            break;
                        case 0x40:
                            m_type = SmlFieldType.Boolean;
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
                                    m_type = SmlFieldType.Signed8;
                                    break;
                                case 0x03:
                                    m_type = SmlFieldType.Signed16;
                                    break;
                                case 0x05:
                                    m_type = SmlFieldType.Signed32;
                                    break;
                                case 0x09:
                                    m_type = SmlFieldType.Signed64;
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
                                    m_type = SmlFieldType.Unsigned8;
                                    break;
                                case 0x03:
                                    m_type = SmlFieldType.Unsigned16;
                                    break;
                                case 0x05:
                                    m_type = SmlFieldType.Unsigned32;
                                    break;
                                case 0x09:
                                    m_type = SmlFieldType.Unsigned64;
                                    break;
                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }
                            break;
                        case 0x70:
                            m_type = SmlFieldType.List;
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

        public override ParseResult BeginPopulate()
        {
            m_parseResult = null;
            return ParseResult.MoreBytesNeeded;
        }

        public override ParseResult ContinuePopulate(byte a_byte)
        {
            ParseResult res;
            if (m_parseResult == null)
            {
                res = Parse(a_byte);
                if (res != ParseResult.Done)
                    return res;

                switch (m_type)
                {
                    case SmlFieldType.EndOfMessage:
                        m_parseResult = s_eom;
                        return ParseResult.Done;

                    case SmlFieldType.Optional:
                        m_parseResult = s_optional;
                        return ParseResult.Done;

                    case SmlFieldType.Boolean:
                        m_parseResult = new SmlBool(this);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.Signed8:
                        m_parseResult = new SmlSigned8(this);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.Signed16:
                        m_parseResult = new SmlSigned16(this);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.Signed32:
                        m_parseResult = new SmlSigned32(this);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.Signed64:
                        m_parseResult = new SmlSigned64(this);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.Unsigned8:
                        m_parseResult = new SmlUnsigned8(this);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.Unsigned16:
                        m_parseResult = new SmlUnsigned16(this);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.Unsigned32:
                        m_parseResult = new SmlUnsigned32(this);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.Unsigned64:
                        m_parseResult = new SmlUnsigned64(this);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.String:
                        m_parseResult = new SmlString(this, m_encoding);
                        res = m_parseResult.BeginPopulate();
                        break;

                    case SmlFieldType.List:
                        m_parseResult = new SmlList(this, m_encoding);
                        res = m_parseResult.BeginPopulate();
                        break;
                }
            }
            else
            {
                res = m_parseResult.ContinuePopulate(a_byte);
            }
            return res;
        }

        public override SmlBase EndPopulate()
        {
            return m_parseResult;
        }
    }
}
