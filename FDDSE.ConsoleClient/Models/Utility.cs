using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;

namespace FDDSE.ConsoleClient.Models
{
    public class Utility : IUtility
    {
        public short CalculateChecksumOfByteArray(byte[] buffer, int bytes)
        {
            int calculatedChecksum = 0;
            try
            {
                //int calculatedChecksum = 0;
                for (int b = 0; b < bytes; b++)
                    calculatedChecksum += (buffer[b]);
                return (short)calculatedChecksum;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string SayString(string what)
        {
            return String.Format("{0}", what); 
            // return SayString(what);
        }

        public string Version()
        {
            var appSettings = ConfigurationManager.AppSettings;
            // return appSettings["Version"] ?? "Not Found";
            //return ConfigurationManager.AppSettings["Version"].ToString();
            // return Properties.Settings.Default.Version.ToString();
            return null;
        }

        /// <summary> Convert a string of hex digits (ex: E4 CA B2) to a byte array. </summary>
        /// <param name="s"> The string containing the hex digits (with or without spaces). </param>
        /// <returns> Returns an array of bytes. </returns>
        private static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary> Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
        /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
        /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
        private string ByteArrayToHexString2029951325(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }
        /// <summary> Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
        /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
        /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }
    }
}
