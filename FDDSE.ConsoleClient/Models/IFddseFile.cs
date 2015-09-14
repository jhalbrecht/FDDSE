using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDDSE.ConsoleClient.Models
{
    public interface IFddseFile
    {
        List<string> EnumerateSerailPorts();
    }
}
