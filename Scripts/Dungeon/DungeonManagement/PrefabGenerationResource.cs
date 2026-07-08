using Godot;
using System;
using Godot.Collections;
namespace Dungeon;

[GlobalClass]
public partial class PrefabGenerationResource : GenerationResource
{

    [Export]    
    public string Name;
    [Export]
    public string TileSet;
    [Export]
    public int PrefabID;
    [Export]
    public Vector2I offset;
    [Export]
    public Array<EnemyGenerationResource> ValidEnemies;
    [Export]
    public Array<ItemGenerationResource> ValidItems;
}
