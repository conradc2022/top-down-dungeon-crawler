using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
namespace Dungeon;
public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2I> SimpleRandomWalk(Vector2I startPos, int walkLength, RandomNumberGenerator rng = null)
    {
        HashSet<Vector2I> path = new()
        {
            startPos
        };
        Vector2I prevPos = startPos;
        for(int i = 0; i<walkLength; i++)
        {
            Vector2I newPos = prevPos + (rng == null ? Direction2D.GetRandomCardinalDirection() : Direction2D.GetRandomCardinalDirection(rng));
            path.Add(newPos);
            prevPos=newPos;
        }
        return path;
    }
    
    public static List<Vector2I> RandomWalkCorridor(Vector2I startPos, int walkLength,  RandomNumberGenerator rng = null)
    {
        List<Vector2I> path = new();

        Vector2I direction = rng == null ? Direction2D.GetRandomCardinalDirection() : Direction2D.GetRandomCardinalDirection(rng);
        path.Add(startPos);
        Vector2I prevPos = startPos;
        for(int i = 0; i<walkLength; i++)
        {
            Vector2I newPos = prevPos + direction;
            path.Add(newPos);
            prevPos=newPos;
        }
        return path;
    }

    public static List<Bounds2I> BinarySpacePartitioning(Bounds2I spaceToSplit, int minWidth, int minHeight, RandomNumberGenerator rng = null)
    {
        Queue<Bounds2I> roomsQueue = new();
        List<Bounds2I>  roomsList = new();
        roomsQueue.Enqueue(spaceToSplit);
        while(roomsQueue.Count > 0)
        {
            
            Bounds2I room = roomsQueue.Dequeue();
            if(room.Size.Y >= minHeight && room.Size.X >= minWidth)
            {
                if(rng == null){
                    if(GD.Randf() < 0.5)
                    {
                        if(room.Size.Y >= minHeight * 2)
                        {
                            SplitHorizontally(minHeight, roomsQueue, room);
                        }
                        else if(room.Size.X >= minWidth * 2)
                        {

                            SplitVertically(minWidth, roomsQueue, room);
                        }
                        else
                        {
                            roomsList.Add(room);
                        }
                    }
                    else
                    {
                        if(room.Size.X >= minWidth * 2)
                        {
                            SplitVertically(minWidth, roomsQueue, room);
                        }
                        else if(room.Size.Y >= minHeight * 2)
                        {

                            SplitHorizontally(minHeight, roomsQueue, room);
                        }
                        else
                        {
                            roomsList.Add(room);
                        }

                    }
                }
                else{
                    if(rng.Randf() < 0.5)
                    {
                        if(room.Size.Y >= minHeight * 2)
                        {
                            SplitHorizontally(minHeight, roomsQueue, room, rng);
                        }
                        else if(room.Size.X >= minWidth * 2)
                        {

                            SplitVertically(minWidth, roomsQueue, room, rng);
                        }
                        else
                        {
                            roomsList.Add(room);
                        }
                    }
                    else
                    {
                        if(room.Size.X >= minWidth * 2)
                        {
                            SplitVertically(minWidth, roomsQueue, room, rng);
                        }
                        else if(room.Size.Y >= minHeight * 2)
                        {

                            SplitHorizontally(minHeight, roomsQueue, room, rng);
                        }
                        else
                        {
                            roomsList.Add(room);
                        }
                    }
                }
            }

        }
        return roomsList;
    }

    public static void SplitHorizontally(int minHeight, Queue<Bounds2I> roomsQueue, Bounds2I room, RandomNumberGenerator rng = null)
    {
        int ySplit = rng == null ? GD.RandRange(1, room.Size.Y) : rng.RandiRange(1, room.Size.Y) ;
        Bounds2I room1 = new(){upperLeft = room.Min, lowerRight = new Vector2I(room.Max.X,room.Min.Y + ySplit)};
        Bounds2I room2 = new(){upperLeft = new Vector2I(room.Min.X, room.Min.Y + ySplit),lowerRight = room.Max};
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);

    }
    public static void SplitVertically(int minWidth, Queue<Bounds2I> roomsQueue, Bounds2I room, RandomNumberGenerator rng = null)
    {
        int xSplit = rng == null ? GD.RandRange(1, room.Size.X) : rng.RandiRange(1, room.Size.X);
        Bounds2I room1 = new(){upperLeft = room.Min, lowerRight = new Vector2I(room.Min.X + xSplit, room.Max.Y)};
        Bounds2I room2 = new(){upperLeft = new Vector2I(room.Min.X + xSplit, room.Min.Y),lowerRight = room.Max};
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public struct Bounds2I
{
    public Vector2I upperLeft {get; set;}
    public Vector2I lowerRight {get; set;}
    public readonly Vector2I Min  {get => new(Math.Min(lowerRight.X, upperLeft.X), Math.Min(lowerRight.Y,upperLeft.Y));}
    public readonly Vector2I Max  {get => new(Math.Max(lowerRight.X, upperLeft.X), Math.Max(lowerRight.Y,upperLeft.Y));}

    public readonly Vector2I Center {get=> new( (int)Math.Round((Min.X + Max.X )/2.0), (int)Math.Round((Min.Y + Max.Y )/2.0));}
    public readonly Vector2I Size {get => new(lowerRight.X- upperLeft.X, lowerRight.Y - upperLeft.Y);}
    public override string ToString()
    {
        return $"{Min.ToString()} {Max.ToString()}";
    }
}
public static class Direction2D
{
    public static List<Vector2I> CardinalDirections = new()
    {
        Vector2I.Up,
        Vector2I.Left,
        Vector2I.Down,
        Vector2I.Right
    };
    public static List<Vector2I> DiagonalDirections = new()
    {
        new Vector2I(1,1),
        new Vector2I(-1,1),
        new Vector2I(-1,-1),
        new Vector2I(1,-1),
    };

    public static List<Vector2I> EightDirectionsList = new()
    {
        Vector2I.Up,
        new Vector2I(-1,-1),
        Vector2I.Left,
        new Vector2I(-1,1),
        Vector2I.Down,
        new Vector2I(1,1),
        Vector2I.Right,
        new Vector2I(1,-1),
    };

    public static Vector2I GetRandomCardinalDirection()
    {
        return CardinalDirections[(int)(GD.Randi() % CardinalDirections.Count)];
    }
    public static Vector2I GetRandomCardinalDirection(RandomNumberGenerator rng)
    {
        return CardinalDirections[(int)(rng.Randi() % CardinalDirections.Count)];
    }

    public static Vector2I Get90DirectionFrom(Vector2I direction)
    {
        switch(direction){
            case Vector2I v when v.Equals(Vector2I.Up):
                return Vector2I.Left;
            case Vector2I v when v.Equals(Vector2I.Down):
                return Vector2I.Right;
            case Vector2I v when v.Equals(Vector2I.Right):
                return Vector2I.Up;
            case Vector2I v when v.Equals(Vector2I.Left):
                return Vector2I.Down;
        } 
        return Vector2I.Zero;
    }
}