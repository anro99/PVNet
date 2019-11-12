using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler.Sml
{
    internal class SmlTransport
    {
        private SmlFile m_smlFile;
        private int m_nbFillBytes;
        private ushort m_messageCheckSum;


        public SmlTransport()
        {
            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
        }

        private const byte ESCAPE_BYTE = 0x1b;
        private const byte START_BYTE = 0x01;
        private const byte STOP_BYTE = 0x1a;
        private const byte FILL_BYTE = 0x00;

        private enum ParserState
        {
            WaitForStartMessageFirstEscapeByte,
            WaitForStartMessageSecondEscapeByte,
            WaitForStartMessageThirdEscapeByte,
            WaitForStartMessageFourthEscapeByte,
            WaitForStartMessageFirstStartByte,
            WaitForStartMessageSecondStartByte,
            WaitForStartMessageThirdStartByte,
            WaitForStartMessageFourthStartByte,

            WaitForMessageByte,

            WaitForSecondEscapeByteInMessage,
            WaitForThirdEscapeByteInMessage,
            WaitForFourthEscapeByteInMessage,
            WaitForFirstEscapedByte,
            WaitForSecondEscapedEscapeByte,
            WaitForThirdEscapedEscapeByte,
            WaitForFourthEscapedEscapeByte,

            WaitForFirstFillByteOrFirstEscapeByte,
            WaitForSecondFillByteOrFirstEscapeByte,
            WaitForThirdFillByteOrFirstEscapeByte,

            WaitForEndMessageSecondEscapeByte,
            WaitForEndMessageThirdEscapeByte,
            WaitForEndMessageFourthEscapeByte,

            WaitForStopByte,
            WaitForNbFillBytes,
            WaitForFirstCheckSumByte,
            WaitForSecondCheckSumByte
        }

        private ParserState m_parserState;

        public void Parse(Byte a_newByte)
        {
            switch (m_parserState)
            {
                case ParserState.WaitForStartMessageFirstEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForStartMessageSecondEscapeByte;
                    break;
                case ParserState.WaitForStartMessageSecondEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForStartMessageThirdEscapeByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParserState.WaitForStartMessageThirdEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForStartMessageFourthEscapeByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParserState.WaitForStartMessageFourthEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForStartMessageFirstStartByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParserState.WaitForStartMessageFirstStartByte:
                    if (a_newByte == START_BYTE)
                        m_parserState = ParserState.WaitForStartMessageSecondStartByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParserState.WaitForStartMessageSecondStartByte:
                    if (a_newByte == START_BYTE)
                        m_parserState = ParserState.WaitForStartMessageThirdStartByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParserState.WaitForStartMessageThirdStartByte:
                    if (a_newByte == START_BYTE)
                        m_parserState = ParserState.WaitForStartMessageFourthStartByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParserState.WaitForStartMessageFourthStartByte:
                    if (a_newByte == START_BYTE)
                    {
                        m_parserState = ParserState.WaitForMessageByte;
                        m_smlFile = new SmlFile();
                        m_smlFile.BeginPopulate();
                    }
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;

                case ParserState.WaitForMessageByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForSecondEscapeByteInMessage;
                    else
                        HandleBytesForFile(0, a_newByte);
                    break;

                case ParserState.WaitForSecondEscapeByteInMessage:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForThirdEscapeByteInMessage;
                    else
                        HandleBytesForFile(1, a_newByte);
                    break;
                case ParserState.WaitForThirdEscapeByteInMessage:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForFourthEscapeByteInMessage;
                    else
                        HandleBytesForFile(2, a_newByte);
                    break;
                case ParserState.WaitForFourthEscapeByteInMessage:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForFirstEscapedByte;
                    else
                        HandleBytesForFile(3, a_newByte);
                    break;
                case ParserState.WaitForFirstEscapedByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForSecondEscapedEscapeByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParserState.WaitForSecondEscapedEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForThirdEscapedEscapeByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParserState.WaitForThirdEscapedEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForFourthEscapedEscapeByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParserState.WaitForFourthEscapedEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        HandleBytesForFile(3, ESCAPE_BYTE);
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;

                case ParserState.WaitForFirstFillByteOrFirstEscapeByte:
                    switch (a_newByte)
                    {
                        case FILL_BYTE:
                            m_parserState = ParserState.WaitForSecondFillByteOrFirstEscapeByte;
                            break;
                        case ESCAPE_BYTE:
                            m_parserState = ParserState.WaitForEndMessageSecondEscapeByte;
                            break;
                        default:
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                            break;
                    }
                    break;

                case ParserState.WaitForSecondFillByteOrFirstEscapeByte:
                    switch (a_newByte)
                    {
                        case FILL_BYTE:
                            m_parserState = ParserState.WaitForThirdFillByteOrFirstEscapeByte;
                            break;
                        case ESCAPE_BYTE:
                            m_parserState = ParserState.WaitForEndMessageSecondEscapeByte;
                            break;
                        default:
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                            break;
                    }
                    break;

                case ParserState.WaitForThirdFillByteOrFirstEscapeByte:
                    switch (a_newByte)
                    {
                        case FILL_BYTE:
                            m_parserState = ParserState.WaitForThirdFillByteOrFirstEscapeByte;
                            break;
                        case ESCAPE_BYTE:
                            m_parserState = ParserState.WaitForEndMessageSecondEscapeByte;
                            break;
                        default:
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                            break;
                    }
                    break;

                case ParserState.WaitForEndMessageSecondEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForEndMessageThirdEscapeByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;

                case ParserState.WaitForEndMessageThirdEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForEndMessageFourthEscapeByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;

                case ParserState.WaitForEndMessageFourthEscapeByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForStopByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;

                case ParserState.WaitForStopByte:
                    if (a_newByte == STOP_BYTE)
                        m_parserState = ParserState.WaitForNbFillBytes;
                    else if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForStartMessageSecondEscapeByte;
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;


                case ParserState.WaitForNbFillBytes:
                    switch (a_newByte)
                    {
                        case 0x01:
                        case 0x02:
                        case 0x03:
                            m_nbFillBytes = a_newByte;
                            m_parserState = ParserState.WaitForFirstCheckSumByte;
                            break;
                        case ESCAPE_BYTE:
                            m_parserState = ParserState.WaitForStartMessageSecondEscapeByte;
                            break;
                        default:
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                            break;
                    }
                    break;
                case ParserState.WaitForFirstCheckSumByte:
                    m_messageCheckSum = a_newByte;
                    m_parserState = ParserState.WaitForSecondCheckSumByte;
                    break;
                case ParserState.WaitForSecondCheckSumByte:
                    m_messageCheckSum = (ushort)((m_messageCheckSum << 8) + a_newByte);
                    m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
            }
        }

        private void HandleBytesForFile(int a_nbEscapeBytes, byte a_byte)
        {
            for(int i = 0; i > a_nbEscapeBytes; i++)
            {
                switch (m_smlFile.ContinuePopulate(ESCAPE_BYTE))
                {
                    case ParseResult.Failed:
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        for (; i > a_nbEscapeBytes; i++)
                            Parse(ESCAPE_BYTE);
                        Parse(a_byte);
                        return;
                    case ParseResult.Done:
                        m_parserState = ParserState.WaitForFirstFillByteOrFirstEscapeByte;
                        for (; i > a_nbEscapeBytes; i++)
                            Parse(ESCAPE_BYTE);
                        Parse(a_byte);
                        return;
                    default:
                        break;
                }
            }

            switch (m_smlFile.ContinuePopulate(a_byte))
            {
                case ParseResult.Failed:
                    m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;
                case ParseResult.Done:
                    m_parserState = ParserState.WaitForFirstFillByteOrFirstEscapeByte;
                    break;
                default:
                    m_parserState = ParserState.WaitForMessageByte;
                    break;
            }
        }

    }
}
