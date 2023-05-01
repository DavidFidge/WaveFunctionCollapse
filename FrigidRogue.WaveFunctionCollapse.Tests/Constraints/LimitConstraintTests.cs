using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Tests.Constraints;

[TestClass]
public class LimitConstraintTests : BaseGraphicsTest
{
    private Texture2D _texture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _texture = new Texture2D(GraphicsDevice, 3, 3);
    }

    [TestMethod]
    public void Should_Pass_If_Limit_Not_Reached()
    {
        // Arrange
        var limitConstraint = new LimitConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute(), _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Limit = 1 }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckLimit = new TileChoice(tileTemplate2, adapters, null, null);

        limitConstraint.Initialise(new List<TileTemplate> { tileTemplate1, tileTemplate2 }, new MapOptions(3, 3));

        // Act
        var result = limitConstraint.Check(tileResult, tileToCheckLimit, new HashSet<TileChoice> { tileToCheck, tileToCheckLimit });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_Fail_If_Limit_Reached()
    {
        // Arrange
        var limitConstraint = new LimitConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute(), _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Limit = 0 }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckLimit = new TileChoice(tileTemplate2, adapters, null, null);

        limitConstraint.Initialise(new List<TileTemplate> { tileTemplate1, tileTemplate2 }, new MapOptions(3, 3));

        // Act
        var result = limitConstraint.Check(tileResult, tileToCheckLimit, new HashSet<TileChoice> { tileToCheck, tileToCheckLimit });

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Should_Fail_After_Limit_Reached_When_AfterChoice_Called()
    {
        // Arrange
        var limitConstraint = new LimitConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute(), _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Limit = 1 }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckLimit = new TileChoice(tileTemplate2, adapters, null, null);

        limitConstraint.Initialise(new List<TileTemplate> { tileTemplate1, tileTemplate2 }, new MapOptions(3, 3));
        limitConstraint.AfterChoice(tileToCheckLimit);

        // Act
        var result = limitConstraint.Check(tileResult, tileToCheckLimit, new HashSet<TileChoice> { tileToCheck, tileToCheckLimit });

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Should_Pass_If_Reverted_After_Limit_Reached()
    {
        // Arrange
        var limitConstraint = new LimitConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute { Limit = 0 }, _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Limit = 1 }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckLimit = new TileChoice(tileTemplate2, adapters, null, null);

        limitConstraint.Initialise(new List<TileTemplate> { tileTemplate1, tileTemplate2 }, new MapOptions(3, 3));
        limitConstraint.AfterChoice(tileToCheckLimit);
        limitConstraint.Revert(tileToCheckLimit);

        // Act
        var result = limitConstraint.Check(tileResult, tileToCheckLimit, new HashSet<TileChoice> { tileToCheck, tileToCheckLimit });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_Fail_After_Limit_Reached_After_Revert_And_After_AfterChoice_Called()
    {
        // Arrange
        var limitConstraint = new LimitConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute { Limit = 1 }, _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Limit = 1 }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckLimit = new TileChoice(tileTemplate2, adapters, null, null);

        limitConstraint.Initialise(new List<TileTemplate> { tileTemplate1, tileTemplate2 }, new MapOptions(3, 3));
        limitConstraint.AfterChoice(tileToCheckLimit);
        limitConstraint.Revert(tileToCheckLimit);
        limitConstraint.AfterChoice(tileToCheckLimit);

        // Act
        var result = limitConstraint.Check(tileResult, tileToCheckLimit, new HashSet<TileChoice> { tileToCheck, tileToCheckLimit });

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Should_Pass_If_Not_A_Limited_Tile()
    {
        // Arrange
        var limitConstraint = new LimitConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute(), _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Limit = 0 }, _texture);

        var tileResult = new TileResult(new Point(1, 1));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheck = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckLimit = new TileChoice(tileTemplate2, adapters, null, null);

        limitConstraint.Initialise(new List<TileTemplate> { tileTemplate1, tileTemplate2 }, new MapOptions(3, 3));

        // Act
        var result = limitConstraint.Check(tileResult, tileToCheck, new HashSet<TileChoice> { tileToCheck, tileToCheckLimit });

        // Assert
        Assert.IsTrue(result);
    }
}