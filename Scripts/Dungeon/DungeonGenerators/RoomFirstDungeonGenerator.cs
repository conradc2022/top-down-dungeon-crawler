using Dungeon.Resources;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dungeon;

public partial class RoomFirstDungeonGenerator : RandomWalkDungeonGenerator
{
    [Export]
    public RoomFirstDungeonGeneratorResource resource;
    protected RoomFirstDungeonGeneratorResource defaultResource;
    public override void _Ready()
    {
        defaultResource = resource;
    }
    public override void SetTempParameters(DungeonGeneratorResource resource)
    {
        try
        {
            resource = (RoomFirstDungeonGeneratorResource)resource;
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Failed to apply resource to RoomFirstGenerator: {ex}");
        }
    }
    public override void ResetDefaultParameters()
    {
        resource = defaultResource;
    }
    public override Dungeon RunProceduralGeneration()
    {
        return CreateRooms();
    }

    private Dungeon CreateRooms()
    {
        List<Bounds2I> rooms = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new Bounds2I(){upperLeft = StartPosition, lowerRight = StartPosition + new Vector2I( resource.dungeonWidth,  resource.dungeonHeight)},  resource.minRoomWidth,  resource.minRoomHeight, randomNumberGenerator);
        HashSet<Vector2I> floor = new();
        if( resource.randomWalkRoom)
        {
            floor = CreateRoomsWithRandomWalk(rooms);
        }
        else
        {
        
            floor = CreateSimpleRooms(rooms);
        }
        List<Vector2I> roomCenters = new();
        foreach(Bounds2I room in rooms)
        {
            roomCenters.Add(room.Center);
        }
        HashSet<Vector2I> halls = ConnectRooms(roomCenters);
        floor.UnionWith(halls);
        TileMapVisualizer.PaintFloorTiles(floor);
        HashSet<Vector2I> walls = WallGenerator.CreateWalls(floor, TileMapVisualizer);
        return new Dungeon(){Floor = floor, Walls = walls};
    }

    private HashSet<Vector2I> CreateSimpleRooms(List<Bounds2I> rooms)
    {
        HashSet<Vector2I> floor = new();
        foreach(Bounds2I room in rooms)
        {
            for(int x =  resource.offset; x < room.Size.X -  resource.offset; x ++)
            {
                for(int y =  resource.offset; y < room.Size.Y -  resource.offset; y ++)
                {
                    floor.Add(room.Min + new Vector2I(x, y));
                }
                
            }
        }
        return floor;
    }

    private HashSet<Vector2I> ConnectRooms(List<Vector2I> roomCenters)
    {
        HashSet<Vector2I> halls= new();
        Vector2I currentRoom = roomCenters[randomNumberGenerator.RandiRange(0,roomCenters.Count -1)];
        roomCenters.Remove(currentRoom);
        while(roomCenters.Count > 0)
        {
            Vector2I closest = FindClosestPointTo(currentRoom, roomCenters);
            roomCenters.Remove(closest);
            halls.UnionWith(CreateHall(currentRoom, closest));
            currentRoom = closest;
        }
        return halls;
    }

    private Vector2I FindClosestPointTo(Vector2I target, IEnumerable<Vector2I> candidates)
    {
        Vector2I closest = Vector2I.Zero;
        int distance = int.MaxValue;
        foreach(Vector2I position in candidates)
        {
            int currDistance = (target - position).LengthSquared();
            if(distance > currDistance)
            {
                distance = currDistance;
                closest = position;
            }
        }

        return closest;
    }

    private HashSet<Vector2I> CreateHall(Vector2I start, Vector2I end)
    {
        HashSet<Vector2I> hall = new();
        Vector2I position = start;
        hall.Add(position);
        while(position.Y != end.Y)
        {
            if(end.Y > position.Y)
            {
                position += Vector2I.Down;
            }
            else
            {
                position += Vector2I.Up;
            }
            hall.Add(position);
        }
        while(position.X != end.X)
        {
            if(end.X > position.X)
            {
                position += Vector2I.Right;
            }
            else
            {
                position += Vector2I.Left;
            }
            hall.Add(position);
        }
        return hall;
    }

    private HashSet<Vector2I> CreateRoomsWithRandomWalk(List<Bounds2I> rooms)
    {
        HashSet<Vector2I> floor = new();
        for(int i = 0; i< rooms.Count; i++)
        {
            Bounds2I roomBounds = rooms[i];
            HashSet<Vector2I> room = RunRandomWalk(randomWalkResource, roomBounds.Center);
            foreach(Vector2I tile in room)
            {
                if(tile.X >= (roomBounds.Min.X +  resource.offset) && tile.X <= (roomBounds.Max.X -  resource.offset) && tile.Y >= (roomBounds.Min.Y +  resource.offset) && tile.Y <= (roomBounds.Max.Y-  resource.offset))
                {
                    floor.Add(tile);
                }
            }
        }
        return floor;
    }
}
