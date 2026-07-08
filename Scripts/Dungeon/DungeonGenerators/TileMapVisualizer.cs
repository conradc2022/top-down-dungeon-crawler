using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
namespace Dungeon;
public partial class TileMapVisualizer : Node
{
    //Size of the tiles (16,16) is default
    public Vector2I TileMapSize {get; private set;}
    public Vector2 TilePosition {get; private set;}
    [Export]
    protected TileMap FloorTileMap;
    [Export]
    protected TileMap VisibilityTileMap;

    public Dungeon Dungeon {get; set;}
    
    [ExportGroup("Visibility Mappings")]
    [Export]
    protected Vector2I InvisibleTile = Vector2I.Zero;
    [Export]
    protected Vector2I SeenTile = Vector2I.Right;

    [ExportGroup("FloorTileMap Mappings")]
    [Export]
    protected Vector2I FloorTile;
    [Export]
    protected Vector2I WallTile;
    [Export]
    protected Vector2I WallTopTile, WallRightTile, WallBottomTile, WallLeftTile, WallFullTile;
    [Export]
    protected Vector2I WallInnerCornerDownLeft, WallInnerCornerDownRight, 
    WallDiagonalCornerDownLeft, WallDiagonalCornerDownRight,  WallDiagonalCornerUpLeft, WallDiagonalCornerUpRight;
    [Export]
    protected int TileSource;
    [Export]
    protected int FloorLayer;
    [Export]
    protected int WallLayer;

    protected int LayerCount = 2;
    public override void _Ready()
    {
        TilePosition = GetParent<DungeonGenerator>().Position;
        TileMapSize = FloorTileMap.TileSet.TileSize;
    }
    public void PaintFloorTiles(IEnumerable<Vector2I> floorPos)
    {
        PaintTiles(floorPos, FloorTileMap, FloorTile, FloorLayer);
        PaintTiles(floorPos, VisibilityTileMap, InvisibleTile, FloorLayer);
    }
    //If Autotiler fails
    public void PaintTiles(IEnumerable<Vector2I> positions, TileMap tileMap, Vector2I tile, int layer)
    {
        foreach(Vector2I position in positions)
        {
            PaintSingleTile(tileMap, tile,layer, position);
        }
    }
    
    //If autotiler works
    public void PaintTilesV2(IEnumerable<Vector2I> tilePos)
    {
        PaintConnectingTiles( FloorTileMap, new Array<Vector2I>(tilePos), FloorLayer);
    }
    public void PaintSingleTile(TileMap tileMap, Vector2I tile, int tileLayer, Vector2I position)
    {
        tileMap.SetCell(tileLayer, position, 0, tile);
    }
    public void ClearSingleTile(TileMap tileMap, int tileLayer, Vector2I position)
    {
        tileMap.SetCell(tileLayer, position, 0);
    }
    public void PaintConnectingTiles(TileMap tileMap, Array<Vector2I> tile, int tileLayer)
    {
        tileMap.SetCellsTerrainConnect(0, tile, 0, tileLayer, false);
    }
    public void Clear(int layer = -1)
    {
        FloorTileMap.ClearLayer(0);
        if(layer < 0){
            FloorTileMap.Clear();
            VisibilityTileMap.Clear();
        }
        else
        {
            FloorTileMap.ClearLayer(layer);
            VisibilityTileMap.ClearLayer(layer);
        }
    }

    //If autotiler cannot be created
    public void PaintWallTile(Vector2I position, string binValue)
    {
        int typeAsInt = Convert.ToInt32(binValue, 2);
        Vector2I? tile = null;
        if(WallByteTypes.wallTop.Contains(typeAsInt))
        {
            tile = WallTopTile;
        }
        else if(WallByteTypes.wallSideLeft.Contains(typeAsInt))
        {
            tile = WallLeftTile;
        }
        else if(WallByteTypes.wallSideRight.Contains(typeAsInt))
        {
            tile = WallRightTile;
        }
        else if(WallByteTypes.wallBottm.Contains(typeAsInt))
        {
            tile = WallBottomTile;
        }
        else if(WallByteTypes.wallFull.Contains(typeAsInt))
        {
            tile = WallFullTile;
        }
        if(tile.HasValue)
        {
            PaintSingleTile( FloorTileMap,tile.Value, WallLayer, position);
            PaintSingleTile( VisibilityTileMap, InvisibleTile, FloorLayer, position);
        }
    }
    public void PaintCornerWallTile(Vector2I position, string binValue)
    {
        int typeAsInt = Convert.ToInt32(binValue, 2);
        Vector2I? tile = null;
        if(WallByteTypes.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = WallInnerCornerDownLeft;
        }
        else if(WallByteTypes.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = WallInnerCornerDownRight;
        }
        else if(WallByteTypes.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = WallDiagonalCornerDownLeft;
        }
        else if(WallByteTypes.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = WallDiagonalCornerDownRight;
        }
        else if(WallByteTypes.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = WallDiagonalCornerUpLeft;
        }
        else if(WallByteTypes.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = WallDiagonalCornerUpRight;
        }
        else if(WallByteTypes.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = WallFullTile;
        }
        else if(WallByteTypes.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = WallBottomTile;
        }
        if(tile.HasValue)
        {
            PaintSingleTile( FloorTileMap,tile.Value, WallLayer, position);
            PaintSingleTile( VisibilityTileMap, InvisibleTile, FloorLayer, position);
        }
    }
    public void PaintWallTiles(IEnumerable<Vector2I> wallPos, IEnumerable<Vector2I> floorPos)
    {
        //PaintWallTilesV2(wallPos);
        
        foreach(Vector2I position in wallPos)
        {
            PaintSingleTile( FloorTileMap,WallTile, WallLayer, position);
        }
        
    }
    public void PaintWallTilesV2(IEnumerable<Vector2I> wallPos)
    {
        PaintConnectingTiles( FloorTileMap, new Array<Vector2I>(wallPos), WallLayer);
    }
    public Dungeon ApplyPrefab(int id, Vector2I position)
    {
        Debug.WriteLine($"Using Prefab: {id} at {position}");
        Dungeon dungeon = new();
        if(FloorTileMap.TileSet.GetPatternsCount() / LayerCount > id)
        {
            dungeon.Floor = ApplyPattern(id*LayerCount, position, FloorLayer).ToHashSet();
            dungeon.Walls = ApplyPattern(id*LayerCount+1, position-Vector2I.One, WallLayer).ToHashSet();
        }
        PaintTiles(dungeon.Floor.Union(dungeon.Walls), VisibilityTileMap, InvisibleTile, FloorLayer);
        return dungeon;
    }
    public Array<Vector2I> ApplyPattern(int id, Vector2I position, int layer = -1)
    {
        TileMapPattern pattern = FloorTileMap.TileSet.GetPattern(id);
        FloorTileMap.SetPattern(layer, position, pattern);
        return new (pattern.GetUsedCells().Select(tile => tile +position).ToList());
    }

    public void UpdateVisibility(Vector2I povTilePosition, World2D world2D)
    {
        uint lightLayerBit = 1 << 5; // Bit 0 (first layer)
        
        Vector2 povGlobalPosition = TranslateTileToPosition(povTilePosition);
        if(Dungeon == null){return;}
        List<Vector2I> tiles = Dungeon.Floor.Union(Dungeon.Walls).ToList();
        Debug.WriteLine(tiles.Count());
        foreach(Vector2I tile in tiles)
        {
            if(VisibilityTileMap.GetCellAtlasCoords(FloorLayer, tile) == InvisibleTile || VisibilityTileMap.GetCellAtlasCoords(FloorLayer, tile) == SeenTile)
            {
                
                int signX = povTilePosition.X > tile.X ? 1 : -1;
                int signY = povTilePosition.Y > tile.Y ? 1 : -1;
                Vector2 testLocation = TranslateTileToPosition(tile) + new Vector2(signX, signY) * TileMapSize/2;
                var query = PhysicsRayQueryParameters2D.Create(povGlobalPosition,testLocation, lightLayerBit);
        
                Dictionary occlusion = world2D.DirectSpaceState.IntersectRay(query);
                if(occlusion.Count <= 0 || ((Vector2)occlusion["position"] - testLocation).Length() < 1)
                {
                    ClearSingleTile(VisibilityTileMap, FloorLayer, tile);
                }
                else
                {
                    //PaintSingleTile(VisibilityTileMap, SeenTile, FloorLayer, tile);
                }
            }
        }
    }
    public Vector2I TranslatePositionToTile(Vector2 position)
    {
        return (Vector2I)((position + TilePosition - TileMapSize/2)/TileMapSize);
    }
    public Vector2 TranslateTileToPosition(Vector2 position)
    {
        return  position * TileMapSize - TilePosition + TileMapSize/2;
    }
}
