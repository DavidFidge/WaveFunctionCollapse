using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Tests.Constraints;

[TestClass]
public class MandatoryAdapterConstraintTests : BaseGraphicsTest
{
    private Texture2D _texture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _texture = new Texture2D(GraphicsDevice, 3, 3);
    }

    [DataTestMethod]
    [DataRow("A","","","", true)]
    [DataRow("","A","","", true)]
    [DataRow("","","A","", true)]
    [DataRow("","","","A", true)]
    [DataRow("","","","", false)]
    public void Should_Pass_When_One_Adjacent_Tile_Has_Mandatory_Adapter(string upAdaptor, string downAdapter, string leftAdapter, string rightAdapter, bool expectedResult)
    {
        // Arrange
        var mandatoryAdapterConstraint = new MandatoryAdapterConstraint();

        var tileTemplate1 = new TileTemplate("Test", new TileAttribute(), _texture);
        var tileTemplate2 = new TileTemplate("Test2", new TileAttribute() { MandatoryAdapters = "A"}, _texture);

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
            { Direction.Down, downAdapter },
            { Direction.Up, upAdaptor },
            { Direction.Left, leftAdapter },
            { Direction.Right, rightAdapter }
        };

        var adaptersMiddle = new Dictionary<Direction, Adapter>
        {
            // using arbitrary values for adapters - the mandatory adapters checker only checks 
            // the mandatory adapters string of the tile being allocated against the adapters of the neighbour tiles
            { Direction.Down, "B" },
            { Direction.Up, "B" },
            { Direction.Left, "B" },
            { Direction.Right, "B" }
        };

        var tileToCheckUp = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckDown = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckLeft = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckRight = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckMiddle = new TileChoice(tileTemplate2, adaptersMiddle, null);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        // Act
        var result = mandatoryAdapterConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void Should_Pass_When_No_Mandatory_Adapters()
    {
        // Arrange
        var mandatoryAdapterConstraint = new MandatoryAdapterConstraint();

        var tileTemplate1 = new TileTemplate("Test", new TileAttribute(), _texture);
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

        var adaptersMiddle = new Dictionary<Direction, Adapter>
        {
            // using arbitrary values for adapters - the mandatory adapters checker only checks 
            // the mandatory adapters string of the tile being allocated against the adapters of the neighbour tiles
            { Direction.Down, "B" },
            { Direction.Up, "B" },
            { Direction.Left, "B" },
            { Direction.Right, "B" }
        };

        var tileToCheckUp = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckDown = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckLeft = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckRight = new TileChoice(tileTemplate1, adapters, null);
        var tileToCheckMiddle = new TileChoice(tileTemplate2, adaptersMiddle, null);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        // Act
        var result = mandatoryAdapterConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckMiddle });

        // Assert
        Assert.IsTrue(result);
    }
}