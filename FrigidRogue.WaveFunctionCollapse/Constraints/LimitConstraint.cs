using FrigidRogue.WaveFunctionCollapse.Options;

namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class LimitConstraint : TileConstraint
{
    public override int Order => 2;

    private Dictionary<TileTemplate, int> _tileLimits = new();
    private Dictionary<TileTemplate, int> _originalTileLimits = new();

    public override void Initialise(List<TileTemplate> tileTemplates, MapOptions mapOptions)
    {
        _tileLimits.Clear();
        _originalTileLimits.Clear();

        foreach (var item in tileTemplates.Where(t => t.Attributes.Limit >= 0))
        {
            _tileLimits.Add(item, item.Attributes.Limit);
            _originalTileLimits.Add(item, item.Attributes.Limit);
        }
    }

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> otherChoices)
    {
        if (_tileLimits.TryGetValue(tileToCheck.TileTemplate, out var limit))
        {
            if (limit == 0)
            {
                return false;
            }
        }

        return true;
    }

    public override void Revert(TileChoice tileToCheck)
    {
        if (_tileLimits.ContainsKey(tileToCheck.TileTemplate))
        {
            if (_originalTileLimits[tileToCheck.TileTemplate] > _tileLimits[tileToCheck.TileTemplate])
                _tileLimits[tileToCheck.TileTemplate]++;
        }
    }

    public override void AfterChoice(TileChoice tileToCheck)
    {
        if (!_tileLimits.ContainsKey(tileToCheck.TileTemplate))
            return;

        if (_tileLimits[tileToCheck.TileTemplate] > 0)
            _tileLimits[tileToCheck.TileTemplate]--;
    }
}