using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal enum SmlMessageType : UInt32
    {
        Unknown = 0,
        OpenRequest = 0x00000100,
        OpenResponse = 0x00000101,
        CloseRequest = 0x00000200,
        CloseResponse = 0x00000201,
        GetProfilePackRequest = 0x00000300,
        GetProfilePackResponse = 0x00000301,
        GetProfileListRequest = 0x00000400,
        GetProfileListResponse = 0x00000401,
        GetProcParameterRequest = 0x00000500,
        GetProcParameterResponse = 0x00000501,
        SetProcParameterRequest = 0x00000600,
        SetProcParameterResponse = 0x00000601,
        GetListRequest = 0x00000700,
        GetListResponse = 0x00000701,
        GetCosemRequest = 0x00000800,
        GetCosemResponse = 0x00000801,
        SetCosemRequest = 0x00000900,
        SetCosemResponse = 0x00000901,
        ActionCosemRequest = 0x00000A00,
        ActionCosemResponse = 0x00000A01,
        AttentionResponse = 0x0000FF01
    }


    internal class SmlMessage : SmlBase
    {
        private SmlTypeLengthField m_tl = new SmlTypeLengthField();
        private SmlString m_transactionId;
        private SmlUnsigned8 m_groupNo;
        private SmlUnsigned8 m_abortOnError;
        private SmlMessageType m_messageType = SmlMessageType.Unknown;
        private SmlMessageBody m_body;
        private SmlUnsigned16 m_crc;
        private Encoding m_encoding;


        public SmlMessage()
            : this(null)
        {
        }

        public SmlMessage(Encoding a_encoding)
        {
            m_state = State.Wait;
            m_encoding = a_encoding;
        }

        public byte[] TransactionIdBytes => m_transactionId?.ValueBytes;
        public string TransactionIdString => m_transactionId?.ValueString;
        public int GroupNo => m_groupNo == null ? 0 : (int)(m_groupNo.Value);
        public int AbortOnError => m_abortOnError == null ? 0 : (int)(m_abortOnError.Value);
        public SmlMessageType MessageType => m_messageType;
        public SmlMessageBody MessageBody => m_body;



        private enum State
        {
            Done,
            Failed,
            Wait,
        }

        private State m_state;

        public override ParseResult BeginPopulate()
        {
            m_tl = new SmlTypeLengthField(m_encoding);
            m_state = State.Wait;
            return m_tl.BeginPopulate();
        }

        private ParseResult EndWithFailed()
        {
            m_state = State.Failed;
            return ParseResult.Failed;
        }

        public override ParseResult ContinuePopulate(byte a_byte)
        {
            var ret = m_tl.ContinuePopulate(a_byte);
            if (ret == ParseResult.MoreBytesNeeded)
                return ParseResult.MoreBytesNeeded;

            if (ret == ParseResult.Failed)
                return EndWithFailed();
                
            SmlList list = m_tl.EndPopulate() as SmlList;
            if (list == null || list.Length != 6)
                return EndWithFailed();

            m_transactionId = list.GetElement(0) as SmlString;
            if (m_transactionId == null)
                return EndWithFailed();

            m_groupNo = list.GetElement(1) as SmlUnsigned8;
            if (m_groupNo == null)
                return EndWithFailed();

            m_abortOnError = list.GetElement(2) as SmlUnsigned8;
            if (m_abortOnError == null)
                return EndWithFailed();

            var body = list.GetElement(3) as SmlList;
            if (body == null || body.Length != 2)
                return EndWithFailed();
            var type = body.GetElement(0) as SmlUnsigned32;
            if (type == null || !Enum.IsDefined(typeof(SmlMessageType), type.Value))
                return EndWithFailed();
            m_messageType = (SmlMessageType)type.Value;
            m_body = SmlMessageBody.Create(m_messageType, body.GetElement(1));
            if (m_body == null)
                return EndWithFailed();

            m_crc = list.GetElement(4) as SmlUnsigned16;
            if (m_crc == null)
                return EndWithFailed();

            var eom = list.GetElement(5) as SmlTypeLengthField;
            if (eom == null || eom.Type != SmlFieldType.EndOfMessage)
                return EndWithFailed();

            m_state = State.Done;
            return ParseResult.Done;
        }

        public override SmlBase EndPopulate()
        {
            return m_state == State.Done ? this : null;
        }
    }
}
