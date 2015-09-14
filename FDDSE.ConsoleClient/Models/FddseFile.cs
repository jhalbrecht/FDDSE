using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace FDDSE.ConsoleClient.Models
{
    public class FddseFile : IFddseFile
    {
        public List<string> EnumerateSerailPorts()
        {
            return  SerialPort.GetPortNames().ToList();
        }
    }
}