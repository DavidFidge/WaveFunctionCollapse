using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Tests.Constraints;

[TestClass]
public class ConnectToSelfConstraintTests : BaseGraphicsTest
{
    private Texture2D _texture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _texture = new Texture2D(GraphicsDevice, 3, 3);
    }

    [TestMethod]
    public void Should_Pass_With_Default_Settings()
    {
        // Arrange
        var emptyPlacementConstraint = new ConnectToSelfConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute(), _texture);

        var tileResultUp = new TileResult(new Point(0, 0));
        var tileResultDown = new TileResult(new Point(0, 1));

        tileResultDown.SetNeighbours(new[]
        {
            tileResultUp, tileResultDown
        }, 1, 2);

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var tileChoiceUp = new TileChoice(tileTemplate, adapters, null, null, null);
        tileResultUp.ChosenTile = tileChoiceUp;

        var tileChoiceMiddle = new TileChoice(tileTemplate, adapters, null, null, null);

        // Act
        var result = emptyPlacementConstraint.Check(tileResultDown, tileChoiceMiddle,
            new HashSet<TileChoice> { tileChoiceMiddle });

        // Assert
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow(false, true, true, true, false)]
    [DataRow(true, false, true, true, false)]
    [DataRow(true, true, false, true, false)]
    [DataRow(true, true, true, false, false)]
    [DataRow(true, true, true, true, true)]
    public void Should_Fail_When_ConnectToSelf_Is_False(bool up, bool down, bool left, bool right, bool expectedResult)
    {
        // Arrange
        var connectToSelfConstraint = new ConnectToSelfConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute(), _texture);

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

        var tileChoice = new TileChoice(tileTemplate, adapters, null, null, null);
        tileResultUp.ChosenTile = tileChoice;
        tileResultDown.ChosenTile = tileChoice;
        tileResultLeft.ChosenTile = tileChoice;
        tileResultRight.ChosenTile = tileChoice;

        var canConnectToSelf = new Dictionary<Direction, bool>
        {
            { Direction.Up, up },
            { Direction.Down, down },
            { Direction.Left, left },
            { Direction.Right, right },
        };

        var tileToCheckMiddle = new TileChoice(tileTemplate, adapters, null, null, canConnectToSelf);

        // Act
        var result = connectToSelfConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
  
    [TestMethod]
    public void Should_Pass_When_ConnectToSelf_Is_False_And_No_Self_Tiles_Next_To_Tile()
    {
        // Arrange
        var emptyPlacementConstraint = new ConnectToSelfConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute(), _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute(), _texture);

        var tileResultUp = new TileResult(new Point(0, 0));
        var tileResultDown = new TileResult(new Point(0, 1));

        tileResultDown.SetNeighbours(new []
        {
            tileResultUp, tileResultDown
        }, 1, 2);

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "A" },
            { Direction.Up, "A" },
            { Direction.Left, "A" },
            { Direction.Right, "A" }
        };

        var canConnectToSelf = new Dictionary<Direction, bool>
        {
            { Direction.Up, false },
            { Direction.Down, false },
            { Direction.Left, false },
            { Direction.Right, false },
        };

        var tileChoiceUp = new TileChoice(tileTemplate1, adapters, null, null, canConnectToSelf);
        tileResultUp.ChosenTile = tileChoiceUp;

        var tileChoiceMiddle = new TileChoice(tileTemplate2, adapters, null, null, canConnectToSelf);

        // Act
        var result = emptyPlacementConstraint.Check(tileResultDown, tileChoiceMiddle, new HashSet<TileChoice> { tileChoiceMiddle });

        // Assert
        Assert.IsTrue(result);
    }
}