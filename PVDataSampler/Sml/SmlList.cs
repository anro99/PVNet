using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlList : SmlBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        SmlTypeLengthField m_tl = new SmlTypeLengthField();
        Encoding m_encoding;
        SmlBase[] m_elements;
        SmlBase m_actElement;
        int m_nbElementsRead;

        public SmlList(SmlTypeLengthField a_typeLengthField, Encoding a_encoding)
        {
            m_state = State.WaitForNextListElement;
            m_encoding = a_encoding;

            if (a_typeLengthField == null)
                throw (new ArgumentNullException());

            m_tl = a_typeLengthField;
            if (a_typeLengthField.Type != SmlFieldType.List)
                m_state = State.Failed;
            else
            {
                m_nbElementsRead = 0;
                m_actElement = new SmlTypeLengthField(m_encoding);
                m_actElement.BeginPopulate();
                m_elements = new SmlBase[m_tl.NbListElements];
                m_state = State.WaitForNextListElement;
            }
        }

        public int Length => m_elements == null ? 0 : m_elements.Length;

        public SmlBase GetElement(int a_idx)
        {
            return m_elements == null ? null : m_elements[a_idx];
        }



        private enum State
        {
            Done,
            Failed,
            WaitForNextListElement,
        }

        private State m_state;


        public override ParseResult BeginPopulate()
        {
            switch(m_state)
            {
                case State.Failed:
                    return ParseResult.Failed;

                case State.Done:
                    return ParseResult.Done;

                default:
                    return ParseResult.MoreBytesNeeded;
            }
        }

        public override ParseResult ContinuePopulate(byte a_byte)
        {
            try
            {
                switch (m_state)
                {
                    case State.Done:
                        m_state = State.Failed;
                        return ParseResult.Failed;

                    case State.WaitForNextListElement:
                        var res = m_actElement.ContinuePopulate(a_byte);
                        switch(res)
                        {
                            case ParseResult.Failed:
                                m_elements = null;
                                m_state = State.Failed;
                                return ParseResult.Failed;
                            case ParseResult.Done:
                                m_elements[m_nbElementsRead] = m_actElement.EndPopulate();
                                m_nbElementsRead++;
                                if (m_tl.NbListElements == m_nbElementsRead)
                                {
                                    m_state = State.Done;
                                    return ParseResult.Done;
                                }
                                m_actElement = new SmlTypeLengthField(m_encoding);
                                m_actElement.BeginPopulate();
                                return ParseResult.MoreBytesNeeded;

                            default:
                                return res;
                        }

                    default:
                        return ParseResult.Failed;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                m_state = State.Failed;
                return ParseResult.Failed;
            }
        }

        public override SmlBase EndPopulate()
        {
            return m_state == State.Done ? this : null;
        }

    }
}
