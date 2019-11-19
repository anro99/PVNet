using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlString : SmlSimpleValue<byte[]>
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static Encoding DefaultEncoding;

        private Encoding m_encoding;
        private SmlTypeLengthField m_tl = new SmlTypeLengthField();
        private int m_nbAdded;
        private string m_string;
        private byte[] m_bytes;

        static SmlString()
        {
            DefaultEncoding = Encoding.GetEncoding("iso-8859-15");
        }

        public SmlString(SmlTypeLengthField a_typeLengthField, Encoding a_encoding)
            : base(a_typeLengthField)
        {
            if (a_encoding != null)
                m_encoding = a_encoding;
            else
                m_encoding = DefaultEncoding;
            m_string = null;
            m_bytes = null;
        }

        public SmlString(byte[] a_bytes, Encoding a_encoding)
        {
            m_bytes = a_bytes;
            m_string = null;
            if (a_encoding != null)
                m_encoding = a_encoding;
            else
                m_encoding = DefaultEncoding;
        }

        public string ValueString 
        {
            get
            {
                if (m_bytes == null)
                    return null;
                if (m_string != null)
                    m_string = m_encoding.GetString(m_bytes);
                return m_string;
            }
        }

        public byte[] ValueBytes => m_bytes;

        protected override SmlFieldType CorespondingSmlType => SmlFieldType.String;

        protected override byte[] InitialValue 
        {
            get
            {
                m_nbAdded = 0;
                return new byte[ValueLength];
            }
        }


        protected override byte[] AddNextByte(byte[] a_currentValue, byte a_nextByte)
        {
            a_currentValue[m_nbAdded] = a_nextByte;
            m_nbAdded++;
            m_bytes = a_currentValue;
            return a_currentValue;
        }
    }
}
