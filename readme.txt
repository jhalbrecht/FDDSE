jha 9/15/2015 FDDSE readme.txt
http://retrowagon.org/wiki/index.php/FDDSE

limitations
fddse supports a maximum of four (4) mounted drives.
fddse currently supports a single baud rate on linux; 230400

Command line arguments
-e enumerate serial ports to find the correct port and syntax
-o Overwrite / persist the config file
--help display brief usage syntax.

Notes:
Be sure that CommandLine.dll, distributed with fddse.zip, is in your path. I keep it with the fddse.exe program.

Debian Linux on a BeagleBone Black

root@fddse.retrowagon.org:~# apt-get update
root@fddse.retrowagon.org:~# apt-get upgrade
root@fddse.retrowagon.org:~# apt-get install mono-complete
root@fddse.retrowagon.org:~# apt-get install unzip
root@fddse.retrowagon.org:~# unzip FDDSE
root@fddse.retrowagon.org:~# cd FDDSE
root@fddse.retrowagon.org:~/FDDSE# chmod a+x fddse.exe
root@fddse.retrowagon.org:~/FDDSE# ./fddse -e
root@fddse.retrowagon.org:~/FDDSE# ./fddse.exe -e
FDDSE Floppy Disk Drive Serial Emulator

Default Port: /dev/ttyUSB0, Speed: 230400
Available serial ports.
/dev/ttyS0
/dev/ttyS1
/dev/ttyS2
/dev/ttyS3
/dev/ttyUSB0
root@fddse.retrowagon.org:~/FDDSE#
root@fddse.retrowagon.org:~/FDDSE# ./fddse.exe  -p/dev/ttyUSB0 -s230400 -aData/cpm48k.dsk -bData/wordstar.dsk -o

Next start up just enter fddse without the arguments. 
root@fddse.retrowagon.org:~/FDDSE# ./fddse.exe
It will start with options from previous session as long as you previously used the -o for overwrite (persist) command line switch.
root@fddse.retrowagon.org:~/FDDSE# ./fddse.exe
