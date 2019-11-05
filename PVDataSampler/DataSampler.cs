using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVDataSampler
{
    internal class DataSampler : IDisposable
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private bool m_started = false;

        private SmartMeterSml m_gridMeter = null;

        public DataSampler()
        {

        }

        public bool Started 
        {
            get 
            {
                return m_started;
            }
            private set 
            {
                m_started = value;
                StartedChanged?.Invoke(this, new EventArgs());
            } 
        }
        public event EventHandler StartedChanged;

        public void Start()
        {
            try
            {
                if (m_gridMeter != null)
                    return;
                m_gridMeter = new SmartMeterSml(AppSettings.Instance.GridMeter);
                m_gridMeter.Start();
                Started = true;
            }
            catch (Exception ex)
            {
                m_gridMeter = null;
                logger.Error(ex);
            }
        }

        public void Stop()
        {
            try
            {
                m_gridMeter?.Stop();
                m_gridMeter = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            Started = false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Stop();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DataSampler()
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
