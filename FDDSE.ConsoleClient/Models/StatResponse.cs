using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDDSE.ConsoleClient.Models
{
/*
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
    public class StatResponse
    {
        // static byte[] MountedDrives { get; set; }

        byte[] SetMountedDrives(short set)
        {
            return null;
        }

        byte[] GetMountedDrives(short get)
        {
            return null;
        }


    }
}
