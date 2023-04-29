using CommandLine;

namespace WaveFunctionCollapse;

public class Options
{
    [Option('t', "TileSet", Required = false, HelpText = "Starts with the given tile set selected")]
    public string TileSet { get; set; }
}