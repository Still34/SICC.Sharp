using CommandLine;

namespace SICCSharp
{
    public class LaunchOptions
    {
        [Option('i', "input", HelpText = "Input filepath", Required = true)]
        public string InputPath { get; set; }
        [Option('v', "verbose", Default = true, HelpText = "Enable verbose output")]
        public bool IsVerbose { get; set; }
        [Option('o', "output", HelpText = "Output directory. Defaults to input directory.")]
        public string OutputPath { get; set; }
    }
}