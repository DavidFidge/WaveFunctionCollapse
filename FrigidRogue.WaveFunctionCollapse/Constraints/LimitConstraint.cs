namespace FrigidRogue.WaveFunctionCollapse;

public class LimitConstraint : TileConstraint
{
    public override int Order => 2;

    private Dictionary<TileContent, int> _tileLimits = new();
    private Dictionary<TileContent, int> _originalTileLimits = new();

    public override void Initialise(List<TileContent> tileContent)
    {
        _tileLimits.Clear();
        _originalTileLimits.Clear();

        foreach (var item in tileContent.Where(t => t.Attributes.Limit >= 0))
        {
            _originalTileLimits.Add(item, item.Attributes.Limit);
        }
    }

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> otherChoices)
    {
        if (_tileLimits.TryGetValue(tileToCheck.TileContent, out var limit))
        {
            if (limit == 0)
            {
                return false;
            }
        }

        return true;
    }

    public override void Revert(TileResult tile, TileChoice tileToCheck)
    {
        if (_tileLimits.ContainsKey(tileToCheck.TileContent))
        {
            if (_originalTileLimits[tileToCheck.TileContent] > _tileLimits[tileToCheck.TileContent])
                _tileLimits[tileToCheck.TileContent]++;
        }
    }

    public override void AfterChoice(TileResult tile, TileChoice tileToCheck)
    {
        if (!_tileLimits.ContainsKey(tileToCheck.TileContent))
            return;

        if (_tileLimits[tileToCheck.TileContent] > 0)
            _tileLimits[tileToCheck.TileContent]--;
    }
}