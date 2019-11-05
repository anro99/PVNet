using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVDataSampler.Sml;

namespace PVDataSampler
{
    internal class SmartMeterSml : IDisposable
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private string m_portName;
        private SerialPort m_port;
        private SmlTransport m_transport = null;

        public SmartMeterSml( string a_port)
        {
            m_portName = a_port;
        }

        public void Start()
        {
            try
            {
                m_port = new SerialPort(m_portName, 9600, Parity.None, 8, StopBits.One);
                m_port.Handshake = Handshake.None;
                m_port.RtsEnable = false;
                m_port.ReceivedBytesThreshold = 1;
                m_port.DataReceived += DataReceived;
                m_transport = new SmlTransport();
                m_port.Open();
            }
            catch( Exception ex)
            {
                m_port.DataReceived -= DataReceived;
                m_port.Dispose();
                m_port = null;
                logger.Error(ex);
            }
        }

        public void Stop()
        {
            try
            {
                if (m_port == null)
                    return;
                m_port.DataReceived -= DataReceived;
                m_port.Close();
                m_port.Dispose();
                m_port = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


        private static int ReadBufferLength = 10;
        private Byte[] m_readBuffer = new Byte[ReadBufferLength];

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int nbRead = 0;
                while(0 != m_port.BytesToRead && m_port.IsOpen)
                {
                    nbRead = m_port.Read(m_readBuffer, 0, ReadBufferLength);
                    for (int i = 0; i < nbRead; i++)
                        m_transport.Parse(m_readBuffer[i]);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }




        #region IDisposable Support
        private bool m_disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool a_disposing)
        {
            if (!m_disposedValue)
            {
                if (a_disposing)
                {
                    Stop();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                m_disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SmartMeterSml()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
