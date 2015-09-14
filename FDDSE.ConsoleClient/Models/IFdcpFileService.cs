using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDDSE.ConsoleClient.Models
{
    public interface IFdcpFileService
    {
        byte[] ReadDiskDataByte(int drive, int track, int bytes);
        bool WriteDiskDataByte(int drive, int track, int transferLength, byte[] bytes);
        int MountedDrives { get; }
    }
}
