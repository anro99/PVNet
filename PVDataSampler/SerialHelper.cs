using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace PVDataSampler
{
    internal class SerialHelper
    {
        internal static string[] GetSerialPorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}