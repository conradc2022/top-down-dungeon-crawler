using Dungeon.Resources;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Dungeon;

public abstract partial class AbstractDungeonGenerator : Node
{
    
    [Export]
    protected Vector2I StartPosition = Vector2I.Zero;
    [Export]
    public TileMapVisualizer TileMapVisualizer;
    protected RandomNumberGenerator randomNumberGenerator = new();

    public abstract Dungeon RunProceduralGeneration();

    public Dungeon GenerateDungeon()
    {
        if(TileMapVisualizer != null)
        {
            TileMapVisualizer.Clear();
        }
        Dungeon dungeon = RunProceduralGeneration();
        TileMapVisualizer.Dungeon = dungeon;
        List<Vector2I> tiles = dungeon.Floor.Union(dungeon.Walls).ToList();
        Debug.WriteLine(tiles.Count());
        return dungeon;
    }
    public void SetRandomNumberGenerator(RandomNumberGenerator rng)
    {
        randomNumberGenerator = rng;
    }

    public abstract void ResetDefaultParameters();
    public abstract void SetTempParameters(DungeonGeneratorResource resource);
}
