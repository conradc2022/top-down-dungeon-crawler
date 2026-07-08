using Godot;
using System;
using Godot.Collections;
using Dungeon.Resources;
namespace Dungeon;

[GlobalClass]
public partial class FloorParameterResource : GenerationResource
{
    [Export]
    public string Name;
    [Export]
    public Array<string> TileSets;
    [Export]
    public Array<EnemyGenerationResource> ValidEnemies;
    [Export]
    public Array<PrefabGenerationResource> ValidPrefabs;
    [Export]
    public int MaxPrefabCount;
    [Export]
    public int EnemySpawnRate;
    [Export]
    public int MaxItemCount;
    [Export]
    public Array<ItemGenerationResource> ValidItems;
    [Export]
    public DungeonGeneratorResource DungeonGeneratorResource;

    public override string ToString()
    {
        return $"{Name}: {(DungeonGeneratorResource != null ? $"{DungeonGeneratorResource.GetType().Name}  {DungeonGeneratorResource.ToString()}" : "PREFAB")}";
    }
}
