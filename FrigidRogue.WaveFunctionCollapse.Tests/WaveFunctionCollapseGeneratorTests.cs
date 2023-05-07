using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.TestInfrastructure;
using FrigidRogue.WaveFunctionCollapse.Options;
using GoRogue.Random;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;

namespace FrigidRogue.WaveFunctionCollapse.Tests;

[TestClass]
public class WaveFunctionCollapseGeneratorTests : BaseGraphicsTest
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "^",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL",
                        ProhibitedEmptyNeighbourRules = "None,None,All,All"
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);

        // Assert
        var tiles = waveFunctionCollapse.TileChoices;

        Assert.AreEqual(4, tiles.Count);

        AssertTile(tiles[0], "ABC,DEF,GHI,JKL", "None,None,All,All", _floorTexture);
        AssertTile(tiles[1], "JKL,ABC,DEF,GHI", "All,None,None,All", _floorTexture, expectedRotation: (float)Math.PI / 2);
        AssertTile(tiles[2], "GHI,JKL,ABC,DEF", "All,All,None,None", _floorTexture, expectedRotation: (float)Math.PI);
        AssertTile(tiles[3], "DEF,GHI,JKL,ABC", "None,All,All,None", _floorTexture, expectedRotation: (float)-Math.PI / 2);
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "I",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL",
                        ProhibitedEmptyNeighbourRules = "None,None,All,All"
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);

        // Assert
        var tiles = waveFunctionCollapse.TileChoices;

        Assert.AreEqual(2, tiles.Count);

        AssertTile(tiles[0], "ABC,DEF,GHI,JKL", "None,None,All,All", _floorTexture);
        AssertTile(tiles[1], "JKL,ABC,DEF,GHI", "All,None,None,All", _floorTexture, expectedRotation: (float)Math.PI / 2);
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "/",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL",
                        ProhibitedEmptyNeighbourRules = "None,None,All,All"
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);

        // Assert
        var tiles = waveFunctionCollapse.TileChoices;

        Assert.AreEqual(2, tiles.Count);

        AssertTile(tiles[0], "ABC,DEF,GHI,JKL", "None,None,All,All", _floorTexture);
        AssertTile(tiles[1], "GHI,JKL,ABC,DEF", "All,All,None,None", _floorTexture, expectedRotation: (float)Math.PI);
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        ProhibitedEmptyNeighbourRules = "None,None,All,All"
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);

        // Assert
        var tiles = waveFunctionCollapse.TileChoices;

        Assert.AreEqual(1, tiles.Count);

        AssertTile(tiles[0], "AAA,AAA,AAA,AAA", "None,None,All,All", _floorTexture);
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);

        // Act
        waveFunctionCollapse.Prepare(null);

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.Tiles.Length);
        
        foreach (var tile in waveFunctionCollapse.Tiles)
        {
            AssertTileResult(
                tile,
                null,
                tile.Point
                    .Neighbours(1, 1, AdjacencyRule.Types.Cardinals)
                    .Select(p => waveFunctionCollapse.Tiles.First(t => t.Point == p))
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 0 },
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 2), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 0 },
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 2), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
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
            { "Floor", _floorTexture }
        };

        var passOptionsWithInitialisation = new PassOptions
        {
            Options = new GeneratorOptions { RunFirstRules = new[] { "[Y] == 0", "[Y] == 2" } },
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

        waveFunctionCollapse.CreateTiles(textures, passOptionsWithInitialisation, new MapOptions(1, 3), GlobalRandom.DefaultRNG);

        // Act
        waveFunctionCollapse.Prepare(null);

        // Assert
        Assert.AreEqual(3, waveFunctionCollapse.Tiles.Length);

        Assert.AreEqual(-Int32.MaxValue / 2, waveFunctionCollapse.Tiles[0].Entropy);
        Assert.AreEqual(0, waveFunctionCollapse.Tiles[1].Entropy);
        Assert.AreEqual(-Int32.MaxValue / 3, waveFunctionCollapse.Tiles[2].Entropy);
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours},
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);
        
        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[0].ChosenTile);

        var otherTiles = waveFunctionCollapse.Tiles.Except(tileResults).ToList();

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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours },
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);
        
        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[0].ChosenTile);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[1].ChosenTile);

        var otherTiles = waveFunctionCollapse.Tiles.Except(tileResults).ToList();

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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours },
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(3, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[0].ChosenTile);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[1].ChosenTile);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[2].ChosenTile);

        var otherTiles = waveFunctionCollapse.Tiles.Except(tileResults).ToList();

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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.Tiles.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(4, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[0].ChosenTile);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[1].ChosenTile);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[2].ChosenTile);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[3].ChosenTile);

        var otherTiles = waveFunctionCollapse.Tiles.Except(tileResults).ToList();

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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        var result = waveFunctionCollapse.Execute();

        // Assert
        Assert.AreEqual(4, waveFunctionCollapse.Tiles.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(4, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[0].ChosenTile);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[1].ChosenTile);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[2].ChosenTile);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[3].ChosenTile);

        var otherTiles = waveFunctionCollapse.Tiles.Except(tileResults).ToList();

        Assert.AreEqual(0, otherTiles.Count);
    }

    [TestMethod]
    public void Should_Allocate_Initial_Tiles_First()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptionsWithInitialisation = new PassOptions
        {
            Options = new GeneratorOptions() { RunFirstRules = new []{ "([X] == 0 || [X] == [MaxX] || [Y] == 0 || [Y] == [MaxY])" }},
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

        waveFunctionCollapse.CreateTiles(textures, passOptionsWithInitialisation, new MapOptions(3, 3), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

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
        Assert.AreEqual(9, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(8, tileResults.Count);

        var midPoint = new Point(1, 1);
        Assert.IsFalse(waveFunctionCollapse.Tiles.Single(t => t.Point == midPoint).IsCollapsed);
        Assert.IsTrue(tileResults.All(t => t.ChosenTile == waveFunctionCollapse.TileChoices[0]));
    }

    [TestMethod]
    public void Should_Fail_When_CanConnectToSelf_Is_False()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptionsWithInitialisation = new PassOptions
        {
            Options = new GeneratorOptions { FallbackAttempts = 0 },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        CanConnectToSelf = "false,false,false,false"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptionsWithInitialisation, new MapOptions(1, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

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

        var waveFunctionCollapseGeneratorOptions = new GeneratorOptions();
        waveFunctionCollapseGeneratorOptions.FallbackAttempts = 0;

        var passOptions = new PassOptions
        {
            Options = waveFunctionCollapseGeneratorOptions,
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 1), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

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

        var waveFunctionCollapseGeneratorOptions = new GeneratorOptions();

        var passOptions = new PassOptions
        {
            Options = waveFunctionCollapseGeneratorOptions,
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 1), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        var result1 = waveFunctionCollapse.ExecuteNextStep();
        var result2 = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsFalse(result1.IsFailed);
        Assert.IsFalse(result1.IsComplete);

        Assert.IsFalse(result2.IsFailed);
        Assert.IsFalse(result2.IsComplete);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(0, tileResults.Count);
    }

    [TestMethod]
    public void Should_Recalculate_Entropy_On_Roll_Back()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        var failingTexture = new Texture2D(GraphicsDevice, 3, 3);

        var textures = new Dictionary<string, Texture2D>
        {
            { "FailingTexture", failingTexture }
        };

        var waveFunctionCollapseGeneratorOptions = new GeneratorOptions
        {
            FallbackAttempts = 1,
            FallbackRadiusIncrement = 0,
            FallbackRadius = 1,
            RunFirstRules = new [] {"[X] == 1", "[X] == 0 && [Y] == 2"},
            EntropyHeuristic = EntropyHeuristic.ReduceByMaxWeightOfNeighbours
        };

        var passOptions = new PassOptions
        {
            Options = waveFunctionCollapseGeneratorOptions,
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "FailingTexture", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 5,
                        Adapters = "A,DEF,A,JKL",
                        EntropyWeights = "10,11,12,100"
                    }
                }
            }
        };

        var mapOptions = new MapOptions(3, 3);
        waveFunctionCollapse.CreateTiles(textures, passOptions, mapOptions, GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);

        Assert.AreEqual(waveFunctionCollapse.Tiles[new Point(0, 0).ToIndex(mapOptions.MapWidth)].Entropy, -100);
        Assert.IsTrue(waveFunctionCollapse.Tiles[new Point(1, 0).ToIndex(mapOptions.MapWidth)].IsCollapsed);
        Assert.AreEqual(waveFunctionCollapse.Tiles[new Point(2, 0).ToIndex(mapOptions.MapWidth)].Entropy, -11);
        Assert.AreEqual(waveFunctionCollapse.Tiles[new Point(0, 1).ToIndex(mapOptions.MapWidth)].Entropy, 0);
        Assert.AreEqual(waveFunctionCollapse.Tiles[new Point(1, 1).ToIndex(mapOptions.MapWidth)].Entropy, (-Int32.MaxValue / 2) - 12);
        Assert.AreEqual(waveFunctionCollapse.Tiles[new Point(2, 1).ToIndex(mapOptions.MapWidth)].Entropy, 0);
        Assert.AreEqual(waveFunctionCollapse.Tiles[new Point(0, 2).ToIndex(mapOptions.MapWidth)].Entropy, -Int32.MaxValue / 3);
        Assert.AreEqual(waveFunctionCollapse.Tiles[new Point(1, 2).ToIndex(mapOptions.MapWidth)].Entropy, -Int32.MaxValue / 2);
        Assert.AreEqual(waveFunctionCollapse.Tiles[new Point(2, 2).ToIndex(mapOptions.MapWidth)].Entropy, 0);
    }

    [TestMethod]
    public void Should_Increment_Radius_When_Roll_Back_Occurs()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        var failingTexture = new Texture2D(GraphicsDevice, 3, 3);

        var textures = new Dictionary<string, Texture2D>
        {
            { "FailingTexture", failingTexture }
        };

        var waveFunctionCollapseGeneratorOptions = new GeneratorOptions
        {
            FallbackRadius = 1,
            FallbackRadiusIncrement = 1,
            RunFirstRules = new []{ "[X] == 0" }
        };

        var passOptions = new PassOptions
        {
            Options = waveFunctionCollapseGeneratorOptions,
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "FailingTexture", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        PlacementRule = "[X] <= 2"
                    }
                },
                {
                    "FailingTexture2", new TileAttribute
                    {
                        TextureName = "FailingTexture",
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "Z,Z,Z,Z",
                        PlacementRule = "[X] > 2"
                    }               
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(4, 1), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.IsFalse(result.IsComplete);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
        
        Assert.IsTrue(waveFunctionCollapse.Tiles.First().IsCollapsed);
    }
    
    [TestMethod]
    public void Should_Decrement_Rollback_Radius_When_Successfully_Placed_Enough_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        var failingTexture = new Texture2D(GraphicsDevice, 3, 3);

        var textures = new Dictionary<string, Texture2D>
        {
            { "FailingTexture", failingTexture }
        };

        var waveFunctionCollapseGeneratorOptions = new GeneratorOptions
        {
            FallbackRadius = 1,
            FallbackRadiusIncrement = 1,
            SuccessfullyPlacedTilesToReduceFallbackRadius = 1,
            RunFirstRules = new []{ "[X] == 0" }
        };

        var passOptions = new PassOptions
        {
            Options = waveFunctionCollapseGeneratorOptions,
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "FailingTexture", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        PlacementRule = "[X] <= 2"
                    }
                },
                {
                    "FailingTexture2", new TileAttribute
                    {
                        TextureName = "FailingTexture",
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "Z,Z,Z,Z",
                        PlacementRule = "[X] > 2"
                    }               
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(4, 1), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.IsFalse(result.IsComplete);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
        
        Assert.IsTrue(waveFunctionCollapse.Tiles.First().IsCollapsed);
        Assert.IsTrue(waveFunctionCollapse.Tiles.Skip(1).First().IsCollapsed);
    }

    [TestMethod]
    public void Should_Decrement_Rollback_Radius_When_Successfully_Placed_Enough_Tiles_Using_Formula()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        var failingTexture = new Texture2D(GraphicsDevice, 3, 3);

        var textures = new Dictionary<string, Texture2D>
        {
            { "FailingTexture", failingTexture }
        };

        var waveFunctionCollapseGeneratorOptions = new GeneratorOptions
        {
            FallbackRadius = 1,
            FallbackRadiusIncrement = 1,
            SuccessfullyPlacedTilesToReduceFallbackRadiusFormula = "[MaxX] + [MaxY] + [MapWidth] + [MapHeight] - 7", // = 1
            RunFirstRules = new []{ "[X] == 0" }
        };

        var passOptions = new PassOptions
        {
            Options = waveFunctionCollapseGeneratorOptions,
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "FailingTexture", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        PlacementRule = "[X] <= 2"
                    }
                },
                {
                    "FailingTexture2", new TileAttribute
                    {
                        TextureName = "FailingTexture",
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "Z,Z,Z,Z",
                        PlacementRule = "[X] > 2"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(4, 1), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.IsFalse(result.IsComplete);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
        
        Assert.IsTrue(waveFunctionCollapse.Tiles.First().IsCollapsed);
        Assert.IsTrue(waveFunctionCollapse.Tiles.Skip(1).First().IsCollapsed);
    }

    [TestMethod]
    public void Should_Roll_Back_Tiles_Within_Fallback_Radius()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        var failingTexture = new Texture2D(GraphicsDevice, 3, 3);

        var textures = new Dictionary<string, Texture2D>
        {
            { "FailingTexture", failingTexture }
        };

        var waveFunctionCollapseGeneratorOptions = new GeneratorOptions
        {
            FallbackRadius = 2,
            RunFirstRules = new []{ "[X] == 0" }
        };

        var passOptions = new PassOptions
        {
            Options = waveFunctionCollapseGeneratorOptions,
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "FailingTexture", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        PlacementRule = "[X] <= 2"
                    }
                },
                {
                    "FailingTexture2", new TileAttribute
                    {
                        TextureName = "FailingTexture",
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "Z,Z,Z,Z",
                        PlacementRule = "[X] > 2"
                    }               
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(4, 1), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.IsFalse(result.IsComplete);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
        
        Assert.IsTrue(waveFunctionCollapse.Tiles.First().IsCollapsed);
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

        var waveFunctionCollapseGeneratorOptions = new GeneratorOptions();
        waveFunctionCollapseGeneratorOptions.FallbackAttempts = 1;

        var passOptions = new PassOptions
        {
            Options = waveFunctionCollapseGeneratorOptions,
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 1), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

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

        var passOptionsWithInitialisation = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 0 },
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

        waveFunctionCollapse.CreateTiles(textures, passOptionsWithInitialisation, new MapOptions(1, 3), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(3, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
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

        var passOptionsWithInitialisation = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 0 },
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

        waveFunctionCollapse.CreateTiles(textures, passOptionsWithInitialisation, new MapOptions(1, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);

        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        tileResults = waveFunctionCollapse.Tiles
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

        var passOptionsWithInitialisation = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 1 },
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

        waveFunctionCollapse.CreateTiles(textures, passOptionsWithInitialisation, new MapOptions(1, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
    }

    [TestMethod]
    public void Limited_Tiles_Should_Not_Exceed_Their_Limit_If_OnlyAllowedIfNoValidTiles_Is_Set()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions { FallbackAttempts = 0 },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        Limit = 2,
                        OnlyAllowedIfNoValidTiles = true
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 3), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(3, waveFunctionCollapse.Tiles.Length);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
    }

    [TestMethod]
    public void Limit_Value_Of_Zero_Should_Not_Place_Tile()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 0 },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        Limit = 0
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 3), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        
        // Assert
        Assert.AreEqual(3, waveFunctionCollapse.Tiles.Length);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(0, tileResults.Count);
    }

    [TestMethod]
    public void Should_Place_Tile_If_Category_Matches()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Corner", _cornerTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions { FallbackAttempts = 0, RunFirstRules = new []{ "[X] == 0" }},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        Category = "D"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        Category = "D",
                        PlacementRule = "[X] == 1",
                        CanConnectToCategories = new [] { "D" }
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 1), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
    }

    [TestMethod]
    public void Should_Not_Place_Tile_If_Category_Does_Not_Match()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Corner", _cornerTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions { FallbackAttempts = 0, RunFirstRules = new []{ "[X] == 0" }},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        Category = "D",
                        PlacementRule = "[X] == 0"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        CanConnectToCategories = new [] { "F" },
                        PlacementRule = "[X] == 1"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 1), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
    }

    [TestMethod]
    public void Should_Not_Place_Tile_If_Adjacent_Square_Is_Uncollapsed_And_ProhibitedEmptyNeighbourRule_Is_Set_To_Uncollapsed_For_That_Direction()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions { FallbackAttempts = 0, RunFirstRules = new []{ "[X] == 0" } },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        ProhibitedEmptyNeighbourRules = "None,Uncollapsed,None,None"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 1), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

        // Act
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(0, tileResults.Count);
    }

    [TestMethod]
    public void Should_Not_Place_Tile_If_On_Edge_Of_Map_And_ProhibitedEmptyNeighbourRule_Is_Set_To_EdgeOfMap_For_That_Direction()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions { FallbackAttempts = 0 },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        ProhibitedEmptyNeighbourRules = "EdgeOfMap,None,None,None"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

        // Act
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(1, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(0, tileResults.Count);
    }


    [TestMethod]
    public void Should_Place_Tile_If_Adjacent_Square_Is_Not_Empty_And_ProhibitedEmptyNeighbourRule_Is_Not_Set_To_Uncollapsed_For_That_Direction()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Corner", _cornerTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions { FallbackAttempts = 0, RunFirstRules = new []{ "[X] == 1" }},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        Category = "D",
                        PlacementRule = "[X] == 0",
                        ProhibitedEmptyNeighbourRules = "None,All,None,None"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        CanConnectToCategories = new [] { "F" },
                        PlacementRule = "[X] == 1"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 1), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 0, RunFirstRules = new []{ "[X] == 0" }},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,C"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,B,A,A", // This tile can match on the left but due to lack of a neighbour on the right to match the mandatory adapter "B" it cannot be placed
                        MandatoryAdapters = "B",
                        PlacementRule = "[X] == 1"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 1), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(2, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 0, RunFirstRules = new []{ "[X] == 0", "[X] == 2" }},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,A,A,A",
                        PlacementRule = "[X] == 0"
                    }
                },
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "B,B,B,B",
                        PlacementRule = "[X] == 2"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "A,B,A,A",
                        MandatoryAdapters = "B",
                        PlacementRule = "[X] == 1"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(3, 1), GlobalRandom.DefaultRNG);

        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(3, waveFunctionCollapse.Tiles.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours},
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.Tiles.Single(t => !t.IsCollapsed);

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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { EntropyHeuristic = EntropyHeuristic.ReduceByWeightOfNeighbours},
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.Tiles.Single(t => !t.IsCollapsed);

        Assert.AreEqual(-6, remainingTile.Entropy);
    }


    [TestMethod]
    public void Entropy_Should_ReduceByWeightOfNeighbours_When_Using_EntropyWeights_Attribute()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { EntropyHeuristic = EntropyHeuristic.ReduceByWeightOfNeighbours, RunFirstRules = new []{"[X] == 1 && [Y] == 1"}},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 3,
                        Adapters = "AAA,AAA,AAA,AAA",
                        EntropyWeights = "1,11,111,1111"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(3, 3), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var tileUp = waveFunctionCollapse.Tiles[Point.ToIndex(1, 0, 3)];
        Assert.AreEqual(-1, tileUp.Entropy);

        var tileDown = waveFunctionCollapse.Tiles[Point.ToIndex(1, 2, 3)];
        Assert.AreEqual(-111, tileDown.Entropy);

        var tileRight = waveFunctionCollapse.Tiles[Point.ToIndex(2, 1, 3)];
        Assert.AreEqual(-11, tileRight.Entropy);

        var tileLeft = waveFunctionCollapse.Tiles[Point.ToIndex(0, 1, 3)];
        Assert.AreEqual(-1111, tileLeft.Entropy);
    }

    [TestMethod]
    public void Entropy_Should_ReduceByCountAndWeightOfNeighbours()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { EntropyHeuristic = EntropyHeuristic.ReduceByCountAndWeightOfNeighbours},
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

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.Tiles.Single(t => !t.IsCollapsed);

        Assert.AreEqual(-8, remainingTile.Entropy);
    }

    [TestMethod]
    public void Entropy_Should_ReduceByCountAndMaxWeightOfNeighbours()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Corner", _cornerTexture },
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions()
                { EntropyHeuristic = EntropyHeuristic.ReduceByCountAndMaxWeightOfNeighbours, RunFirstRules = new[] {"[X] == 0 || [Y] == 0"}},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 3,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 1"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 10,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 0"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.Tiles.Single(t => !t.IsCollapsed);

        Assert.AreEqual(-12, remainingTile.Entropy);
    }

    [TestMethod]
    public void Entropy_Should_ReduceByMaxWeightOfNeighbours()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Corner", _cornerTexture },
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions()
                { EntropyHeuristic = EntropyHeuristic.ReduceByMaxWeightOfNeighbours, RunFirstRules = new[] {"[X] == 0 || [Y] == 0"}},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 3,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 1"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 10,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 0"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(2, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.Tiles.Single(t => !t.IsCollapsed);

        Assert.AreEqual(-10, remainingTile.Entropy);
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

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { EntropyHeuristic = EntropyHeuristic.ReduceByCountOfAllTilesMinusPossibleTiles, RunFirstRules = new []{ "[Y] == 0" }},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "/",
                        Weight = 3,
                        Adapters = "AAA,AAA,AAA,AAA"
                    }
                },
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "/",
                        Weight = 3,
                        Adapters = "BBB,BBB,BBB,BBB",
                        PlacementRule = "[Y] != 0"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 2), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        waveFunctionCollapse.ExecuteNextStep();

        // Assert
        var remainingTile = waveFunctionCollapse.Tiles.Single(t => !t.IsCollapsed);

        Assert.AreEqual(-2, remainingTile.Entropy);
    }

    [TestMethod]
    public void Mirrored_Horizontally_Should_Process_Into_Two_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL",
                        ProhibitedEmptyNeighbourRules = "None,None,All,All",
                        FlipHorizontally = true
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);

        // Assert
        var tiles = waveFunctionCollapse.TileChoices;

        Assert.AreEqual(2, tiles.Count);

        AssertTile(tiles[0], "ABC,DEF,GHI,JKL", "None,None,All,All", _floorTexture, SpriteEffects.None);
        AssertTile(tiles[1], "CBA,LKJ,IHG,FED", "None,All,All,None", _floorTexture, SpriteEffects.FlipHorizontally);
    }

    [TestMethod]
    public void Mirrored_Vertically_Should_Process_Into_Two_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL",
                        ProhibitedEmptyNeighbourRules = "None,None,All,All",
                        FlipVertically = true
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);

        // Assert
        var tiles = waveFunctionCollapse.TileChoices;

        Assert.AreEqual(2, tiles.Count);

        AssertTile(tiles[0], "ABC,DEF,GHI,JKL", "None,None,All,All", _floorTexture, SpriteEffects.None);
        AssertTile(tiles[1], "IHG,FED,CBA,LKJ", "All,None,None,All", _floorTexture, SpriteEffects.FlipVertically);
    }

    [TestMethod]
    public void Mirrored_Vertically_And_Horizontally_Should_Process_Into_Two_Tiles()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions(),
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "ABC,DEF,GHI,JKL",
                        ProhibitedEmptyNeighbourRules = "None,None,All,All",
                        FlipHorizontally = true,
                        FlipVertically = true
                    }
                }
            }
        };

        // Act
        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);

        // Assert
        var tiles = waveFunctionCollapse.TileChoices;

        Assert.AreEqual(4, tiles.Count);

        AssertTile(tiles[0], "ABC,DEF,GHI,JKL", "None,None,All,All", _floorTexture, SpriteEffects.None);
        AssertTile(tiles[1], "GHI,JKL,ABC,DEF", "All,All,None,None", _floorTexture, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
        AssertTile(tiles[2], "CBA,LKJ,IHG,FED", "None,All,All,None", _floorTexture, SpriteEffects.FlipHorizontally);
        AssertTile(tiles[3], "IHG,FED,CBA,LKJ", "All,None,None,All", _floorTexture, SpriteEffects.FlipVertically);
    }

    [TestMethod]
    public void Should_Only_Place_Tiles_Within_PlacementRule_Successful_Placement()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 0 },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[X] == 0 && [Y] == 0"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(1, waveFunctionCollapse.Tiles.Length);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(1, tileResults.Count);
        Assert.AreEqual(waveFunctionCollapse.TileChoices[0], tileResults[0].ChosenTile);
    }

    [TestMethod]
    public void Should_Only_Place_Tiles_Within_PlacementRule_Failed_Placement()
    {
        // Arrange
        var waveFunctionCollapse = new WaveFunctionCollapseGenerator();

        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture }
        };

        var passOptions = new PassOptions
        {
            Options = new GeneratorOptions() { FallbackAttempts = 0 },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[X] == 1 && [Y] == 0"
                    }
                }
            }
        };

        waveFunctionCollapse.CreateTiles(textures, passOptions, new MapOptions(1, 1), GlobalRandom.DefaultRNG);
        waveFunctionCollapse.Prepare(null);

        // Act
        var result = waveFunctionCollapse.ExecuteNextStep();

        // Assert
        Assert.AreEqual(1, waveFunctionCollapse.Tiles.Length);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = waveFunctionCollapse.Tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(0, tileResults.Count);
    }

    [TestMethod]
    public void Should_Process_Two_Passes_With_No_Masking_Rule()
    {
        // Arrange
        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Line", _lineTexture }
        };

        var firstPass = new PassOptions
        {
            Options = new GeneratorOptions { EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours},
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

        var secondPass = new PassOptions
        {
            Options = new GeneratorOptions { EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "BBB,BBB,BBB,BBB"
                    }
                }
            }
        };

        var rules = new Rules
        {
            MapOptions = new MapOptions(1, 2),
            Passes = new[] { firstPass, secondPass }
        };

        var waveFunctionCollapseGeneratorPasses = new WaveFunctionCollapseGeneratorPasses();

        waveFunctionCollapseGeneratorPasses.CreatePasses(rules, textures);

        waveFunctionCollapseGeneratorPasses.Reset();

        // Act
        waveFunctionCollapseGeneratorPasses.ExecuteNextStep();
        waveFunctionCollapseGeneratorPasses.ExecuteNextStep();
        waveFunctionCollapseGeneratorPasses.ExecuteNextStep();
        var result = waveFunctionCollapseGeneratorPasses.ExecuteNextStep();

        // Assert
        var tiles = waveFunctionCollapseGeneratorPasses
            .GetAllTiles()
            .ToList();

        Assert.AreEqual(4, tiles.Count);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);
        
        var tileResults = tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(4, tileResults.Count);

        AssertTile(tileResults[0].ChosenTile, "AAA,AAA,AAA,AAA", _floorTexture);
        AssertTile(tileResults[1].ChosenTile, "AAA,AAA,AAA,AAA", _floorTexture);
        AssertTile(tileResults[2].ChosenTile, "BBB,BBB,BBB,BBB", _lineTexture);
        AssertTile(tileResults[3].ChosenTile, "BBB,BBB,BBB,BBB", _lineTexture);

        Assert.AreEqual(0, tiles.Count(t => t.IsUnused));
    }

    [TestMethod]
    public void Should_Process_Two_Passes_With_Masking_Rule()
    {
        // Arrange
        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Line", _lineTexture },
            { "Corner", _cornerTexture }
        };

        var firstPass = new PassOptions
        {
            Options = new GeneratorOptions { EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 0"
                    }
                },
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 1"
                    }
                }
            }
        };

        var secondPass = new PassOptions
        {
            Options = new GeneratorOptions
            {
                PassMask = new Dictionary<string, string[]> { { "0", new[] { "Floor" } } },
            },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "BBB,BBB,BBB,BBB",
                    }
                }
            }
        };

        var rules = new Rules
        {
            MapOptions = new MapOptions(1, 2),
            Passes = new[] { firstPass, secondPass }
        };

        var waveFunctionCollapseGeneratorPasses = new WaveFunctionCollapseGeneratorPasses();

        waveFunctionCollapseGeneratorPasses.CreatePasses(rules, textures);
        waveFunctionCollapseGeneratorPasses.Reset();

        // Act
        waveFunctionCollapseGeneratorPasses.ExecuteNextStep();
        waveFunctionCollapseGeneratorPasses.ExecuteNextStep();
        var result = waveFunctionCollapseGeneratorPasses.ExecuteNextStep();

        // Assert
        var tiles = waveFunctionCollapseGeneratorPasses
            .GetAllTiles()
            .ToList();

        Assert.AreEqual(4, tiles.Count);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);
        
        var tileResults = tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(3, tileResults.Count);

        AssertTile(tileResults[0].ChosenTile, "AAA,AAA,AAA,AAA", _floorTexture);
        AssertTile(tileResults[1].ChosenTile, "AAA,AAA,AAA,AAA", _lineTexture);
        AssertTile(tileResults[2].ChosenTile, "BBB,BBB,BBB,BBB", _cornerTexture);

        var point = new Point(0, 0);
        Assert.AreEqual(point, tileResults[0].Point);
        Assert.AreEqual(point, tileResults[2].Point);

        Assert.AreEqual(new Point(0, 1), tileResults[1].Point);

        Assert.AreEqual(1, tiles.Count(t => t.IsUnused));
    }

    [TestMethod]
    public void Should_Process_Two_Passes_With_Second_Pass_Failing_Due_To_ProhibitedEmptyNeighbour_Unused_Rule()
    {
        // Arrange
        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Line", _lineTexture },
            { "Corner", _cornerTexture }
        };

        var firstPass = new PassOptions
        {
            Options = new GeneratorOptions
            {
                EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours,
                FallbackAttempts = 0
            },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 0"
                    }
                },
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 1"
                    }
                }
            }
        };

        var secondPass = new PassOptions
        {
            Options = new GeneratorOptions
            {
                PassMask = new Dictionary<string, string[]> { { "0", new[] { "Floor" } } },
                FallbackAttempts = 0
            },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "BBB,BBB,BBB,BBB",
                        ProhibitedEmptyNeighbourRules = "None,None,Unused,None"
                    }
                }
            }
        };

        var rules = new Rules
        {
            MapOptions = new MapOptions(1, 2),
            Passes = new[] { firstPass, secondPass }
        };

        var waveFunctionCollapseGeneratorPasses = new WaveFunctionCollapseGeneratorPasses();

        waveFunctionCollapseGeneratorPasses.CreatePasses(rules, textures);
        waveFunctionCollapseGeneratorPasses.Reset();

        // Act
        waveFunctionCollapseGeneratorPasses.ExecuteNextStep();
        waveFunctionCollapseGeneratorPasses.ExecuteNextStep();
        var result = waveFunctionCollapseGeneratorPasses.ExecuteNextStep();

        // Assert
        var tiles = waveFunctionCollapseGeneratorPasses
            .GetAllTiles()
            .ToList();

        Assert.AreEqual(4, tiles.Count);
        Assert.IsFalse(result.IsComplete);
        Assert.IsTrue(result.IsFailed);

        var tileResults = tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(2, tileResults.Count);

        AssertTile(tileResults[0].ChosenTile, "AAA,AAA,AAA,AAA", _floorTexture);
        AssertTile(tileResults[1].ChosenTile, "AAA,AAA,AAA,AAA", _lineTexture);
    }

    [TestMethod]
    public void Should_Process_Two_Passes_With_Second_Pass_Succeeding_Due_To_ProhibitedEmptyNeighbour_Rules_Other_Than_Unused()
    {
        // Arrange
        var textures = new Dictionary<string, Texture2D>
        {
            { "Floor", _floorTexture },
            { "Line", _lineTexture },
            { "Corner", _cornerTexture }
        };

        var firstPass = new PassOptions
        {
            Options = new GeneratorOptions { EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours},
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Floor", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 0"
                    }
                },
                {
                    "Line", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "AAA,AAA,AAA,AAA",
                        PlacementRule = "[Y] == 1"
                    }
                }
            }
        };

        var secondPass = new PassOptions
        {
            Options = new GeneratorOptions
            {
                PassMask = new Dictionary<string, string[]> { { "0", new[] { "Floor" } } },
            },
            Tiles = new Dictionary<string, TileAttribute>
            {
                {
                    "Corner", new TileAttribute
                    {
                        Symmetry = "X",
                        Weight = 1,
                        Adapters = "BBB,BBB,BBB,BBB",
                        ProhibitedEmptyNeighbourRules = "None,None,Uncollapsed|EdgeOfMap,None"
                    }
                }
            }
        };

        var rules = new Rules
        {
            MapOptions = new MapOptions(1, 2),
            Passes = new[] { firstPass, secondPass }
        };

        var waveFunctionCollapseGeneratorPasses = new WaveFunctionCollapseGeneratorPasses();

        waveFunctionCollapseGeneratorPasses.CreatePasses(rules, textures);
        waveFunctionCollapseGeneratorPasses.Reset();

        // Act
        waveFunctionCollapseGeneratorPasses.ExecuteNextStep();
        waveFunctionCollapseGeneratorPasses.ExecuteNextStep();
        var result = waveFunctionCollapseGeneratorPasses.ExecuteNextStep();

        // Assert
        var tiles = waveFunctionCollapseGeneratorPasses
            .GetAllTiles()
            .ToList();

        Assert.AreEqual(4, tiles.Count);
        Assert.IsTrue(result.IsComplete);
        Assert.IsFalse(result.IsFailed);
        
        var tileResults = tiles
            .Where(c => c.IsCollapsed)
            .ToList();

        Assert.AreEqual(3, tileResults.Count);

        AssertTile(tileResults[0].ChosenTile, "AAA,AAA,AAA,AAA", _floorTexture);
        AssertTile(tileResults[1].ChosenTile, "AAA,AAA,AAA,AAA", _lineTexture);
        AssertTile(tileResults[2].ChosenTile, "BBB,BBB,BBB,BBB", secondPass.Tiles.First().Value.ProhibitedEmptyNeighbourRules, _cornerTexture);

        var point = new Point(0, 0);
        Assert.AreEqual(point, tileResults[0].Point);
        Assert.AreEqual(point, tileResults[2].Point);

        Assert.AreEqual(new Point(0, 1), tileResults[1].Point);

        Assert.AreEqual(1, tiles.Count(t => t.IsUnused));
    }

    private void AssertTileResult(TileResult tileResult, TileChoice expectedTileChoice, TileResult[] expectedNeighbours,
        int expectedEntropy, bool expectedCollapsed)
    {
        Assert.AreEqual(expectedTileChoice, tileResult.ChosenTile);
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
        AssertTile(tileChoice, expectedAdapters, "None,None,None,None", expectedTexture, expectedSpriteEffect, expectedRotation);
    }

    private void AssertTile(
        TileChoice tileChoice,
        string expectedAdapters,
        string expectedProhibitedEmptyNeighbourFlags,
        Texture2D expectedTexture,
        SpriteEffects expectedSpriteEffect = SpriteEffects.None,
        float expectedRotation = 0f)
    {
        var adapters = expectedAdapters.Split(",");

        Assert.AreEqual(adapters[0], tileChoice.Adapters[Direction.Up].Pattern);
        Assert.AreEqual(adapters[1], tileChoice.Adapters[Direction.Right].Pattern);
        Assert.AreEqual(adapters[2], tileChoice.Adapters[Direction.Down].Pattern);
        Assert.AreEqual(adapters[3], tileChoice.Adapters[Direction.Left].Pattern);

        var prohibitedEmptyNeighbourFlags = expectedProhibitedEmptyNeighbourFlags.Split(",");

        Assert.AreEqual(Enum.Parse<ProhibitedEmptyNeighbourFlags>(prohibitedEmptyNeighbourFlags[0].Replace("|",",")), tileChoice.ProhibitedEmptyNeighbours[Direction.Up]);
        Assert.AreEqual(Enum.Parse<ProhibitedEmptyNeighbourFlags>(prohibitedEmptyNeighbourFlags[1].Replace("|",",")), tileChoice.ProhibitedEmptyNeighbours[Direction.Right]);
        Assert.AreEqual(Enum.Parse<ProhibitedEmptyNeighbourFlags>(prohibitedEmptyNeighbourFlags[2].Replace("|",",")), tileChoice.ProhibitedEmptyNeighbours[Direction.Down]);
        Assert.AreEqual(Enum.Parse<ProhibitedEmptyNeighbourFlags>(prohibitedEmptyNeighbourFlags[3].Replace("|",",")), tileChoice.ProhibitedEmptyNeighbours[Direction.Left]);

        Assert.AreEqual(expectedTexture, tileChoice.Texture);
        Assert.AreEqual(expectedSpriteEffect, tileChoice.SpriteEffects);
        Assert.AreEqual(expectedRotation, tileChoice.Rotation);
    }
}
