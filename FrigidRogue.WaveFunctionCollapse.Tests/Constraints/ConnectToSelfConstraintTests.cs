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

    [TestMethod]
    public void Should_Fail_When_ConnectToSelf_Is_False()
    {
        // Arrange
        var emptyPlacementConstraint = new ConnectToSelfConstraint();

        var tileTemplate = new TileTemplate("Test", new TileAttribute { CanConnectToSelf = "false,false,false,false" }, _texture);

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

        var tileChoiceUp = new TileChoice(tileTemplate, adapters, null, null, null);
        tileResultUp.ChosenTile = tileChoiceUp;

        var tileChoiceMiddle = new TileChoice(tileTemplate, adapters, null, null, null);

        // Act
        var result = emptyPlacementConstraint.Check(tileResultDown, tileChoiceMiddle, new HashSet<TileChoice> { tileChoiceMiddle });

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Should_Pass_When_ConnectToSelf_Is_False_And_No_Self_Tiles_Next_To_Tile()
    {
        // Arrange
        var emptyPlacementConstraint = new ConnectToSelfConstraint();

        var tileTemplate1 = new TileTemplate("Test1", new TileAttribute { CanConnectToSelf = "false,false,false,false"}, _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute { CanConnectToSelf = "false,false,false,false"}, _texture);

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

        var tileChoiceUp = new TileChoice(tileTemplate1, adapters, null, null, null);
        tileResultUp.ChosenTile = tileChoiceUp;

        var tileChoiceMiddle = new TileChoice(tileTemplate2, adapters, null, null, null);

        // Act
        var result = emptyPlacementConstraint.Check(tileResultDown, tileChoiceMiddle, new HashSet<TileChoice> { tileChoiceMiddle });

        // Assert
        Assert.IsTrue(result);
    }
}