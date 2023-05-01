using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Tests.Constraints;

[TestClass]
public class ProhibitedEmptyNeighbourConstraintTests : BaseGraphicsTest
{
    private Texture2D _texture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _texture = new Texture2D(GraphicsDevice, 3, 3);
    }

    private Dictionary<Direction, ProhibitedEmptyNeighbourFlags> CreateProhibitedEmptyNeighbourRules(string prohibitedEmptyNeighbourRule)
    {
        var prohibitedEmptyNeighbourRules = new Dictionary<Direction, ProhibitedEmptyNeighbourFlags>();

        var parts = prohibitedEmptyNeighbourRule.Split(',');

        prohibitedEmptyNeighbourRules.Add(Direction.Up, Enum.Parse<ProhibitedEmptyNeighbourFlags>(parts[0].Replace("|",",")));
        prohibitedEmptyNeighbourRules.Add(Direction.Right, Enum.Parse<ProhibitedEmptyNeighbourFlags>(parts[1].Replace("|",",")));
        prohibitedEmptyNeighbourRules.Add(Direction.Down, Enum.Parse<ProhibitedEmptyNeighbourFlags>(parts[2].Replace("|",",")));
        prohibitedEmptyNeighbourRules.Add(Direction.Left, Enum.Parse<ProhibitedEmptyNeighbourFlags>(parts[3].Replace("|",",")));

        return prohibitedEmptyNeighbourRules;
    }

    [DataTestMethod]
    [DataRow("None,None,None,Uncollapsed", false)]
    [DataRow("None,None,Uncollapsed,None", false)]
    [DataRow("None,Uncollapsed,None,None", false)]
    [DataRow("Uncollapsed,None,None,None", false)]
    [DataRow("None,None,None,None", true)]
    public void Should_Not_Place_Tile_Next_To_Uncollapsed_Square(string prohibitedEmptyNeighbourRules, bool expectedResult)
    {
        // Arrange
        var emptyPlacementConstraint = new ProhibitedEmptyNeighbourConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute { ProhibitedEmptyNeighbourRules = prohibitedEmptyNeighbourRules }, _texture);

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

        var tileToCheckMiddle = new TileChoice(tileTemplate, adapters, CreateProhibitedEmptyNeighbourRules(prohibitedEmptyNeighbourRules), null);

        // Act
        var result = emptyPlacementConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [DataTestMethod]
    [DataRow("None,None,None,Unused", false)]
    [DataRow("None,None,Unused,None", false)]
    [DataRow("None,Unused,None,None", false)]
    [DataRow("Unused,None,None,None", false)]
    [DataRow("None,None,None,None", true)]
    public void Should_Not_Place_Tile_Next_To_Unused_Square(string prohibitedEmptyNeighbourRules, bool expectedResult)
    {
        // Arrange
        var emptyPlacementConstraint = new ProhibitedEmptyNeighbourConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute { ProhibitedEmptyNeighbourRules = prohibitedEmptyNeighbourRules }, _texture);

        var tileResultUp = new TileResult(new Point(1, 0)) { IsUnused = true };
        var tileResultDown = new TileResult(new Point(1, 2)) { IsUnused = true };
        var tileResultLeft = new TileResult(new Point(0, 1)) { IsUnused = true };
        var tileResultRight = new TileResult(new Point(2, 1)) { IsUnused = true };
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

        var tileToCheckMiddle = new TileChoice(tileTemplate, adapters, CreateProhibitedEmptyNeighbourRules(prohibitedEmptyNeighbourRules), null);

        // Act
        var result = emptyPlacementConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [DataTestMethod]
    [DataRow("None,None,None,EdgeOfMap", false)]
    [DataRow("None,None,EdgeOfMap,None", false)]
    [DataRow("None,EdgeOfMap,None,None", false)]
    [DataRow("EdgeOfMap,None,None,None", false)]
    [DataRow("None,None,None,None", true)]
    public void Should_Not_Place_Tile_Next_Edge_Of_Map(string prohibitedEmptyNeighbourRules, bool expectedResult)
    {
        // Arrange
        var emptyPlacementConstraint = new ProhibitedEmptyNeighbourConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute { ProhibitedEmptyNeighbourRules = prohibitedEmptyNeighbourRules }, _texture);

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

        var tileToCheckMiddle = new TileChoice(tileTemplate, adapters, CreateProhibitedEmptyNeighbourRules(prohibitedEmptyNeighbourRules), null);

        // Act
        var result = emptyPlacementConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [DataTestMethod]
    [DataRow("None,None,None,All", false)]
    [DataRow("None,None,All,None", false)]
    [DataRow("None,All,None,None", false)]
    [DataRow("All,None,None,None", false)]
    [DataRow("None,None,None,None", true)]
    [DataRow("None,None,None,Uncollapsed|Unused|EdgeOfMap", false)]
    [DataRow("None,None,Uncollapsed|Unused|EdgeOfMap,None", false)]
    [DataRow("None,Uncollapsed|Unused|EdgeOfMap,None,None", false)]
    [DataRow("Uncollapsed|Unused|EdgeOfMap,None,None,None", false)]
    public void Should_Not_Place_Tile_Next_To_Any_Empty_Square(string prohibitedEmptyNeighbourRules, bool expectedResult)
    {
        // Arrange
        var emptyPlacementConstraint = new ProhibitedEmptyNeighbourConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute { ProhibitedEmptyNeighbourRules = prohibitedEmptyNeighbourRules }, _texture);

        var tileResultUp = new TileResult(new Point(1, 0));
        var tileResultLeft = new TileResult(new Point(0, 1));
        var tileResultRight = new TileResult(new Point(2, 1)) { IsUnused = true };
        var tileResultMiddle = new TileResult(new Point(1, 1));

        tileResultMiddle.SetNeighbours(new []
        {
            new TileResult(new Point(0, 0)), tileResultUp, new TileResult(new Point(2, 0)),
            tileResultLeft, tileResultMiddle, tileResultRight
        }, 3, 2);

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheckMiddle = new TileChoice(tileTemplate, adapters, CreateProhibitedEmptyNeighbourRules(prohibitedEmptyNeighbourRules), null);

        // Act
        var result = emptyPlacementConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void Should_Place_Tile_If_Square_Not_Empty()
    {
        // Arrange
        var emptyPlacementConstraint = new ProhibitedEmptyNeighbourConstraint();

        var tileTemplate1 = new TileTemplate("Test", new TileAttribute { ProhibitedEmptyNeighbourRules = "None,None,None,None" }, _texture);
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

        var tileToCheckUp = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckDown = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckLeft = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckRight = new TileChoice(tileTemplate1, adapters, null, null);
        var tileToCheckMiddle = new TileChoice(tileTemplate2, adapters, CreateProhibitedEmptyNeighbourRules("None,None,None,None"), null);

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