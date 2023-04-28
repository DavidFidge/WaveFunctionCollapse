using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Tests.Constraints;

[TestClass]
public class PlacementConstraintTests : BaseGraphicsTest
{
    private Texture2D _texture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _texture = new Texture2D(GraphicsDevice, 3, 3);
    }

    [DataTestMethod]
    [DataRow("[X]", 1, 1, 2, true)]
    [DataRow("[X]", 5, 1, 2, false)]
    [DataRow("[Y]", 2, 1, 2, true)]
    [DataRow("[Y]", 5, 1, 2, false)]
    public void Should_Constrain_Placement_Of_Tile_To_PlacementRule_X_And_Y(string expression, int value, int x, int y, bool expectedResult)
    {
        // Arrange
        var placementConstraint = new PlacementConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute { PlacementRule = $"{expression} == {value}" }, _texture);

        var tileResult = new TileResult(new Point(x, y));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A,A,A,A" },
            { Direction.Up, "A,A,A,A" },
            { Direction.Left, "A,A,A,A" },
            { Direction.Right, "A,A,A,A" }
        };

        var tileToCheck = new TileChoice(tileTemplate, adapters, null);

        placementConstraint.Initialise(new List<TileTemplate>(), new MapOptions(5, 5));

        // Act
        var result = placementConstraint.Check(tileResult, tileToCheck, new HashSet<TileChoice> { tileToCheck });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void Should_Not_Constrain_Placement_If_No_Placement_Rule()
    {
        // Arrange
        var placementConstraint = new PlacementConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute(), _texture);

        var tileResult = new TileResult(new Point(2, 2));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A,A,A,A" },
            { Direction.Up, "A,A,A,A" },
            { Direction.Left, "A,A,A,A" },
            { Direction.Right, "A,A,A,A" }
        };

        var tileToCheck = new TileChoice(tileTemplate, adapters, null);

        placementConstraint.Initialise(new List<TileTemplate>(), new MapOptions(5, 5));

        // Act
        var result = placementConstraint.Check(tileResult, tileToCheck, new HashSet<TileChoice> { tileToCheck });

        // Assert
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("[X]", "[MaxX]", 4, 2, true)]
    [DataRow("[X]", "[MaxX]", 1, 2, false)]
    [DataRow("[Y]", "[MaxY]", 1, 4, true)]
    [DataRow("[Y]", "[MaxY]", 1, 2, false)]
    public void Should_Constrain_Placement_Of_Tile_To_PlacementRule_X_And_Y_Max(string expression, string value, int x, int y, bool expectedResult)
    {
        // Arrange
        var placementConstraint = new PlacementConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute { PlacementRule = $"{expression} == {value}" }, _texture);

        var tileResult = new TileResult(new Point(x, y));

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A,A,A,A" },
            { Direction.Up, "A,A,A,A" },
            { Direction.Left, "A,A,A,A" },
            { Direction.Right, "A,A,A,A" }
        };

        var tileToCheck = new TileChoice(tileTemplate, adapters, null);

        placementConstraint.Initialise(new List<TileTemplate>(), new MapOptions(5, 5));

        // Act
        var result = placementConstraint.Check(tileResult, tileToCheck, new HashSet<TileChoice> { tileToCheck });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}