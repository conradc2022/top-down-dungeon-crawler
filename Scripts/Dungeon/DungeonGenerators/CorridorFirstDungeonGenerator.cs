
using Dungeon.Resources;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
namespace Dungeon;
public partial class CorridorFirstDungeonGenerator : RandomWalkDungeonGenerator
{
    [Export]
    public CorridorDungeonGeneratorResource resource;
    protected CorridorDungeonGeneratorResource defaultResource;
    public override void _Ready()
    {
        defaultResource = resource;
    }
    public override Dungeon RunProceduralGeneration()
    {
        return CorridorFirstGeneration();
    }
    public override void SetTempParameters(DungeonGeneratorResource resource)
    {
        try
        {
            resource = (CorridorDungeonGeneratorResource)resource;
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Failed to apply resource to CorridorFirstGenerator: {ex}");
        }
    }
    public override void ResetDefaultParameters()
    {
        resource = defaultResource;
    }

    private Dungeon CorridorFirstGeneration()
    {
        HashSet<Vector2I> floorTiles = new();
        HashSet<Vector2I> potentialRoomPositions = new();
        List<List<Vector2I>> corridors= CreateCorridors(floorTiles, potentialRoomPositions);

        HashSet<Vector2I> roomPositions = CreateRooms(potentialRoomPositions);
        List<Vector2I> deadEndPositions = FindAllDeadEnds(floorTiles);
        CreateRoomsAtDeadEnds(deadEndPositions, roomPositions);

        floorTiles.UnionWith(roomPositions);

        if(resource.corridorWidth > 1)
        {
            for(int i = 0; i< corridors.Count; i++)
            {
                corridors[i] = IncreaseCorridorSizeByWidth(corridors[i], resource.corridorWidth -1);
                floorTiles.UnionWith(corridors[i]);
            }
        }

        TileMapVisualizer.PaintFloorTiles(floorTiles);
        HashSet<Vector2I> walls = WallGenerator.CreateWalls(floorTiles, TileMapVisualizer);
        return new Dungeon() {Floor = floorTiles, Walls = walls};
    }

    private List<List<Vector2I>> CreateCorridors(HashSet<Vector2I> floorPosition,HashSet<Vector2I> roomPosition)
    {
        Vector2I currentPosition = StartPosition;
        roomPosition.Add(currentPosition);
        List<List<Vector2I>> corridors = new();
        for(int i = 0; i< resource.corridorCount; i++)
        {
           List<Vector2I> path = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, resource.corridorLength);
           int floorCount = floorPosition.Count;
           floorPosition.UnionWith(path);

           currentPosition = path[path.Count -1];
           if(floorPosition.Count > floorCount){
            roomPosition.Add(currentPosition);
            corridors.Add(path);
           }
        }
        return corridors;
    }

    private HashSet<Vector2I> CreateRooms(HashSet<Vector2I> potentialRoomPositions)
    {
        HashSet<Vector2I> roomPositions = new();
        int roomCount = Mathf.RoundToInt(resource.roomPercent * (float)potentialRoomPositions.Count);
        List<Vector2I> roomSeeds = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomCount).ToList();
        foreach(Vector2I room in roomSeeds)
        {
            roomPositions.UnionWith(RunRandomWalk(randomWalkResource, room));
        }
        return roomPositions;
    }

    private List<Vector2I> FindAllDeadEnds(HashSet<Vector2I> floorTiles)
    {
        List<Vector2I> deadEnds = new();
        foreach(Vector2I position in floorTiles)
        {
            int neighborCount = 0;
            foreach(Vector2I direction in Direction2D.CardinalDirections)
            {
                if(floorTiles.Contains(direction + position))
                {
                    neighborCount ++;
                }
            }
            if(neighborCount == 1)
            {
                deadEnds.Add(position);
            }
        }
        return deadEnds;
    }

    private void CreateRoomsAtDeadEnds(IEnumerable<Vector2I> deadEnds, HashSet<Vector2I> roomPositions)
    {
        foreach(Vector2I deadEnd in deadEnds)
        {
            if(!roomPositions.Contains(deadEnd))
            {
               roomPositions.UnionWith(RunRandomWalk(randomWalkResource, deadEnd));
            }
        }
    }

    private List<Vector2I> IncreaseCorridorSizeByWidth(List<Vector2I> corridor, int width = 1)
    {
        List<Vector2I> newCorridor = new();
        Vector2I previewDirection = Vector2I.Zero;
        Debug.WriteLine($"Corridor: {corridor.Count}");
        for(int i = 1; i < corridor.Count; i++)
        {
            Vector2I directionFrom = corridor[i] - corridor[i-1];
            if(previewDirection != Vector2I.Zero && directionFrom != previewDirection)
            {
                for(int x = -1; x < 2; x++)
                {
                    for(int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i-1] + new Vector2I(x,y));
                    }
                }
                previewDirection = directionFrom;
            }
            else
            {
                Vector2I newCorridorTileOffset = Direction2D.Get90DirectionFrom(directionFrom);
                newCorridor.Add(corridor[i-1]);
                Vector2I previousPosition = corridor[i-1];
                for(int j = 0; j<width; j++){
                    Vector2I newPosition = previousPosition+(1+j/2)*(int)(Math.Pow(-1,j))*newCorridorTileOffset;
                    newCorridor.Add(newPosition);
                    previousPosition = corridor[i-1];
                }
            }
        }
        Debug.WriteLine($"NEW Corridor: {newCorridor.Count}");
        return newCorridor;
    }
}
