using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDDSE.ConsoleClient.Models
{
    public interface IUtility
    {
        short CalculateChecksumOfByteArray(byte[] buffer, int bytes);
        string SayString(string what);

        string Version();
    }
}
