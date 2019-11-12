using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal abstract class SmlSimpleValue<T> : SmlBase
    { 
        private SmlTypeLengthField m_tl = new SmlTypeLengthField();
        private T m_value = default(T);
        private int m_nbBytesRead;

        public SmlSimpleValue()
        {
            m_state = State.WaitForTL;
        }

        protected SmlSimpleValue(SmlTypeLengthField a_typeLengthField)
        {
            if (a_typeLengthField != null)
            {
                m_tl = a_typeLengthField;
                if (a_typeLengthField.Type != CorespondingSmlType)
                    m_state = State.Failed;
                else
                {
                    m_value = InitialValue;
                    m_nbBytesRead = 0;
                    m_state = State.WaitForValueByte;
                }
            }
            else
                m_state = State.WaitForTL;
        }

        protected abstract SmlFieldType CorespondingSmlType { get; }
        protected abstract T InitialValue { get;  }
        protected abstract T AddNextByte(T a_currentValue, byte a_nextByte);

        public T Value => m_value;

        protected int ValueLength => m_tl.ValueLength;

        private enum State
        {
            Done,
            Failed,
            WaitForTL,
            WaitForValueByte,
        }

        private State m_state;

        public ParseResult Parse(byte a_byte)
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
                            if (m_tl.Type == SmlFieldType.Optional)
                            {
                                m_value = default(T);
                                m_state = State.Done;
                                return ParseResult.Done;
                            }

                            if (m_tl.Type == CorespondingSmlType)
                            {
                                    m_value = InitialValue;
                                    m_nbBytesRead = 0;
                                    m_state = State.WaitForValueByte;
                                    return ParseResult.MoreBytesNeeded;
                            }
                            m_state = State.Failed;
                            return ParseResult.Failed;

                        default:
                            m_state = State.Failed;
                            return ParseResult.Failed;
                    }

                case State.WaitForValueByte:
                    m_value = AddNextByte(m_value, a_byte);
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

        public override ParseResult BeginPopulate()
        {
            return ParseResult.MoreBytesNeeded;
        }

        public override ParseResult ContinuePopulate(byte a_byte)
        {
            if (m_state == State.Done || m_state == State.Failed || m_state == State.WaitForTL)
                return ParseResult.Failed;

            return Parse(a_byte);
        }

        public override SmlBase EndPopulate()
        {
            return m_state == State.Done ? this : null;
        }
    }
}

