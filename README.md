# WaveFunctionCollapse
A C# Wave Function Collapse library for MonoGame plus a tool with samples.  This is designed for one of my games, thus it takes a dependency on DavidFidge/FrigidRogue (plus some of its dependencies like GoRogue, SadRogue.Primitives, MonoGame.Extended etc).

This implementation is a tiled implementation and uses reducing entropy values to determine the next tile to place.

## Instructions

### Step 1 - CreateTiles
The main class is WaveFunctionCollapseGenerator.  You must call CreateTiles first to load the tiles into the object.  There are several overloads which offer either code-defined configuration or json-defined configuration.

#### json-defined configuration

Create a folder under Content and place your tiles in there.  Create a file called TileAtributes.json also in the directory.

The TileAttributes.json file defines the properties and rules of each tile.  Here is an example file:

```
{
  "Tiles": {
    "Floor": {
      "Symmetry": "X",
      "Weight": 20,
      "Adapters": "AAA,AAA,AAA,AAA",
      "InitialisationRule": "([X] == 0 || [X] == [MaxX] || [Y] == 0 || [Y] == [MaxY])"
    },
    "Corner": {
      "Symmetry": "^",
      "Weight": 1,
      "Adapters": "AAA,AAA,ABA,ABA"
    },
    "Wall": {
      "Symmetry": "I",
      "Weight": 20,
      "Adapters": "ABA,AAA,ABA,AAA"
    }
  }
}
```

Tiles: defines a list of tiles.  The key is the name of the tile filename without extension or path, the value is a collection of properties.

Symmetry: There are currently three symmetry types,  X, I and ^.  X is a fully symmetrical tile, I is a 2-way symmetric tile (i.e. the tile maps out to two distinct tiles, one normal and one rotated 90 degrees) ^ is a 4-way symmetric tile (i.e. the tile maps out to four distinct tiles, one normal and three rotated 90, 180 and 270 degrees).

Weight: The weight is the chance of the tile being chosen from the list of valid tiles.

Adapters: The adapters define the allowed connection to other tiles.  It can be any string whatsoever, however the order of the string is important. The ordering is clockwise starting from the north side of the tile. So for example a tile defined as "AB,AB,AB,AB" cannot join on itself.  It must be defined as "AB,AB,BA,BA" - a visualisation of the ordering is below:

```
   AB->
 ^  --   A
 A |  |  B
 B  --  \/
   AB
   <--
```

InitialisationRule: An expression that is evaluated to determine if the tile can be placed at a given location (refer to the NCalc GitHub project).  The variables [X] and [Y] are the location of the tile being considered, [MaxX] and [MaxY] are the maximum X and Y values of the map.  The expression must return a boolean.  Initialisation rules are analysed across all tiles first.  The tiles involved have their entropy reduced, making the wave collapse algorithm run them first.  The interescting set of tiles are used as possible choices for a particular location.

#### code-defined configuration
Code defined configuration mirrors the json-defined configuration - you pass in a dictionary of tile names to texture 2D objects and pass in a TileAttributes object which contains the dictionary and properties list for each tile as described in the json configuration section. 

### Step 2 - Reset
Next, call Reset, passing in a WaveFunctionCollapseGeneratorOptions object.  This contains properties for the wave function collapse algorithm behaviour.

int MapWidth: required.  Width of map to create. 
int MapHeight: required.  Height of map to create.
int FallbackAttempts: defaults to 5.  Total amount of retries to perform if a collapse fails at any stage.  This is a total number, not local to any particular failure point, as are the other Fallback-properties below.
int FallbackRadius: defaults to 1.  If a tile cannot be placed in a spot due to there being no valid tile that matches the existing neighbours then any assigned tiles in a radius of FallbackRadius are cleared (and entropy is set to ensure all these tiles are collapsed next).
int FallbackRadiusIncrement: defaults to 1.  After a failure, the FallbackRadius is incremented by this figure, meaning a bigger area is cleared each time.

### Step 3 - NextStep

Keep on calling NextStep until IsComplete or IsFailed returns true.  This will 'collapse' (assign) a tile one by one or perform a rollback attempt if the chosen tile cannot be collapsed.

### Step 4 - Get the result

The result is in the CurrentState array.  Each element has the tile and the chosen tile with the texture in it is in CurrentState[x].TileChoice.Texture.

