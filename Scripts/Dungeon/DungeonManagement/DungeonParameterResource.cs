using Godot;
using System;
using Godot.Collections;
using System.Linq;
namespace Dungeon;

[GlobalClass]
public partial class DungeonParameterResource : Resource
{
    [Export]
    public string Name;
    public int FloorCount {get {return FloorSets != null ? FloorSets.Count() : 0;}}
    [Export]
    public Dictionary<string, TileSet> TileSets;
    [Export]
    public Array<FloorParameterResource> FloorSets;
}
