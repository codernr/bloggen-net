using CommandLine;

public class CommandLineOptions
{
    [Option('s', "source", Required = true, HelpText = "Source directory")]
    public string SourceDirectory { get; set; } = string.Empty;

    [Option('o', "output", Required = true, HelpText = "Output directory")]
    public string OutputDirectory { get; set; } = string.Empty;
}