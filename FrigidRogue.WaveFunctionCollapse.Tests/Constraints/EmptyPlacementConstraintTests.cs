using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Tests.Constraints;

[TestClass]
public class EmptyPlacementConstraintTests : BaseGraphicsTest
{
    private Texture2D _texture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _texture = new Texture2D(GraphicsDevice, 3, 3);
    }

    private Dictionary<Direction, bool> CreateEmptyPlacementRules(string emptyPlacementRule)
    {
        var emptyPlacementRules = new Dictionary<Direction, bool>();

        var parts = emptyPlacementRule.Split(',');

        emptyPlacementRules.Add(Direction.Up, bool.Parse(parts[0]));
        emptyPlacementRules.Add(Direction.Right, bool.Parse(parts[1]));
        emptyPlacementRules.Add(Direction.Down, bool.Parse(parts[2]));
        emptyPlacementRules.Add(Direction.Left, bool.Parse(parts[3]));

        return emptyPlacementRules;
    }

    [DataTestMethod]
    [DataRow("true,true,true,false", false)]
    [DataRow("true,true,false,true", false)]
    [DataRow("true,false,true,true", false)]
    [DataRow("false,true,true,true", false)]
    [DataRow("true,true,true,true", true)]
    public void Should_Not_Place_Tile_Next_To_Empty_Square(string emptyPlacementRule, bool expectedResult)
    {
        // Arrange
        var emptyPlacementConstraint = new EmptyPlacementConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute { EmptyPlacementRules = emptyPlacementRule }, _texture);

        var tileResultUp = new TileResult(new Point(1, 0));
        var tileResultDown = new TileResult(new Point(1, 2));
        var tileResultLeft = new TileResult(new Point(0, 1));
        var tileResultRight = new TileResult(new Point(2, 1));
        var tileResultMiddle = new TileResult(new Point(1, 1));

        tileResultMiddle.SetNeighbours(new []
        {
            new TileResult(new Point(0, 0)), tileResultUp, new TileResult(new Point(2, 0)),
            tileResultLeft, tileResultMiddle, tileResultRight,
            new TileResult(new Point(0, 2)), tileResultDown, new TileResult(new Point(2, 2))
        }, 3, 3);

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheckMiddle = new TileChoice(tileTemplate, adapters, CreateEmptyPlacementRules(emptyPlacementRule));

        // Act
        var result = emptyPlacementConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [DataTestMethod]
    [DataRow("true,true,true,false", false)]
    [DataRow("true,true,false,true", false)]
    [DataRow("true,false,true,true", false)]
    [DataRow("false,true,true,true", false)]
    [DataRow("true,true,true,true", true)]
    public void Should_Not_Place_Tile_Next_Edge_Of_Map(string emptyPlacementRule, bool expectedResult)
    {
        // Arrange
        var emptyPlacementConstraint = new EmptyPlacementConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute { EmptyPlacementRules = emptyPlacementRule }, _texture);

        var tileResultMiddle = new TileResult(new Point(0, 0));

        tileResultMiddle.SetNeighbours(new []
        {
           tileResultMiddle
        }, 1, 1);

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheckMiddle = new TileChoice(tileTemplate, adapters, CreateEmptyPlacementRules(emptyPlacementRule));

        // Act
        var result = emptyPlacementConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void Should_Place_Tile_If_Square_Not_Empty()
    {
        // Arrange
        var emptyPlacementConstraint = new EmptyPlacementConstraint();

        var tileTemplate1 = new TileTemplate("Test", new TileAttribute { EmptyPlacementRules = "false,false,false,false" }, _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute(), _texture);

        var tileResultUp = new TileResult(new Point(1, 0));
        var tileResultDown = new TileResult(new Point(1, 2));
        var tileResultLeft = new TileResult(new Point(0, 1));
        var tileResultRight = new TileResult(new Point(2, 1));
        var tileResultMiddle = new TileResult(new Point(1, 1));

        tileResultMiddle.SetNeighbours(new []
        {
            new TileResult(new Point(0, 0)), tileResultUp, new TileResult(new Point(2, 0)),
            tileResultLeft, tileResultMiddle, tileResultRight,
            new TileResult(new Point(0, 2)), tileResultDown, new TileResult(new Point(2, 2))
        }, 3, 3);

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheckUp = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckDown = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckLeft = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckRight = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckMiddle = new TileChoice(tileTemplate2, adapters, CreateEmptyPlacementRules("false,false,false,false"));

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        // Act
        var result = emptyPlacementConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.IsTrue(result);
    }
}