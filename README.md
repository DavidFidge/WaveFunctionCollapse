# WaveFunctionCollapse
A C# Wave Function Collapse library for MonoGame plus a tool with samples.  This is designed for one of my games, thus it currently takes a dependency on DavidFidge/FrigidRogue.  You could easily remove this and only take a dependency on Chris3606/GoRogue, an excellent, polished, performant roguelike C# library.

This implementation is a "simple tiled" implementation and comes with a host of features and options.

## Projects

The sample project is WaveFunctionCollapse.  It uses my game's view engine which makes use of GeonBit.UI and my own library.

The library project is called FrigidRogue.WaveFunctionCollapse.

Test coverage is comprehensive.

## Instructions

### Step 1 - CreateTiles
The main class is WaveFunctionCollapseGeneratorPasses.  You must call CreatePasses to first to load the tiles and rules.  There are several overloads which offer either code-defined configuration or json-defined configuration.

#### Json-defined configuration

Create a folder under Content and place your tiles in there.  Create a file called Rules.json also in the directory.

The Rules.json file defines the properties and rules of each tile.  Here is an example file:

```
{
  "MapOptions":
  {
    "MapWidth": 20,
    "MapHeight": 20
  },
  "Passes":
  [
    {
      "Options": {
        "FallbackAttempts": 999,
        "FallbackRadius": 2,
        "FallbackRadiusIncrement": 0,
        "EntropyHeuristic": "ReduceByCountAndMaxWeightOfNeighbours"
      },
      "Tiles": {
        "Floor": {
          "Symmetry": "X",
          "Weight": 50,
          "Adapters": "A,B,A,B",
          "FlipHorizontally": true
        }
      }
    }
  ]
}
```
MapOptions: defines the width and height of the map to generate. 

    MapWidth - width of map to create
    
    MapHeight - height of map to create
    
    TileSizeMultiplier - used for drawing code
    
Passes: defines a list of passes to perform.  Each pass runs its own wave function collapse process over the tiles.  Subsequent passes can have a mask defined which will only run the wave function collapse process over the tiles that match the mask.  This mask is defined as the pass number and a list of textures where those squares that have a texture in the list will be considered for the wave function collapse process.  If the mask is not defined then all tiles are considered.

Options:

    FallbackAttempts - total amount of retries to perform if a collapse fails at any stage.  This is a total number, not local to any particular failure point, as are the other Fallback-properties below.
    
    FallbackRadius - if a tile cannot be placed in a spot due to there being no valid tile that matches the exist~~~~ing neighbours then any assigned tiles in a radius of FallbackRadius are cleared (and entropy is set to ensure all these tiles are collapsed next).

    FallbackRadiusIncrement - After a failure, the FallbackRadius is incremented by this figure, meaning a bigger area is cleared each time.

    SuccessfullyPlacedTilesToReduceFallbackRadius - after this many tiles are placed successfully in a row then the FallbackRadius is reduced back towards its original value.

    RunFirstRules: Array of strings defining rules on which tiles should run first.  The tiles involved have their entropy reduced, making the wave collapse algorithm run them first.  Rules must follow the same semantics as PlacementRule (defined in Tiles section below).
    
    EntropyHeuristic - the heuristic to use to reduce entropy.  There are currently six options:

        ReduceByCountOfNeighbours,
        ReduceByWeightOfNeighbours,
        ReduceByMaxWeightOfNeighbours,
        ReduceByCountAndWeightOfNeighbours,
        ReduceByCountAndMaxWeightOfNeighbours,
        ReduceByCountOfAllTilesMinusPossibleTiles
        
    PassMask - the mask to use for this pass.  This is a list of tiles from previous passes which will allow that tile to be used in this pass.  If not defined then all tiles are considered.  Example:

        "PassMask": {
          "0": ["Floor"]
        }

    In the above example, only tiles that have been assigned the texture "Floor" in pass 0 will be considered for this pass.


Tiles:

defines a list of tiles.  The key is the name of the tile filename without extension or path, the value is a collection of properties.

Symmetry: typically you don't want to create multiple different tile graphics if you can simply reuse the existing tile by rotating it in some way.  This setting allows you to do this. There are currently four symmetry types,  X, I, ^ and /.  X is a fully symmetrical tile.  I is a 2-way symmetric tile (i.e. the tile maps out to two distinct tiles, one normal and one rotated 90 degrees). ^ is a 4-way symmetric tile (i.e. the tile maps out to four distinct tiles, one normal and three rotated 90, 180 and 270 degrees).  / is a 2-way symmetrical tile with the symmetry being a 180 degree rotation.  Adapters are automatically recalculated for the tile variations.

FlipHorizontally: Like the symmetry rules but allows a tile graphic to be flipped horizontally (different from rotating!).  Adapters are automatically recalculated for the tile variations.

FlipVertically: as per flip horizontally but vertically instead.  Using both FlipHorizontally and FlipVertically will give you 4 new tiles - one flipped horizontally, one flipped vertically and one flipped horizontally then vertically.

Weight: The weight is the chance of the tile being chosen from the list of valid tiles.  Also used for reducing entropy when one of the "Weights" heuristics is being used.

Adapters: The adapters define the allowed connection to other tiles.  Each direction must be specified in the order of up, right, down, left.  You can include a pipe symbol to match against multiple patterns e.g. "A|B,A,A,A"  It can be any string whatsoever, however the order of the string is important. The ordering is clockwise starting from the north side of the tile. So for example a tile defined as "AB,AB,AB,AB" cannot join on itself.  It must be defined as "AB,AB,BA,BA" - a visualisation of the ordering is below:

```
    AB→
 ↑  --
 A |  | A
 B |  | B
    --  ↓
   ←AB
```

MandatoryAdapters: A tile can only be placed if an existing collapsed neighbour in any direction matches against a mandatory adapter.  You use the pipe symbol to designate more than one mandatory adapters e.g. "A|B" will mean at least one of the neighbours must match against A or B.  Specific directions for the source tile are currently not supported.

PlacementRule: An expression that is evaluated to determine if the tile can be placed at a given location (refer to the NCalc GitHub project).  The variables [X] and [Y] are the location of the tile being considered, [MaxX] and [MaxY] are the maximum X and Y values of the map.  The expression must return a boolean.

Limit: The maximum number of times this tile can be used in the map.  If not defined then there is no limit.

OnlyAllowedIfNoValidTilesConstraint: If true, this tile can only be placed if this tile (and other tiles with this flag set) is the only valid tile that can be placed.  This is a useful way of constructing multi-tile objects that must be placed together.

ProhibitedEmptyNeighbourRules: Flags that define whether this tile can be placed if this tile is on the edge of the map or a neighbour is uncollapsed or unused.  Possible values are None|Uncollapsed|Unused|EdgeOfMap.  You can also use All which is the same as Uncollapsed|Unused|EdgeOfMap.  Each direction must be specified in the order of up, right, down, left (like adapters).  The rules are automatically translated for tiles that are flipped or have symmetrical copies.

EntropyWeights: When using one of the "Weights" heuristics then instead of using the Weight value of the tile, use the weight specified in this list for the given direction. Each direction must be specified in the order of up, right, down, left (like adapters) in the form of a string e.g. "20,30,40,50".  The weights are automatically translated for tiles that are flipped or have symmetrical copies.  If no value is given to EntropyWeights then it uses the Weight value instead.

#### Code-defined configuration
Code defined configuration mirrors the json-defined configuration - you pass in a dictionary of tile names to texture 2D objects and pass in a Rules object which contains all rules as defined above.

### Step 2 - Reset
Next, call Reset.  Call Reset every time you want to create a new map.

### Step 3 - ExecuteNextStep OR Execute 

Keep on calling ExecuteNextStep until IsComplete or IsFailed returns true.  Each ExecuteNextStep will 'collapse' (assign) a tile one by one (or perform a rollback attempt if the chosen tile cannot be collapsed).

If you don't want to call ExecuteNextStep one by one, call Execute instead which will generate the entire map all at once.

### Step 4 - Get the result

Call GetCurrentTiles() to get the results, ordered by pass.  Note that there are separate tiles for the same point for each wave function collapse pass.  It is up to you to combine them into one texture.

## Examples

See the WaveFunctionCollapse project for a series of example projects which will help you understand the various features.

