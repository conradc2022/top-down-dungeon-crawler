using Dungeon;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public static class WallGenerator
{
    public static HashSet<Vector2I> CreateWalls(HashSet<Vector2I> floorPositions, TileMapVisualizer tileMapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.CardinalDirections);
        var cornerPositions = FindWallsInDirections(floorPositions, Direction2D.DiagonalDirections);

        CreateBasicWalls(tileMapVisualizer, basicWallPositions, floorPositions);
        CreateCornerWalls(tileMapVisualizer, cornerPositions, floorPositions);
        basicWallPositions.UnionWith(cornerPositions);
        return basicWallPositions;
    }

    public static void CreateBasicWalls(TileMapVisualizer tileMapVisualizer, HashSet<Vector2I> wallPos, HashSet<Vector2I> floorPos)
    {
        foreach(Vector2I position in wallPos){
            string neighborsBinValue = "";
            foreach(var direction in Direction2D.CardinalDirections )
            {
                var neighbor = position + direction;
                if(floorPos.Contains(neighbor))
                {
                    neighborsBinValue += "1";
                }
                else
                {
                    neighborsBinValue += "0";
                }
            }
           tileMapVisualizer.PaintWallTile(position, neighborsBinValue);
        }
    }
    public static void CreateCornerWalls(TileMapVisualizer tileMapVisualizer, HashSet<Vector2I> wallPos, HashSet<Vector2I> floorPos)
    {
        foreach(Vector2I position in wallPos){
            string neighborsBinValue = "";
            foreach(var direction in Direction2D.EightDirectionsList )
            {
                var neighbor = position + direction;
                if(floorPos.Contains(neighbor))
                {
                    neighborsBinValue += "1";
                }
                else
                {
                    neighborsBinValue += "0";
                }
            }
           tileMapVisualizer.PaintCornerWallTile(position, neighborsBinValue);
        }
    }

    public static HashSet<Vector2I> FindWallsInDirections(HashSet<Vector2I>floorPositions, List<Vector2I> directions)
    {
        HashSet<Vector2I> wallPositions = new();
        foreach(Vector2I position in floorPositions)
        {
            foreach(Vector2I direction in directions)
            {
                var neighborPos = position + direction;
                if(!floorPositions.Contains(neighborPos))
                {
                    wallPositions.Add(neighborPos);
                }
            }
        }
        return wallPositions;
    }
}
public static class WallByteTypes
{
    public static HashSet<int> wallTop = new HashSet<int>
    {
        0b1111,
        0b0110,
        0b0011,
        0b0010,
        0b1010,
        0b1100,
        0b1110,
        0b1011,
        0b0111
    };

    public static HashSet<int> wallSideLeft = new HashSet<int>
    {
        0b0100
    };

    public static HashSet<int> wallSideRight = new HashSet<int>
    {
        0b0001
    };

    public static HashSet<int> wallBottm = new HashSet<int>
    {
        0b1000
    };

    public static HashSet<int> wallInnerCornerDownLeft = new HashSet<int>
    {
        0b11110001,
        0b11100000,
        0b11110000,
        0b11100001,
        0b10100000,
        0b01010001,
        0b11010001,
        0b01100001,
        0b11010000,
        0b01110001,
        0b00010001,
        0b10110001,
        0b10100001,
        0b10010000,
        0b00110001,
        0b10110000,
        0b00100001,
        0b10010001
    };

    public static HashSet<int> wallInnerCornerDownRight = new HashSet<int>
    {
        0b11000111,
        0b11000011,
        0b10000011,
        0b10000111,
        0b10000010,
        0b01000101,
        0b11000101,
        0b01000011,
        0b10000101,
        0b01000111,
        0b01000100,
        0b11000110,
        0b11000010,
        0b10000100,
        0b01000110,
        0b10000110,
        0b11000100,
        0b01000010

    };

    public static HashSet<int> wallDiagonalCornerDownLeft = new HashSet<int>
    {
        0b01000000
    };

    public static HashSet<int> wallDiagonalCornerDownRight = new HashSet<int>
    {
        0b00000001
    };

    public static HashSet<int> wallDiagonalCornerUpLeft = new HashSet<int>
    {
        0b00010000,
        0b01010000,
    };

    public static HashSet<int> wallDiagonalCornerUpRight = new HashSet<int>
    {
        0b00000100,
        0b00000101
    };

    public static HashSet<int> wallFull = new HashSet<int>
    {
        0b1101,
        0b0101,
        0b1101,
        0b1001

    };

    public static HashSet<int> wallFullEightDirections = new HashSet<int>
    {
        0b00010100,
        0b11100100,
        0b10010011,
        0b01110100,
        0b00010111,
        0b00010110,
        0b00110100,
        0b00010101,
        0b01010100,
        0b00010010,
        0b00100100,
        0b00010011,
        0b01100100,
        0b10010111,
        0b11110100,
        0b10010110,
        0b10110100,
        0b11100101,
        0b11010011,
        0b11110101,
        0b11010111,
        0b11010111,
        0b11110101,
        0b01110101,
        0b01010111,
        0b01100101,
        0b01010011,
        0b01010010,
        0b00100101,
        0b00110101,
        0b01010110,
        0b11010101,
        0b11010100,
        0b10010101

    };

    public static HashSet<int> wallBottmEightDirections = new HashSet<int>
    {
        0b01000001
    };

}
