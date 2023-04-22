namespace FrigidRogue.WaveFunctionCollapse;

public class Adapter : ICloneable
{

    // Patterns must be defined in a clockwise order
    public string Pattern { get; set; }

    public static implicit operator Adapter(string pattern)
    {
        return new Adapter { Pattern = pattern };
    }

    public Adapter Clone()
    {
        return new Adapter
        {
            Pattern = Pattern
        };
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}