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
                    }
                    else
                        m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;

                case ParserState.WaitForMessageByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForSecondEscapeByteInMessage;
                    else
                        if (ParseResult.Failed == m_smlFile.Parse(a_newByte))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                    break;

                case ParserState.WaitForSecondEscapeByteInMessage:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForThirdEscapeByteInMessage;
                    else
                    {
                        if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else if (ParseResult.Failed == m_smlFile.Parse(a_newByte))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else
                            m_parserState = ParserState.WaitForMessageByte;
                    }
                    break;
                case ParserState.WaitForThirdEscapeByteInMessage:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForFourthEscapeByteInMessage;
                    else
                    {
                        if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else if (ParseResult.Failed == m_smlFile.Parse(a_newByte))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else
                            m_parserState = ParserState.WaitForMessageByte;
                    }
                    break;
                case ParserState.WaitForFourthEscapeByteInMessage:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForFirstEscapedByte;
                    else
                    {
                        if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else if (ParseResult.Failed == m_smlFile.Parse(a_newByte))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else
                            m_parserState = ParserState.WaitForMessageByte;
                    }
                    break;
                case ParserState.WaitForFirstEscapedByte:
                    if (a_newByte == ESCAPE_BYTE)
                        m_parserState = ParserState.WaitForSecondEscapedEscapeByte;
                    else if (a_newByte == STOP_BYTE)
                        m_parserState = ParserState.WaitForNbFillBytes;
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
                    {
                        if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else if (ParseResult.Failed == m_smlFile.Parse(ESCAPE_BYTE))
                            m_parserState = ParserState.WaitForStartMessageFirstEscapeByte;
                        else
                            m_parserState = ParserState.WaitForMessageByte;
                    }
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

    }
}
