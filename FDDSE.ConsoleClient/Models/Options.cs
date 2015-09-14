using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace FDDSE.ConsoleClient.Models
{
    class Options
    {
        //[Option('i', "input", Required = true, HelpText = "Input file to read.")]
        //public string InputFile { get; set; }

        //[Option("length", DefaultValue = -1, HelpText = "The maximum number of bytes to process.")]
        //public int MaximumLength { get; set; }

        //[Option('v', null, HelpText = "Print details during execution.")]
        //public bool Verbose { get; set; }

        [Option('v', null, HelpText = "Print details during execution.(Not implemented)")]
        public bool Verbose { get; set; }

        [Option('o', "OverWriteConfig", DefaultValue = false, HelpText = "Persist (save) command line options to application configuration file for future use.")]
        public bool OverWriteConfig { get; set; }

        [Option('e', "EnumeratePorts", DefaultValue = false, HelpText = "Enumerate / List serial ports.")]
        public bool EnumeratePorts { get; set; }

        [Option('s', "Speed", HelpText = "Port Speed")]
        public int Speed { get; set; }

        [Option('p', "Port", DefaultValue = null, HelpText = "Specify serial port to use")]
        public string Port { get; set; }

        [Option('a', "DskA", DefaultValue = null, HelpText = "Fully qualified path to DskA")]
        public string DskA { get; set; }

        [Option('b', "DskB", DefaultValue = null, HelpText = "Fully qualified path to DskB")]
        public string DskB { get; set; }

        [Option('c', "DskC", DefaultValue = null, HelpText = "Fully qualified path to DskC")]
        public string DskC { get; set; }

        [Option('d', "DskD", DefaultValue = null, HelpText = "Fully qualified path to DskD")]
        public string DskD { get; set; }

        //[OptionList('f', "files", HelpText = "colon ':' seperated list of up to four (4) files maped to drives A through D")]
        //// [OptionList('f', "files", HelpText = "colon seperated list of up to four (4) files maped to drives A through D")]
        //public IList<string> Files { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            //var usage = new StringBuilder();
            //usage.AppendLine()
            //usage.AppendLine("Quickstart Application 1.0");
            //usage.AppendLine("Read user manual for usage instructions...");
            //return usage.ToString();
        }
    }
}
