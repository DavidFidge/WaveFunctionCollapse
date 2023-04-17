namespace FrigidRogue.WaveFunctionCollapse;

public class Adapter
{
    // Patterns must be defined in a clockwise order
    public string Pattern { get; set; }

    public static implicit operator Adapter(string pattern)
    {
        return new Adapter { Pattern = pattern };
    }
}