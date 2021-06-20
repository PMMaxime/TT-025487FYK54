using CommandLine;

namespace CSVToJSON
{
    public class CmdLineArgs
    {
        [Option('h', "headers", Required = false, HelpText = "Are headers present in the csv input file ?")]
        public bool Headers { get; set; }
        [Option('f', "file", Required = false, HelpText = "the path to the csv file to be transformed.")]
        public string FilePath { get; set; }
    }
}
