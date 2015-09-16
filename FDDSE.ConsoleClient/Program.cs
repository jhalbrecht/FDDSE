using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FDDSE.ConsoleClient.Models;
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using CommandLine;
using FDDSE.ConsoleClient.Properties;

namespace FDDSE.ConsoleClient
{
    internal class Program
    {
        private static Utility _utility;
        private static FdcPlusSettings _settings;
        private static Options _options;
        private static FdcpFileService _fdcpFileService;
        private static SerialPort _serialPort;

        private static short transferLength;
        private static byte[] responseBytes;

        private static int driveNumber;
        private static int specifiedTrack;
        static bool IsLogging { get; set; }
        static bool IsDebugging { get; set; }

        private static void Main(string[] args)
        {
            IsLogging = true;
            IsDebugging = false;
            // -pCOM6 -s403200 -aData/cpm48k.dsk -o
            // -pCOM4 -s403200 -bData/wordstar.dsk -o
            Console.WriteLine("FDDSE Floppy Disk Drive Serial Emulator \n");
            _options = new Options();

            if (IsLogging)
                Console.WriteLine("Default Port: {0}, Speed: {1}", Settings.Default.Port, Settings.Default.Speed);

            if (CommandLine.Parser.Default.ParseArguments(args, _options))
            {
                if (IsDebugging)
                    Console.WriteLine("Muy Bueno on Command Line Parsing");

                UpdateSettingsFromOptions();
            }
            else
            {
                if (IsDebugging)
                    Console.WriteLine("Bleuch no bueno on command line.");

                Console.WriteLine(_options.GetUsage());
            }

            if (_options.EnumeratePorts == true)
            {
                Console.WriteLine("Available serial ports.");
                foreach (string port in SerialPort.GetPortNames())
                {
                    Console.WriteLine(port);
                }
                // Console.ReadLine();
                Environment.Exit(0);
            }

            Task task = new Task(ReadConsoleInput);
            task.Start();

            _utility = new Utility();
            _settings = new FdcPlusSettings();
            _fdcpFileService = new FdcpFileService();

            Console.WriteLine("After command line parse Port: {0}, Speed: {1}", Settings.Default.Port, Settings.Default.Speed);

            if (Settings.Default.Port == null)
            {
                Console.WriteLine("serial port null");
                Environment.Exit(0);
            }


            try
            {
            _serialPort = new SerialPort(
                Settings.Default.Port,
                Settings.Default.Speed,
                Parity.None, 8, StopBits.One
                );

            _serialPort.ReadBufferSize = 5120;
            // _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                _serialPort.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error opening serial port");
                Console.WriteLine("The Exception is: " + e.ToString());
                Console.WriteLine("\nPerhaps you need to run first time setup. Please review readme.txt\n");
                Environment.Exit(0);
            }
            // Console.ReadKey();
            // _serialPort.Close();
            //}
            //private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
            //{


            while (true)
            {
                int calculatedChecksum = 0;
                int bytesAvailableToRead = 0; // = _serialPort.BytesToRead;
                if (IsDebugging)
                    Console.WriteLine("DataReceivedHandler bytes: {0}", bytesAvailableToRead);

                while (bytesAvailableToRead < 10)
                {
                    bytesAvailableToRead = _serialPort.BytesToRead;
                }
                //if (bytesAvailableToRead == 0)
                //    return;

                // TODO think about it!!!!!!!!!!!!!!!!!!!!!!  bytes = 10; // let's just process one request
                // WRIT = 87 82 73 84
                if (IsDebugging)
                    Debug.WriteLine("bytes: {0}", bytesAvailableToRead);

                byte[] serialPortReadBuffer = new byte[bytesAvailableToRead];
                _serialPort.Read(serialPortReadBuffer, 0, bytesAvailableToRead);
                calculatedChecksum = _utility.CalculateChecksumOfByteArray(serialPortReadBuffer, 8);
                short sentChecksum = BitConverter.ToInt16(serialPortReadBuffer, 8);

                if (IsDebugging)
                    Debug.WriteLine("BitConverter calculatedChecksum: {0}, sentChecksum: {1}", sentChecksum, sentChecksum);

                if (calculatedChecksum == sentChecksum)
                {
                    if (IsDebugging)
                        Debug.WriteLine("Checksum Good.");

                    string stringResult = Encoding.UTF8.GetString(serialPortReadBuffer);

                    int checkSum;
                    switch (stringResult.Substring(0, 4))
                    {
                        case "STAT":

                            #region STAT docs 

                            /*
                        
                        ;  FDC TO SERVER COMMANDS
                                ;    Commands from the FDC to the server are fixed length, ten byte messages. The 
                                ;    first four bytes are a command in ASCII, the remaining six bytes are grouped
                                ;    as three 16 bit words (little endian). The checksum is the 16 bit sum of the
                                ;    first eight bytes of the message.
                                ;
                                ;    Bytes 0-3   Bytes 4-5 as Word   Bytes 6-7 as Word   Bytes 8-9 as Word
                                ;    ---------   -----------------   -----------------   -----------------
                                ;     Command       Parameter 1         Parameter 2           Checksum

                                ;      STAT - Provide and request drive status. The FDC sends the selected drive
                                ;             number and head load status in Parameter 1 and the current track 
                                ;             number in Parameter 2. The Server responds with drive mount status
                                ;             (see below). The LSB of Parameter 1 contains the currently selected
                                ;             drive number or 0xff is no drive is selected. The MSB of parameter 1
                                ;             is zero if the head is loaded, non-zero if not loaded.
                                ;
                                ;             The FDC issues the STAT command about ten times per second so that
                                ;             head status and track number information is updated quickly. The 
                                ;             server may also want to assume the drive is selected, the head is
                                ;             loaded, and update the track number whenever a READ is received.  
                                ;
                                ;  SERVER TO FDC 
                                ;    Reponses from the server to the FDC are fixed length, ten byte messages. The 
                                ;    first four bytes are a response command in ASCII, the remaining six bytes are
                                ;    grouped as three 16 bit words (little endian). The checksum is the 16 bit sum
                                ;    of the first eight bytes of the message.
                                ;
                                ;    Bytes 0-3   Bytes 4-5 as Word   Bytes 6-7 as Word   Bytes 8-9 as Word
                                ;    ---------   -----------------   -----------------   -----------------
                                ;     Command      Response Code        Reponse Data          Checksum
                                ;
                                ;    Commands:
                                ;      STAT - Returns drive status in Response Data with one bit per drive. "1" means a
                                ;             drive image is mounted, "0" means not mounted. Bits 15-0 correspond to
                                ;             drive numbers 15-0. Response code is ignored by the FDC.                                                  
                            */

                            #endregion
                            if (IsDebugging)
                                Console.WriteLine("We've got 'STAT' sentChecksum: {0}", sentChecksum);

                            byte[] response = new byte[10];
                            response = Encoding.ASCII.GetBytes("STAT      ");
                            response[4] = 0;
                            response[5] = 0;
                            response[6] = (byte) _fdcpFileService.MountedDrives;
                            response[7] = (byte) (_fdcpFileService.MountedDrives >> 8);
                            checkSum = _utility.CalculateChecksumOfByteArray(response, 8);
                            if (IsDebugging)
                                Debug.WriteLine(checkSum.ToString(("X4")));

                            response[8] = (byte) checkSum;
                            response[9] = (byte) (checkSum >> 8);

                            try
                            {
                                _serialPort.Write(response, 0, 10);
                                if (IsDebugging)
                                    Console.WriteLine("serial port output");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception: {0}", ex.ToString());
                            }
                            break;

                        case "READ":

                            #region READ docs

                            /* excerpt from serial protocol.txt
                                ;      READ - Read specified track. Parameter 1 contains the drive number in the
                                ;             MSNibble. The lower 12 bits contain the track number. Transfer length
                                ;             length is in Parameter 2 and must be the track length. Also see
                                ;             "Transfer of Track Data" below.

                                ;    Bytes 0-3   Bytes 4-5 as Word   Bytes 6-7 as Word   Bytes 8-9 as Word
                                ;    ---------   -----------------   -----------------   -----------------
                                ;     Command       Parameter 1         Parameter 2           Checksum
                            */

                            #endregion
                            if (IsDebugging)
                                Console.WriteLine("We've got 'READ' sentChecksum: {0}", sentChecksum);

                            transferLength = BitConverter.ToInt16(serialPortReadBuffer, 6);
                            responseBytes = new byte[transferLength + 2];
                            // TODO incomplete. Check drive nibble MSNibble and track 12 bits
                            // Parameter 1 contains the drive number in the MSNibble.
                            driveNumber = (byte)((serialPortReadBuffer[5] >> 4) & 0x0F);
                            // driveNumber = serialPortReadBuffer[5] & 0xf0; //0b11110000;
                            specifiedTrack = serialPortReadBuffer[4];
                            byte[] transferBytes = _fdcpFileService.ReadDiskDataByte(
                                driveNumber, 
                                specifiedTrack, 
                                transferLength);
                            Buffer.BlockCopy(transferBytes, 0, responseBytes, 0, transferLength);
                            checkSum = _utility.CalculateChecksumOfByteArray(responseBytes, transferLength);
                            responseBytes[transferLength + 0] = (byte) checkSum;
                            responseBytes[transferLength + 1] = (byte) (checkSum >> 8);

                            try
                            {
                                _serialPort.Write(responseBytes, 0, transferLength + 2);
                                if (IsDebugging)
                                    Console.WriteLine("serial port output from READ");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception: {0}", ex.ToString());
                            }

                            break;

                        case "WRIT": // 87 82 73 84
                            if (IsDebugging)
                                Debug.WriteLine("We've got 'WRIT' sentChecksum: {0}", sentChecksum);

                            #region WRIT docs

                            /*
                        ;      WRIT - Write specified track. Parameter 1 contains the drive number in the
                        ;             MSNibble. The lower 12 bits contain the track number. Transfer length
                        ;             must be track length. Server responds with WRIT response when ready
                        ;             for the FDC to send the track of data. See "Transfer of Track Data" below.
                        */


                            /*
                        ;      WRIT - Issued in repsonse to a WRIT command from the FDC. This response is
                        ;             used to tell the FDC that the server is ready to accept continuous transfer
                        ;             of a full track of data (response code word set to "OK."**) TODO add right paren** If the request
                        ;             can't be fulfilled (e.g., specified drive not mounted), the reponse code
                        ;             is set to NOT READY. The Response Data word is don't care.

                        ;    Reponse Code:
                        ;      0x0000 - OK
                        ;      0x0001 - Not Ready (e.g., write request to unmounted drive)
                        ;      0x0002 - Checksum error (e.g., on the block of write data)
                        ;      0x0003 - Write error (e.g., write to disk failed)
                        */

                            #endregion

                            //responseBytes = new byte[transferLength + 2];
                            transferLength = BitConverter.ToInt16(serialPortReadBuffer, 6);
                            //driveNumber = serialPortReadBuffer[5] & 0xf0; //0b11110000;
                            driveNumber = (byte)((serialPortReadBuffer[5] >> 4) & 0x0F);
                            specifiedTrack = serialPortReadBuffer[4];
                            if (IsDebugging)
                                Debug.WriteLine("transferLength: {0}, driveNumber: {1}, specifiedTrack: {2}", transferLength,
                                driveNumber, specifiedTrack);

                            responseBytes = new byte[10];
                            responseBytes = Encoding.ASCII.GetBytes("WRIT      ");
                            responseBytes[4] = 0x0000; // response code OK
                            responseBytes[5] = 0x0000;

                            responseBytes[6] = 0; // not used.
                            responseBytes[7] = 0;
                            //responseBytes[8] = 0;
                            //responseBytes[9] = 0;

                            checkSum = _utility.CalculateChecksumOfByteArray(responseBytes, 8);
                            if (IsDebugging)
                                Debug.WriteLine("WRIT ack from server checkSum: {0}", checkSum.ToString(("X4")));

                            responseBytes[8] = (byte) checkSum;
                            responseBytes[9] = (byte) (checkSum >> 8);

                            try
                            {
                                _serialPort.Write(responseBytes, 0, 10);
                                if (IsDebugging)
                                    Console.WriteLine("serial port output WRIT OK");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception: {0}", ex.ToString());
                            }

                            _serialPort.DiscardInBuffer();
                            // Thread.Sleep(150);
                            Thread.Sleep(300);

                            // 4384
                            int bytesToRead = _serialPort.BytesToRead;
                            if (IsDebugging)
                                Debug.WriteLine("WRIT track received; bytesToRead: {0}", bytesToRead);

                            byte[] writReceivedBuffer = new byte[_serialPort.BytesToRead];
                            _serialPort.Read(writReceivedBuffer, 0, _serialPort.BytesToRead);
                            int writSentChecksum = BitConverter.ToInt16(writReceivedBuffer, transferLength);
                            checkSum = _utility.CalculateChecksumOfByteArray(writReceivedBuffer, transferLength);
                            short shortChecksum = _utility.CalculateChecksumOfByteArray(writReceivedBuffer,
                                transferLength);
                            if (IsDebugging)
                                Debug.WriteLine("writSentChecksum: {0}, checkSum: {1}, shortChecksum: {2}", writSentChecksum,
                                checkSum, shortChecksum);

                            if (writSentChecksum == shortChecksum)

                                // B6 65 = 182 101. 26038=65b6 681398=a 65 b6

                                File.WriteAllBytes(
                                    Path.Combine(Environment.CurrentDirectory, "Data/WriteReceivedBuffer.hex"),
                                    writReceivedBuffer);

                            // File.WriteAllBytes(string path, byte[] bytes)

                            // public int Read(byte[] buffer,int offset,int count)
                            // public bool boolrsp = _fdcpFileService.WriteDiskDataByte(int drive, int track, int transferLength, byte[] bytes)
                            bool well = _fdcpFileService.WriteDiskDataByte(driveNumber, specifiedTrack, transferLength, writReceivedBuffer);

                            if (well)
                            {
                                // send nifty
                                if (IsDebugging)
                                    Console.WriteLine("We've got a good writ send WSTA");

                                byte[] wstaResponse = new byte[10];
                                wstaResponse = Encoding.ASCII.GetBytes("WSTA      ");
                                wstaResponse[4] = 0; // OK
                                wstaResponse[5] = 0;
                                wstaResponse[6] = 0; // Response Data - don't care
                                wstaResponse[7] = 0;
                                checkSum = _utility.CalculateChecksumOfByteArray(wstaResponse, 8);
                                wstaResponse[8] = (byte)checkSum;
                                wstaResponse[9] = (byte)(checkSum >> 8);

                                try
                                {
                                    _serialPort.Write(wstaResponse, 0, 10);
                                    if (IsDebugging)
                                        Console.WriteLine("serial port output");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("WSTA Exception: {0}", ex.ToString());
                                }

                            }
                            else
                            {
                                // send nak baddie
                            }
                            _serialPort.DiscardInBuffer();
                            break;

                        default:
                            if (IsDebugging)
                                Debug.WriteLine("reached default");

                            break;
                    }
                }
                else
                {
                    if (IsDebugging)
                        Debug.WriteLine("Checksum Bad.");
                }
                //_serialPort.DiscardInBuffer();
            }
        }

        private static void ReadConsoleInput()
        {
            while (true)
            {
                Console.WriteLine("Enter input for on the fly changes:"); // Prompt
                string line = Console.ReadLine(); // Get string from user
                if (line.ToUpper() == "exit".ToUpper()) // Check string
                {
                    Debug.WriteLine("Console Input: {0} is exit.", line);
                }
                Debug.WriteLine(string.Format("Console Input: {0}", line));
            }
        }

        static void UpdateSettingsFromOptions()
        {
            // if (Settings.Default.BaudRate != _options.BaudRate)

            if (_options.Speed != 0) // 1. new setting, 2.update setting 3. no setting use saved
            {
                Settings.Default.Speed = _options.Speed;
            }
            if (_options.Port != null)
            {
                Settings.Default.Port = _options.Port;
            }
            if (_options.DskA != null)
            {
                Settings.Default.DskA = _options.DskA;
            }
            if (_options.DskB != null)
            {
                Settings.Default.DskB = _options.DskB;
            }
            if (_options.DskC != null)
            {
                Settings.Default.DskC = _options.DskC;
            }
            if (_options.DskD != null)
            {
                Settings.Default.DskD = _options.DskD;
            }

            if (_options.OverWriteConfig)
                Settings.Default.Save();
        }

        private static void InitialSetup()
        {
            Console.WriteLine("Configuration not found. Suggest creating initial config using command line arguments with 'Save' option");
            Environment.Exit(0);
        }
    }
}
