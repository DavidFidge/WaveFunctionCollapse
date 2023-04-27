using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Tests.Constraints;

[TestClass]
public class CategoryConstraintTests : BaseGraphicsTest
{
    private Texture2D _texture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _texture = new Texture2D(GraphicsDevice, 3, 3);
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void Should_Pass_When_CanConnectToCategories_Matches_With_Tile_With_Category(int directionToCheck)
    {
        // Arrange
        var typeConstraint = new CategoryConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute { Category = "X", CanConnectToCategories = new[] { "Y" } }, _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Category = "Y", CanConnectToCategories = new[] { "X" } }, _texture);
        var tileTemplateOther = new TileTemplate("Test3", new TileAttribute(), _texture);

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

        var adaptersMiddle = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheckUp = new TileChoice(directionToCheck == 1 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckDown = new TileChoice(directionToCheck == 2 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckLeft = new TileChoice(directionToCheck == 3 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckRight = new TileChoice(directionToCheck == 4 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckMiddle = new TileChoice(tileTemplate2, adaptersMiddle);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        // Act
        var result = typeConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckUp, tileToCheckDown, tileToCheckLeft, tileToCheckRight, tileToCheckMiddle });

        // Assert
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void Should_Still_Pass_When_CanConnectToCategories_Matches_With_Tile_With_Category_And_Other_Side_Does_Not_Connect_In_Reverse(int directionToCheck)
    {
        // Arrange
        var typeConstraint = new CategoryConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute { Category = "X", CanConnectToCategories = new[] { "Z" } }, _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Category = "Y", CanConnectToCategories = new[] { "X" } }, _texture);
        var tileTemplateOther = new TileTemplate("Test3", new TileAttribute(), _texture);

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

        var adaptersMiddle = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheckUp = new TileChoice(directionToCheck == 1 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckDown = new TileChoice(directionToCheck == 2 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckLeft = new TileChoice(directionToCheck == 3 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckRight = new TileChoice(directionToCheck == 4 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckMiddle = new TileChoice(tileTemplate2, adaptersMiddle);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        // Act
        var result = typeConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckUp, tileToCheckDown, tileToCheckLeft, tileToCheckRight, tileToCheckMiddle });

        // Assert
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void Should_Fail_When_CanConnectToCategories_Is_Empty(int directionToCheck)
    {
        // Arrange
        var typeConstraint = new CategoryConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute { Category = "X", CanConnectToCategories = new[] { "Y" } }, _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Category = "Y", CanConnectToCategories = Array.Empty<string>() }, _texture);
        var tileTemplateOther = new TileTemplate("Test3", new TileAttribute(), _texture);

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

        var adaptersMiddle = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheckUp = new TileChoice(directionToCheck == 1 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckDown = new TileChoice(directionToCheck == 2 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckLeft = new TileChoice(directionToCheck == 3 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckRight = new TileChoice(directionToCheck == 4 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckMiddle = new TileChoice(tileTemplate2, adaptersMiddle);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        // Act
        var result = typeConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckUp, tileToCheckDown, tileToCheckLeft, tileToCheckRight, tileToCheckMiddle });

        // Assert
        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void Should_Fail_When_CanConnectToCategories_Does_Not_Match_With_Tile_With_Category(int directionToCheck)
    {
        // Arrange
        var typeConstraint = new CategoryConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute { Category = "X", CanConnectToCategories = new[] { "Y" } }, _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { Category = "Y", CanConnectToCategories = new[] { "Z" } }, _texture);
        var tileTemplateOther = new TileTemplate("Test3", new TileAttribute(), _texture);

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

        var adaptersMiddle = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileToCheckUp = new TileChoice(directionToCheck == 1 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckDown = new TileChoice(directionToCheck == 2 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckLeft = new TileChoice(directionToCheck == 3 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckRight = new TileChoice(directionToCheck == 4 ? tileTemplate1 : tileTemplateOther, adapters);
        var tileToCheckMiddle = new TileChoice(tileTemplate2, adaptersMiddle);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        // Act
        var result = typeConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckUp, tileToCheckDown, tileToCheckLeft, tileToCheckRight, tileToCheckMiddle });

        // Assert
        Assert.IsFalse(result);
    }
}