using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Tests.Constraints;

[TestClass]
public class OnlyAllowedIfNoValidTilesConstraintTests : BaseGraphicsTest
{
    private Texture2D _texture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _texture = new Texture2D(GraphicsDevice, 3, 3);
    }

    [TestMethod]
    public void Should_Fail_If_Another_Tile_With_OnlyAllowedIfNoValidTiles_Not_Set()
    {
        // Arrange
        var onlyAllowedIfNoValidTilesConstraint = new OnlyAllowedIfNoValidTilesConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute(), _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { OnlyAllowedIfNoValidTiles = true }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckOnlyAllowedIfNoValidTiles = new TileChoice(tileTemplate2, adapters, null);

        // Act
        var result = onlyAllowedIfNoValidTilesConstraint.Check(tileResult, tileToCheckOnlyAllowedIfNoValidTiles, new HashSet<TileChoice> { tileToCheck, tileToCheckOnlyAllowedIfNoValidTiles });

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Should_Pass_If_Testing_A_Tile_Without_OnlyAllowedIfNoValidTiles_Set()
    {
        // Arrange
        var onlyAllowedIfNoValidTilesConstraint = new OnlyAllowedIfNoValidTilesConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute(), _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { OnlyAllowedIfNoValidTiles = true }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckOnlyAllowedIfNoValidTiles = new TileChoice(tileTemplate2, adapters, null);

        // Act
        var result = onlyAllowedIfNoValidTilesConstraint.Check(tileResult, tileToCheck, new HashSet<TileChoice> { tileToCheck, tileToCheckOnlyAllowedIfNoValidTiles });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_Pass_If_Another_Tile_With_OnlyAllowedIfNoValidTiles_Is_Set()
    {
        // Arrange
        var onlyAllowedIfNoValidTilesConstraint = new OnlyAllowedIfNoValidTilesConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute { OnlyAllowedIfNoValidTiles = true }, _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { OnlyAllowedIfNoValidTiles = true }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckOnlyAllowedIfNoValidTiles = new TileChoice(tileTemplate2, adapters, null);

        // Act
        var result = onlyAllowedIfNoValidTilesConstraint.Check(tileResult, tileToCheckOnlyAllowedIfNoValidTiles, new HashSet<TileChoice> { tileToCheck, tileToCheckOnlyAllowedIfNoValidTiles });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_Pass_If_OnlyAllowedIfNoValidTiles_Is_Set_And_Only_Tile()
    {
        // Arrange
        var onlyAllowedIfNoValidTilesConstraint = new OnlyAllowedIfNoValidTilesConstraint();

        var tileTemplate = new TileTemplate("Test1", new TileAttribute { OnlyAllowedIfNoValidTiles = true }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate, adapters, null);

        // Act
        var result = onlyAllowedIfNoValidTilesConstraint.Check(tileResult, tileToCheck, new HashSet<TileChoice> { tileToCheck });

        // Assert
        Assert.IsTrue(result);
    }
}