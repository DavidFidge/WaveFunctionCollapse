# WaveFunctionCollapse
A C# Wave Function Collapse library for MonoGame plus a tool with samples.  This is designed for one of my games, thus it takes a dependency on DavidFidge/FrigidRogue (plus some of its dependencies like GoRogue, SadRogue.Primitives, MonoGame.Extended etc).

This implementation is a tiled implementation and uses reducing entropy values to determine the next tile to place.

## Instructions

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

InitialisationRule: An expression that is evaluated to determine if the tile can be placed at a given location (refer to the NCalc GitHub project).  The variables [X] and [Y] are the location of the tile being considered, [MaxX] and [MaxY] are the maximum X and Y values of the map.  The expression must return a boolean.  Being an Initialisation Rule, it also has a special property where ~~~~