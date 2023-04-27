﻿using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Tests.Constraints;

[TestClass]
public class AdapterConstraintTests : BaseGraphicsTest
{
    private Texture2D _texture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _texture = new Texture2D(GraphicsDevice, 3, 3);
    }

    [TestMethod]
    public void Should_Pass_In_All_Directions()
    {
        // Arrange
        var adapterConstraint = new AdapterConstraint();

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
            new TileResult(new Point(0, 2)), tileResultUp, new TileResult(new Point(2, 2))
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

        var tileToCheckUp = new TileChoice(tileTemplate, adapters);
        var tileToCheckDown = new TileChoice(tileTemplate, adapters);
        var tileToCheckLeft = new TileChoice(tileTemplate, adapters);
        var tileToCheckRight = new TileChoice(tileTemplate, adapters);
        var tileToCheckMiddle = new TileChoice(tileTemplate, adaptersMiddle);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        adapterConstraint.Initialise(new List<TileTemplate>(), new MapOptions(3, 3));

        // Act
        var result = adapterConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckUp, tileToCheckDown, tileToCheckLeft, tileToCheckRight, tileToCheckMiddle });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_Fail_In_All_Directions()
    {
        // Arrange
        var adapterConstraint = new AdapterConstraint();

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
            new TileResult(new Point(0, 2)), tileResultUp, new TileResult(new Point(2, 2))
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
            { Direction.Down, "B" },
            { Direction.Up, "B" },
            { Direction.Left, "B" },
            { Direction.Right, "B" }
        };

        var tileToCheckUp = new TileChoice(tileTemplate, adapters);
        var tileToCheckDown = new TileChoice(tileTemplate, adapters);
        var tileToCheckLeft = new TileChoice(tileTemplate, adapters);
        var tileToCheckRight = new TileChoice(tileTemplate, adapters);
        var tileToCheckMiddle = new TileChoice(tileTemplate, adaptersMiddle);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        adapterConstraint.Initialise(new List<TileTemplate>(), new MapOptions(3, 3));

        // Act
        var result = adapterConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckUp, tileToCheckDown, tileToCheckLeft, tileToCheckRight, tileToCheckMiddle });

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Should_Pass_For_MultiCharacter_Adapter_Strings()
    {
        // Arrange
        var adapterConstraint = new AdapterConstraint();

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
            new TileResult(new Point(0, 2)), tileResultUp, new TileResult(new Point(2, 2))
        }, 3, 3);

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "ABC" },
            { Direction.Up, "BCD" },
            { Direction.Left, "MNO" },
            { Direction.Right, "NOP" }
        };

        var adaptersMiddle = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "DCB" },
            { Direction.Up, "CBA" },
            { Direction.Left, "PON" },
            { Direction.Right, "ONM" }
        };

        var tileToCheckUp = new TileChoice(tileTemplate, adapters);
        var tileToCheckDown = new TileChoice(tileTemplate, adapters);
        var tileToCheckLeft = new TileChoice(tileTemplate, adapters);
        var tileToCheckRight = new TileChoice(tileTemplate, adapters);
        var tileToCheckMiddle = new TileChoice(tileTemplate, adaptersMiddle);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        adapterConstraint.Initialise(new List<TileTemplate>(), new MapOptions(3, 3));

        // Act
        var result = adapterConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckUp, tileToCheckDown, tileToCheckLeft, tileToCheckRight, tileToCheckMiddle });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_Pass_For_Empty_Adapter()
    {
        // Arrange
        var adapterConstraint = new AdapterConstraint();

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
            new TileResult(new Point(0, 2)), tileResultUp, new TileResult(new Point(2, 2))
        }, 3, 3);

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "ABC" },
            { Direction.Up, "" },
            { Direction.Left, "MNO" },
            { Direction.Right, "NOP" }
        };

        var adaptersMiddle = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "DCB" },
            { Direction.Up, "CBA" },
            { Direction.Left, "PON" },
            { Direction.Right, "" }
        };

        var tileToCheckUp = new TileChoice(tileTemplate, adapters);
        var tileToCheckDown = new TileChoice(tileTemplate, adapters);
        var tileToCheckLeft = new TileChoice(tileTemplate, adapters);
        var tileToCheckRight = new TileChoice(tileTemplate, adapters);
        var tileToCheckMiddle = new TileChoice(tileTemplate, adaptersMiddle);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        adapterConstraint.Initialise(new List<TileTemplate>(), new MapOptions(3, 3));

        // Act
        var result = adapterConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckUp, tileToCheckDown, tileToCheckLeft, tileToCheckRight, tileToCheckMiddle });

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_Fail_For_MultiCharacter_Adapter_Strings_Not_In_Clockwise_Order()
    {
        // Arrange
        var adapterConstraint = new AdapterConstraint();

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
            new TileResult(new Point(0, 2)), tileResultUp, new TileResult(new Point(2, 2))
        }, 3, 3);

        var adapters = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "ABC" },
            { Direction.Up, "BCD" },
            { Direction.Left, "MNO" },
            { Direction.Right, "NOP" }
        };

        var adaptersMiddle = new Dictionary<Direction, Adapter>
        {
            { Direction.Down, "BCD" },
            { Direction.Up, "ABC" },
            { Direction.Left, "NOP" },
            { Direction.Right, "MNO" }
        };

        var tileToCheckUp = new TileChoice(tileTemplate, adapters);
        var tileToCheckDown = new TileChoice(tileTemplate, adapters);
        var tileToCheckLeft = new TileChoice(tileTemplate, adapters);
        var tileToCheckRight = new TileChoice(tileTemplate, adapters);
        var tileToCheckMiddle = new TileChoice(tileTemplate, adaptersMiddle);

        tileResultUp.ChosenTile = tileToCheckUp;
        tileResultDown.ChosenTile = tileToCheckDown;
        tileResultLeft.ChosenTile = tileToCheckLeft;
        tileResultRight.ChosenTile = tileToCheckRight;

        adapterConstraint.Initialise(new List<TileTemplate>(), new MapOptions(3, 3));

        // Act
        var result = adapterConstraint.Check(tileResultMiddle, tileToCheckMiddle, new HashSet<TileChoice> { tileToCheckUp, tileToCheckDown, tileToCheckLeft, tileToCheckRight, tileToCheckMiddle });

        // Assert
        Assert.IsFalse(result);
    }
}