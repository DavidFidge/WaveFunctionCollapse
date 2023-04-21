using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.TestInfrastructure;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;

namespace FrigidRogue.WaveFunctionCollapse.Tests;

[TestClass]
public class WaveFunctionCollapseTests : BaseGraphicsTest
{
    private Texture2D _floorTexture;
    private Texture2D _lineTexture;
    private Texture2D _cornerTexture;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _floorTexture = new Texture2D(GraphicsDevice, 3, 3);
        _lineTexture = new Texture2D(GraphicsDevice, 3, 3);
        _cornerTexture = new Texture2D(GraphicsDevice, 3, 3);

        // Set color data on floorTexture
        var floorTextureData = new Color[9];
        var lineTextureData = new Color[9];
        var cornerTextureData = new Color[9];

        for (var i = 0; i < 9; i++)
        {
            floorTextureData[i] = Color.White;
            lineTextureData[i] = Color.White;
            cornerTextureData[i] = Color.White;
            
            if (i is 1 or 4 or 7)
                lineTextureData[i] = Color.Black;
            
            if (i is 3 or 4 or 5)
                cornerTextureData[i] = Color.Black;
        }

        _floorTexture.SetData(floorTextureData);
        _lineTexture.SetData(lineTextureData);
        _cornerTexture.SetData(cornerTextureData);
    }

    [TestMethod]
    public void L_Shaped_Symmetry_Should_Process_Into_Four_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Corner", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "^",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL"
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        // Assert
        var tiles = waveFunctionCollapse.Tiles;

        Assert.AreEqual(4, tiles.Count);

        AssertTile(tiles[0], "ABC,DEF,GHI,JKL", _floorTexture);
        AssertTile(tiles[1], "JKL,ABC,DEF,GHI", _floorTexture, expectedRotation: (float)Math.PI / 2);
        AssertTile(tiles[2], "GHI,JKL,ABC,DEF", _floorTexture, expectedRotation: (float)Math.PI);
        AssertTile(tiles[3], "DEF,GHI,JKL,ABC", _floorTexture, expectedRotation: (float)-Math.PI / 2);
    }

    [TestMethod]
    public void I_Shaped_Symmetry_Should_Process_Into_Two_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
            {
                { "Line", _floorTexture }
            };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "I",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL"
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        // Assert
        var tiles = waveFunctionCollapse.Tiles;

        Assert.AreEqual(2, tiles.Count);

        AssertTile(tiles[0], "ABC,DEF,GHI,JKL", _floorTexture);
        AssertTile(tiles[1], "JKL,ABC,DEF,GHI", _floorTexture, expectedRotation: (float)Math.PI / 2);
    }

    [TestMethod]
    public void ForwardSlash_Shaped_Symmetry_Should_Process_Into_Two_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Line", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "/",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL"
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        // Assert
        var tiles = waveFunctionCollapse.Tiles;

        Assert.AreEqual(2, tiles.Count);

        AssertTile(tiles[0], "ABC,DEF,GHI,JKL", _floorTexture);
        AssertTile(tiles[1], "GHI,JKL,ABC,DEF", _floorTexture, expectedRotation: (float)Math.PI);
    }

    [TestMethod]
    public void X_Shaped_Symmetry_Should_Process_Into_One_Tile()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        // Assert
        var tiles = waveFunctionCollapse.Tiles;

        Assert.AreEqual(1, tiles.Count);

        AssertTile(tiles[0], "AAA,AAA,AAA,AAA", _floorTexture);
    }

    [TestMethod]
    public void Should_Reset()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Line", _lineTexture },
            { "Corner", _cornerTexture },
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                },
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "I",
                        Weight = 1,
                        Adapters = "ABA,CCC,ABA,CCC"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "^",
                        Weight = 1,
                        Adapters = "AAA,AAA,ABA,ABA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        // Act
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2));

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        
        foreach (var tile in waveFunctionCollapse.CurrentState)
        {
            AssertTileResult(
                tile,
                null,
                tile.Point
                    .Neighbours(1, 1, AdjacencyRule.Types.Cardinals)
                    .Select(p => waveFunctionCollapse.CurrentState.First(t => t.Point == p))
                    .ToArray(),
                0,
                false);
        }
    }

    [TestMethod]
    public void Adapters_Should_Be_Defined_In_Clockwise_Order()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "ABC,CCC,CBA,CCC"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 2) { FallbackAttempts = 0});

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.CurrentState.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
    }

    [TestMethod]
    public void Adapters_Should_Fail_If_Not_Defined_In_Clockwise_Order()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "ABC,CCC,ABC,CCC"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 2) { FallbackAttempts = 0});

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
    }

    [TestMethod]
    public void Tiles_To_Be_Initialised_Should_Have_Reduced_Entropy_So_They_Are_Processed_First()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Line", _lineTexture },
            { "Corner", _cornerTexture },
        };

        var tileAttributesWithInitialisation = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        InitialisationRule = "([X] == 0 || [X] == [MaxX] || [Y] == 0 || [Y] == [MaxY])"
                    }
                },
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "I",
                        Weight = 1,
                        Adapters = "ABA,CCC,ABA,CCC"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "^",
                        Weight = 1,
                        Adapters = "AAA,AAA,ABA,ABA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributesWithInitialisation);

        // Act
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(3, 3));

        // Assert
        Assert.AreEqual(9, waveFunctionCollapse.CurrentState.Length);
        var midPoint = new Point(1, 1);
        
        foreach (var tile in waveFunctionCollapse.CurrentState)
        {
            AssertTileResult(
                tile,
                null,
                tile.Point
                    .Neighbours(2, 2, AdjacencyRule.Types.Cardinals)
                    .Select(p => waveFunctionCollapse.CurrentState.First(t => t.Point == p))
                    .ToArray(),
                tile.Point == midPoint ? 0 : (0 - (int.MaxValue / 2)),
                false);
        }
    }

    [TestMethod]
    public void Should_Process_Next_Step()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2) { EntropyCalculationMethod = EntropyCalculationMethod.ReduceByCountOfNeighbours});

        // Act
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);
        
        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[0].TileChoice);

        var otherTiles = waveFunctionCollapse.CurrentState.Except(tileResults).ToList();

        Assert.AreEqual(3, otherTiles.Count);
        Assert.AreEqual(2, otherTiles.Count(t => t.Entropy == -1));
        Assert.AreEqual(1, otherTiles.Count(t => t.Entropy == 0));

        CollectionAssert.AreEquivalent(tileResults[0].Neighbours, otherTiles.Where(t => t.Entropy == -1).ToList());
    }

    [TestMethod]
    public void Should_Process_Two_Steps()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2) { EntropyCalculationMethod = EntropyCalculationMethod.ReduceByCountOfNeighbours});

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);
        
        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[0].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[1].TileChoice);

        var otherTiles = waveFunctionCollapse.CurrentState.Except(tileResults).ToList();

        Assert.AreEqual(2, otherTiles.Count);
        Assert.AreEqual(2, otherTiles.Count(t => t.Entropy == -1));
    }

    [TestMethod]
    public void Should_Process_Three_Steps()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2) { EntropyCalculationMethod = EntropyCalculationMethod.ReduceByCountOfNeighbours});

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(3, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[0].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[1].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[2].TileChoice);

        var otherTiles = waveFunctionCollapse.CurrentState.Except(tileResults).ToList();

        Assert.AreEqual(1, otherTiles.Count);
        Assert.AreEqual(1, otherTiles.Count(t => t.Entropy == -2));
    }

    [TestMethod]
    public void Should_Process_Four_Steps()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2));

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(4, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[0].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[1].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[2].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[3].TileChoice);

        var otherTiles = waveFunctionCollapse.CurrentState.Except(tileResults).ToList();

        Assert.AreEqual(0, otherTiles.Count);
    }

    [TestMethod]
    public void Should_Execute_All_Steps()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2));

        // Act
        var result = waveFunctionCollapse.Execute();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.CurrentState.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(4, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[0].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[1].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[2].TileChoice);
        Assert.AreEqual(waveFunctionCollapse.Tiles[0], tileResults[3].TileChoice);

        var otherTiles = waveFunctionCollapse.CurrentState.Except(tileResults).ToList();

        Assert.AreEqual(0, otherTiles.Count);
    }

    [TestMethod]
    public void Should_Set_Initialised_Tiles_With_Rule_For_That_Tile()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributesWithInitialisation = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        InitialisationRule = "([X] == 0 || [X] == [MaxX] || [Y] == 0 || [Y] == [MaxY])"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributesWithInitialisation);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(3, 3));

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(9, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(8, tileResults.Count);

        var midPoint = new Point(1, 1);
        Assert.IsFalse(waveFunctionCollapse.CurrentState.Single(t => t.Point == midPoint).IsCollapsed);
        Assert.IsTrue(tileResults.All(t => t.TileChoice == waveFunctionCollapse.Tiles[0]));
    }

    [TestMethod]
    public void Should_Complete_After_All_Steps_Done()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2));

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsTrue(result.IsComplete);
    }

    [TestMethod]
    public void Should_Fail_If_No_Valid_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        var failingTexture = new Texture2D(GraphicsDevice, 3, 3);

        var textures = new Dictionary<string, Texture2D>
        {
            { "FailingTexture", failingTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "FailingTexture", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        var waveFunctionCollapseGeneratorOptions = new WaveFunctionCollapseGeneratorOptions(2, 1);
        waveFunctionCollapseGeneratorOptions.FallbackAttempts = 0;

        waveFunctionCollapse.Reset(waveFunctionCollapseGeneratorOptions);

        // Act
        var result1 = waveFunctionCollapse.ExecuteNextStep();
        var result2 = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsFalse(result1.IsFailed);
        Assert.IsFalse(result1.IsComplete);

        Assert.IsTrue(result2.IsFailed);
        Assert.IsFalse(result2.IsComplete);
    }

    [TestMethod]
    public void Should_Roll_Back_If_No_Valid_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        var failingTexture = new Texture2D(GraphicsDevice, 3, 3);

        var textures = new Dictionary<string, Texture2D>
        {
            { "FailingTexture", failingTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "FailingTexture", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        var waveFunctionCollapseGeneratorOptions = new WaveFunctionCollapseGeneratorOptions(2, 1);

        waveFunctionCollapse.Reset(waveFunctionCollapseGeneratorOptions);

        // Act
        var result1 = waveFunctionCollapse.ExecuteNextStep();
        var result2 = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsFalse(result1.IsFailed);
        Assert.IsFalse(result1.IsComplete);

        Assert.IsFalse(result2.IsFailed);
        Assert.IsFalse(result2.IsComplete);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(0, tileResults.Count);
    }

    [TestMethod]
    public void Should_Fail_After_All_Attempts_Used()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        var failingTexture = new Texture2D(GraphicsDevice, 3, 3);

        var textures = new Dictionary<string, Texture2D>
        {
            { "FailingTexture", failingTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "FailingTexture", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        var waveFunctionCollapseGeneratorOptions = new WaveFunctionCollapseGeneratorOptions(2, 1);
        waveFunctionCollapseGeneratorOptions.FallbackAttempts = 1;

        waveFunctionCollapse.Reset(waveFunctionCollapseGeneratorOptions);

        // Act
        var result1 = waveFunctionCollapse.ExecuteNextStep();

        // Rolls back first tile
        var result2 = waveFunctionCollapse.ExecuteNextStep();

        // Places tile again
        var result3 = waveFunctionCollapse.ExecuteNextStep();

        // Second failure results in failure of procedure
        var result4 = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsFalse(result1.IsFailed);
        Assert.IsFalse(result1.IsComplete);

        Assert.IsFalse(result2.IsFailed);
        Assert.IsFalse(result2.IsComplete);

        Assert.IsFalse(result3.IsFailed);
        Assert.IsFalse(result3.IsComplete);

        Assert.IsTrue(result4.IsFailed);
        Assert.IsFalse(result4.IsComplete);
    }

    [TestMethod]
    public void Limited_Tiles_Should_Only_Place_Up_To_Their_Limit()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributesWithInitialisation = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        Limit = 2
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributesWithInitialisation);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 3) { FallbackAttempts = 0 });

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(3, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
    }

    [TestMethod]
    public void Limited_Tiles_Should_Only_Place_Up_To_Their_Limit_After_Reset()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributesWithInitialisation = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        Limit = 1
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributesWithInitialisation);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 2) { FallbackAttempts = 0 });
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        Assert.AreEqual(2, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);

        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 2) { FallbackAttempts = 0 });

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
    }

    [TestMethod]
    public void Limited_Tiles_Should_Only_Place_Up_To_Their_Limit_With_Retries()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributesWithInitialisation = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        Limit = 1
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributesWithInitialisation);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 2) { FallbackAttempts = 1 });

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
    }

    [TestMethod]
    public void Should_Throw_Exception_If_Limit_Is_Zero_And_CanExceedLimitIfOnlyValidTile_Is_False()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributesWithInitialisation = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        Limit = 0,
                        CanExceedLimitIfOnlyValidTile = false
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributesWithInitialisation);

        // Act
        var exception = Assert.ThrowsException<Exception>(() => waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 3)));

        // Assert
        Assert.AreEqual($"Tile Floor is defined with a Limit of zero and CanExceedLimitIfOnlyValidTile set to false.  This is not allowed as it would mean no tiles could be placed.", exception.Message);
    }

    [TestMethod]
    public void Limited_Tiles_Should_Exceed_Their_Limit_If_CanExceedLimitIfOnlyValidTile_Is_Set()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        Limit = 2,
                        CanExceedLimitIfOnlyValidTile = true
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 3));

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(3, waveFunctionCollapse.CurrentState.Length);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(3, tileResults.Count);
    }

    [TestMethod]
    public void Limited_Tiles_Should_Exceed_Their_Limit_If_CanExceedLimitIfOnlyValidTile_Is_Set_For_Initial_Limit_Of_Zero()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        Limit = 0,
                        CanExceedLimitIfOnlyValidTile = true
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 3));

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(3, waveFunctionCollapse.CurrentState.Length);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(3, tileResults.Count);
    }

    [TestMethod]
    public void Should_Not_Place_Tile_If_Mandatory_Adapter_Defined_And_Tile_Has_No_Mandatory_Adapters_Nearby()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Corner", _cornerTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,C",
                        InitialisationRule = "[X] == 0"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,B,A,A", // This tile can match on the left but due to lack of a neighbour on the right to match the mandatory adapter "B" it cannot be placed
                        MandatoryAdapters = "B"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 1) { FallbackAttempts = 0});

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.CurrentState.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
    }

    [TestMethod]
    public void Should_Place_Tile_If_Mandatory_Adapter_Defined_And_Tile_Has_Mandatory_Adapter_Nearby()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Corner", _cornerTexture },
            { "Line", _cornerTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        InitialisationRule = "[X] == 0"
                    }
                },
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "B,B,B,B",
                        InitialisationRule = "[X] == 2"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,B,A,A",
                        MandatoryAdapters = "B"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);

        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(3, 1) { FallbackAttempts = 0});

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(3, waveFunctionCollapse.CurrentState.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.CurrentState
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(3, tileResults.Count);
    }

    [TestMethod]
    public void Entropy_Should_ReduceByCountOfNeighbours()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 3,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2) { EntropyCalculationMethod = EntropyCalculationMethod.ReduceByCountOfNeighbours});

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.CurrentState.Single(t => !t.IsCollapsed);

        Assert.AreEqual(-2, remainingTile.Entropy);
    }

    [TestMethod]
    public void Entropy_Should_ReduceByWeightOfNeighbours()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 3,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2) { EntropyCalculationMethod = EntropyCalculationMethod.ReduceByWeightOfNeighbours});

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.CurrentState.Single(t => !t.IsCollapsed);

        Assert.AreEqual(-6, remainingTile.Entropy);
    }

    [TestMethod]
    public void Entropy_Should_ReduceByCountAndMaxWeightOfNeighbours()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 3,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(2, 2) { EntropyCalculationMethod = EntropyCalculationMethod.ReduceByCountAndMaxWeightOfNeighbours});

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.CurrentState.Single(t => !t.IsCollapsed);

        Assert.AreEqual(-5, remainingTile.Entropy);
    }

    [TestMethod]
    public void Entropy_Should_ReduceByCountOfAllTilesMinusPossibleTiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Corner", _cornerTexture }
        };

        var tileAttributes = new TileAttributes
        {
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "/",
                        Weight = 3,
                        Adapters = "AAA,AAA,AAA,AAA",
                        InitialisationRule = "[Y] == 0"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "/",
                        Weight = 3,
                        Adapters = "BBB,BBB,BBB,BBB"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, tileAttributes);
        waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(1, 2) { EntropyCalculationMethod = EntropyCalculationMethod.ReduceByCountOfAllTilesMinusPossibleTiles});

        // Act
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.CurrentState.Single(t => !t.IsCollapsed);

        Assert.AreEqual(-2, remainingTile.Entropy);
    }

    private void AssertTileResult(TileResult tileResult, TileChoice expectedTileChoice, TileResult[] expectedNeighbours,
        int expectedEntropy, bool expectedCollapsed)
    {
        Assert.AreEqual(expectedTileChoice, tileResult.TileChoice);
        CollectionAssert.AreEquivalent(expectedNeighbours, tileResult.Neighbours);
        Assert.AreEqual(expectedEntropy, tileResult.Entropy);
        Assert.AreEqual(expectedCollapsed, tileResult.IsCollapsed);
    }

    private void AssertTile(
        TileChoice tileChoice,
        string expectedAdapters,
        Texture2D expectedTexture,
        SpriteEffects expectedSpriteEffect = SpriteEffects.None,
        float expectedRotation = 0f)
    {
        var adapters = expectedAdapters.Split(",");

        Assert.AreEqual(adapters[0], tileChoice.Adapters[Direction.Up].Pattern);
        Assert.AreEqual(adapters[1], tileChoice.Adapters[Direction.Right].Pattern);
        Assert.AreEqual(adapters[2], tileChoice.Adapters[Direction.Down].Pattern);
        Assert.AreEqual(adapters[3], tileChoice.Adapters[Direction.Left].Pattern);

        Assert.AreEqual(expectedTexture, tileChoice.Texture);
        Assert.AreEqual(expectedSpriteEffect, tileChoice.SpriteEffects);
        Assert.AreEqual(expectedRotation, tileChoice.Rotation);
    }
}
