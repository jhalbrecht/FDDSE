using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FDDSE.ConsoleClient.Properties;

namespace FDDSE.ConsoleClient.Models
{
    internal class FdcpFileService : IFdcpFileService
    {
        // private string dskC, dskD;
        static byte[] dskAfileBytes, dskBfileBytes, dskCfileBytes, dskDfileBytes;
        //private byte[] drivesFileBytes = {dskAfileBytes}; //TODO learn to use an array of arrays in place of the switch statements.
        private int mountedDrives = 0;

        private enum Drives
        {
            DskA = 1,
            DskB = 2,
            DskC = 4,
            DskD = 8
        };

        // private string[] StringDrives = {Settings.Default.DskA, Settings.Default.DskB};

        public FdcpFileService()
        {
            // foreach (var drive in { Drives.DskA, Drives.DskB, Drives.DskC, Drives.DskD} )
            //foreach (var value in StringDrives)
            //{
            //    Console.WriteLine("wondering {0}", value);
            //}

            // DskA
            if (!String.IsNullOrEmpty(Settings.Default.DskA))
            {
                // mountedDrives = mountedDrives | (byte)Convert.ToInt32("00000001", 2);
                mountedDrives = mountedDrives | (byte)Drives.DskA;
                //dskA = Path.Combine(Environment.CurrentDirectory, Settings.Default.DskA);
                dskAfileBytes = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskA));
                Console.WriteLine("DskA: {0}", Settings.Default.DskA);
            }
            else
            {
                Console.WriteLine("Must have DskA to boot. Specifiy on command line and use -p to persist options.");
                Environment.Exit(0);
            }

            // DskB
            if (!String.IsNullOrEmpty(Settings.Default.DskB))
            {
                mountedDrives = mountedDrives | (byte)Convert.ToInt32("00000010", 2);
                //dskB = Path.Combine(Environment.CurrentDirectory, Settings.Default.DskB);
                dskBfileBytes = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskB));
                Console.WriteLine("DskB: {0}", Settings.Default.DskB);
            }
            else
            {
                Console.WriteLine("DskB: {0}", "is null");
            }

            // DskC
            if (!String.IsNullOrEmpty(Settings.Default.DskC))
            {
                mountedDrives = mountedDrives | (byte)Drives.DskC;
                mountedDrives = mountedDrives | (byte)Convert.ToInt32("00000100", 2);
                //dskB = Path.Combine(Environment.CurrentDirectory, Settings.Default.DskB);
                dskCfileBytes = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskC));
                Console.WriteLine("DskC: {0}", Settings.Default.DskC);
            }
            else
            {
                Console.WriteLine("DskC: {0}", "is null");
            }

            // DskD
            if (!String.IsNullOrEmpty(Settings.Default.DskD))
            {
                mountedDrives = mountedDrives | (byte)Drives.DskD;
                mountedDrives = mountedDrives | (byte)Convert.ToInt32("00001000", 2);
                //dskB = Path.Combine(Environment.CurrentDirectory, Settings.Default.DskB);
                dskDfileBytes = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskD));
                Console.WriteLine("DskD: {0}", Settings.Default.DskD);
            }
            else
            {
                Console.WriteLine("DskD: {0}", "is null");
            }
            Debug.WriteLine("mountedDrives: {0}", mountedDrives.ToString("X4"));
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

            // TODO this may work, but I'd like to access an array of arrays and itterate through with drive ie: arrayOfArrays[drive] rather than these two switch statements.

            byte[] returnBytes = new byte[bytes];
            switch (drive)
            {
                case 0:
                    Buffer.BlockCopy(dskAfileBytes, track * 4384, returnBytes, 0, bytes);
                    break;
                case 1:
                    Buffer.BlockCopy(dskBfileBytes, track * 4384, returnBytes, 0, bytes);
                    break;
                case 2:
                    Buffer.BlockCopy(dskCfileBytes, track * 4384, returnBytes, 0, bytes);
                    break;
                case 3:
                    Buffer.BlockCopy(dskDfileBytes, track * 4384, returnBytes, 0, bytes);
                    break;
            }

            //byte[] returnBytes = new byte[bytes];
            //Buffer.BlockCopy(dskAfileBytes, track * 4384, returnBytes, 0, bytes);
            //return returnBytes;

            return returnBytes;
        }

        public bool WriteDiskDataByte(int drive, int track, int transferLength, byte[] bytes)
        {
            bool success = false;
            byte[] saveDiskBytes;

            switch (drive)
            {
                case 0:
                    saveDiskBytes = dskAfileBytes;
                    try
                    {
                        Buffer.BlockCopy(bytes, 0, dskAfileBytes, track * transferLength, transferLength);
                        File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskA), dskAfileBytes);
                        success = true;
                    }
                    catch (Exception)
                    {
                        dskAfileBytes = saveDiskBytes;  // restore previous state of disk if write failure.
                        success = false;
                    }
                    break;

                case 1:
                    saveDiskBytes = dskBfileBytes;
                    try
                    {
                        Buffer.BlockCopy(bytes, 0, dskBfileBytes, track * transferLength, transferLength);
                        File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskB), dskBfileBytes);
                        success = true;
                    }
                    catch (Exception)
                    {
                        dskBfileBytes = saveDiskBytes;  // restore previous state of disk if write failure.
                        success = false;
                    }
                    break;

                case 2:
                    saveDiskBytes = dskCfileBytes;
                    try
                    {
                        Buffer.BlockCopy(bytes, 0, dskCfileBytes, track * transferLength, transferLength);
                        File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskC), dskCfileBytes);
                        success = true;
                    }
                    catch (Exception)
                    {
                        dskCfileBytes = saveDiskBytes;  // restore previous state of disk if write failure.
                        success = false;
                    }
                    break;

                case 3:
                    saveDiskBytes = dskDfileBytes;
                    try
                    {
                        Buffer.BlockCopy(bytes, 0, dskDfileBytes, track * transferLength, transferLength);
                        File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskD), dskDfileBytes);
                        success = true;
                    }
                    catch (Exception)
                    {
                        dskDfileBytes = saveDiskBytes;  // restore previous state of disk if write failure.
                        success = false;
                    }
                    break;
            }

            return success;

            //byte[] saveDiskBytes = dskAfileBytes;
            //try
            //{
            //    // Buffer.BlockCopy(bytes, 0, dskAfileBytes, track * 4384, transferLength);
            //    Buffer.BlockCopy(bytes, 0, dskAfileBytes, track * transferLength, transferLength);
            //    File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, Settings.Default.DskA), dskAfileBytes);
            //    return true;
            //}
            //catch (Exception)
            //{
            //    dskAfileBytes = saveDiskBytes;  // restore previous state of disk if write failure.
            //    return false;
            //}
        }

        public int MountedDrives
        {
            get { return mountedDrives; }
            private set { mountedDrives = value; }
        }

    }
}