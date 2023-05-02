namespace FrigidRogue.WaveFunctionCollapse;

public class Adapter : ICloneable
{

    // Patterns must be defined in a clockwise order
    private string _pattern;

    public string Pattern
    {
        get => _pattern;
        set
        {
            _pattern = value;
            AllPatterns = value?.Split("|").ToArray() ?? Array.Empty<string>();
        }
    }

    public string[] AllPatterns { get; set; }

    public static implicit operator Adapter(string pattern)
    {
        return new Adapter { Pattern = pattern };
    }

    public bool Matches(Adapter secondAdapter)
    {
        if (string.IsNullOrEmpty(Pattern) || string.IsNullOrEmpty(secondAdapter.Pattern))
            return true;

        return AllPatterns.Any(p => secondAdapter.AllPatterns.Any(p2 => p == new string(p2.Reverse().ToArray())));
    }

    public bool MatchesMandatory(Adapter secondAdapter)
    {
        if (string.IsNullOrEmpty(Pattern))
            return true;

        // No reversal when matching against a mandatory adapter
        return AllPatterns.Any(p => secondAdapter.AllPatterns.Any(p2 => p == p2));
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