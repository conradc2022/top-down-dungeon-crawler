using Dungeon.Resources;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dungeon;
public partial class RandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [Export]
    protected RandomWalkResource randomWalkResource;
    protected RandomWalkResource defaultResource;
    public override void _Ready()
    {
        defaultResource = randomWalkResource;
    }
    public override void SetTempParameters(DungeonGeneratorResource resource)
    {
        try
        {
            randomWalkResource = (RandomWalkResource)resource;
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Failed to apply resource to RandomWalkGenerator: {ex}");
        }
    }
    public override void ResetDefaultParameters()
    {
        randomWalkResource = defaultResource;
    }
    public override Dungeon RunProceduralGeneration()
    {
        HashSet<Vector2I> floorPositions = RunRandomWalk(randomWalkResource, StartPosition);
        HashSet<Vector2I> walls = new();
        if(TileMapVisualizer != null)
        {
            TileMapVisualizer.PaintFloorTiles(floorPositions);
            WallGenerator.CreateWalls(floorPositions, TileMapVisualizer);
        }
        return new Dungeon(){Floor = floorPositions, Walls = walls};
    }
    protected HashSet<Vector2I> RunRandomWalk(RandomWalkResource resource, Vector2I startPosition)
    {
        Vector2I currentPosition = startPosition;
        HashSet<Vector2I> floorPosition = new();

        for(int i = 0; i<resource.Iterations; i++)
        {
           HashSet<Vector2I> path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, resource.WalkLength, randomNumberGenerator);
           floorPosition.UnionWith(path);

           if(resource.StartRandomly)
           {
            currentPosition = floorPosition.ElementAt((int)(randomNumberGenerator.Randi() % floorPosition.Count));
           }
        }
        return floorPosition;
    }
}
