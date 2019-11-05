using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlTime : SmlBase
    {
        private SmlTypeLengthField m_tl = new SmlTypeLengthField();
        private SmlUnsigned8 m_choice;
        private SmlUnsigned32 m_unsigned32;
        private SmlSigned16 m_offset;
        private SmlSigned16 m_seasonalOffset;


        public SmlTime()
        {
            m_state = State.WaitForTL;
        }

        public (DateTime? a_date,TimeSpan? a_secondIndex)? Value
        {
            get
            {
                if (m_tl.Type == SmlType.Optional || m_state != State.Done)
                    return null;
                switch(m_choice.Value)
                {
                    case 0x00:
                        return (null, TimeSpan.FromSeconds(m_unsigned32.Value.GetValueOrDefault()));
                    case 0x01:
                        return (new DateTime(1970, 1, 1).AddSeconds(m_unsigned32.Value.GetValueOrDefault()), null);
                    case 0x02:
                        var date = new DateTime(1970, 1, 1).AddSeconds(m_unsigned32.Value.GetValueOrDefault());
                        date = date.AddMinutes(m_offset.Value.GetValueOrDefault() + m_seasonalOffset.Value.GetValueOrDefault());
                        return (date, null);
                    default:
                        return null;
                }
            }
        }

        private enum State
        {
            Done,
            Failed,
            WaitForTL,
            WaitForChoice,
            WaitForUint32,
            WaitForLocalTimeTl,
            WaitForLocalTimeTimestamp,
            WaitForLocalTimeOffset,
            WaitForLocalTimeSeasonalTimeOffset,
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
                                    m_state = State.Done;
                                    return ParseResult.Done;
                                case SmlType.List:
                                    if (m_tl.NbListElements != 2)
                                    {
                                        m_state = State.Failed;
                                        return ParseResult.Failed;
                                    }
                                    m_choice = new SmlUnsigned8();
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
                            switch (m_choice.Value)
                            {
                                case 0x01:
                                case 0x02:
                                    m_unsigned32 = new SmlUnsigned32();
                                    m_state = State.WaitForUint32;
                                    break;
                                case 0x03:
                                    m_tl = new SmlTypeLengthField();
                                    m_state = State.WaitForLocalTimeTl;
                                    break;
                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForUint32:
                    switch (m_choice.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_unsigned32.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_state = State.Done;
                            return ParseResult.Done;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForLocalTimeTl:
                    switch (m_tl.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            switch (m_tl.Type)
                            {
                                case SmlType.Optional:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                                case SmlType.List:
                                    if (m_tl.NbListElements != 3)
                                    {
                                        m_state = State.Failed;
                                        return ParseResult.Failed;
                                    }
                                    m_unsigned32 = new SmlUnsigned32();
                                    m_state = State.WaitForLocalTimeTimestamp;
                                    return ParseResult.MoreBytesNeeded;
                                default:
                                    m_state = State.Failed;
                                    return ParseResult.Failed;
                            }
                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForLocalTimeTimestamp:
                    switch (m_unsigned32.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_unsigned32.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_offset = new SmlSigned16();
                            m_state = State.WaitForLocalTimeOffset;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForLocalTimeOffset:
                    switch (m_offset.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_unsigned32.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
                            m_seasonalOffset = new SmlSigned16();
                            m_state = State.WaitForLocalTimeSeasonalTimeOffset;
                            return ParseResult.MoreBytesNeeded;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForLocalTimeSeasonalTimeOffset:
                    switch (m_seasonalOffset.Parse(a_byte))
                    {
                        case ParseResult.MoreBytesNeeded:
                            return ParseResult.MoreBytesNeeded;
                        case ParseResult.Done:
                            if (m_unsigned32.Value == null)
                            {
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            }
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
