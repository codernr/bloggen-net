using System;
using CommandLine;

namespace Bloggen.Net
{
    class Program
    {
        public class CommandLineOptions
        {
            [Option('s', "source", Required = true, HelpText = "Source directory")]
            public string SourceDirectory { get; set; }

            [Option('o', "output", Required = true, HelpText = "Output directory")]
            public string OutputDirectory { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(o =>
                {
                    Console.WriteLine("Hello World!");
                })
                .WithNotParsed(errors =>
                {
                    Environment.Exit(1);
                });
        }
    }
}
