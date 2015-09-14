using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FDDSE.ConsoleClient.Properties;

namespace FDDSE.ConsoleClient.Models
{
    class FdcpFileService : IFdcpFileService
    {
        private string  dskC, dskD;
        private byte[] dskAfileBytes, dskBfileBytes;
        private int mountedDrives = 0;
        enum Drives { DskA = 1, DskB = 2, DskC = 4, DskD = 8};
        public FdcpFileService()
        {
            if (!String.IsNullOrEmpty(Settings.Default.DskA))
            {
                // mountedDrives = mountedDrives | (byte)Convert.ToInt32("00000001", 2);
                mountedDrives = mountedDrives | (byte)Drives.DskA;
                //dskA = Path.Combine(Environment.CurrentDirectory, Settings.Default.DskA);
                dskAfileBytes = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskA));
            }

            if (! String.IsNullOrEmpty(Settings.Default.DskB))
            {
                mountedDrives = mountedDrives | (byte)Convert.ToInt32("00000010", 2);
                //dskB = Path.Combine(Environment.CurrentDirectory, Settings.Default.DskB);
                dskBfileBytes = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskB));
            }

            Debug.WriteLine(mountedDrives.ToString("X4"));

            if (Settings.Default.DskB != null)
                Console.WriteLine("DskB is null");
        }
        
        public byte[] ReadDiskDataByte(int drive, int track, int bytes)
        {
            /* exceprt from serial protocol.txt
            ; TRANSFER OF TRACK DATA
            ; Track data is sent as a sequence of bytes followed by a 16 bit, little endian
            ; checksum.Note the Transfer Length field does NOT include the two bytes of
            ; the checksum. The following notes apply to both the FDC and the server.
            */

            // string dskA = AppDomain.CurrentDomain.BaseDirectory + @"/Data/cpm48k.dsk";
            // string dskA = Environment.CurrentDirectory + @"/Data/cpm48k.dsk";
            //string dskA = Environment.CurrentDirectory + @"/Data/cpm48k.dsk";
            //byte[] dskAfileBytes = File.ReadAllBytes(dskA);

            //BlockCopy(Array src, int srcOffset, Array dst, int dstOffset, int count)

            byte[] returnBytes = new byte[bytes];
            Buffer.BlockCopy(dskAfileBytes, track * 4384, returnBytes, 0, bytes);
            return returnBytes;
        }

        public bool WriteDiskDataByte(int drive, int track, int transferLength, byte[] bytes)
        {
            byte[] saveDiskBytes = dskAfileBytes;
            try
            {
                // Buffer.BlockCopy(bytes, 0, dskAfileBytes, track * 4384, transferLength);
                Buffer.BlockCopy(bytes, 0, dskAfileBytes, track * transferLength, transferLength);
                File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskB), dskAfileBytes);
                return true;
            }
            catch (Exception)
            {
                dskAfileBytes = saveDiskBytes;  // restore previous state of disk if write failure.
                return false;
            }
        }

        public int MountedDrives
        {
            get { return mountedDrives; }
            private set { mountedDrives = value; }
        }

    }
}