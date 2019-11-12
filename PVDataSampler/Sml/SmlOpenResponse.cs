using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlOpenResponse : SmlMessageBody
    {
        SmlTypeLengthField m_tl = new SmlTypeLengthField();
        SmlString m_codePage;
        Encoding m_encoding;
        SmlString m_clientId;
        SmlString m_requestFileId;
        SmlString m_serverId;
        SmlTime m_time;
        SmlUnsigned8 m_version;

        private SmlOpenResponse( SmlString a_codePage, SmlString a_clientId, SmlString a_requestFileId, SmlString a_serverId, SmlUnsigned8 a_version)
        {
            m_codePage = a_codePage;
            if (m_codePage != null)
                m_encoding = Encoding.GetEncoding(m_codePage.ValueString);
            else
                m_encoding = null;
            m_clientId = m_encoding != null ? new SmlString(a_clientId.ValueBytes, m_encoding) : a_clientId;
            m_requestFileId = m_encoding != null ? new SmlString(a_requestFileId.ValueBytes, m_encoding) : a_requestFileId;
            m_serverId = m_encoding != null ? new SmlString(a_serverId.ValueBytes, m_encoding) : a_serverId;
            m_time = null;
            m_version = a_version;
        }

        internal static SmlMessageBody Create(SmlBase a_baseNode)
        {
            var list = a_baseNode as SmlList;
            if (list == null || list.Length != 6)
                return null;

            var codePage = list.GetElement(0) as SmlString;
            if (codePage == null && !list.GetElement(0).IsOptional)
                return null;
            var encoding = codePage != null ? Encoding.GetEncoding(codePage.ValueString) : null;

            var clientId = list.GetElement(1) as SmlString;
            if (clientId == null && !list.GetElement(1).IsOptional)
                return null;

            var requestFileId = list.GetElement(2) as SmlString;
            if (requestFileId == null)
                return null;

            var serverId = list.GetElement(3) as SmlString;
            if (serverId == null)
                return null;

            var refTime = list.GetElement(4);

            var version = list.GetElement(5) as SmlUnsigned8;
            if (version == null && !list.GetElement(1).IsOptional)
                return null;

            return new SmlOpenResponse(codePage, clientId, requestFileId, serverId, version);
        }


        public Encoding Encoding => m_encoding;
        public string ClientId => m_clientId?.ValueString;
        public byte[] ClientIdBytes => m_clientId?.ValueBytes;
        public string RequestFileId => m_requestFileId?.ValueString;
        public byte[] RequestFileIdBytes => m_requestFileId?.ValueBytes;
        public string ServerId => m_serverId?.ValueString;
        public byte[] ServerIdBytes => m_serverId?.ValueBytes;


    }
}
