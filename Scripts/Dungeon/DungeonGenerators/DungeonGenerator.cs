using Dungeon.Resources;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dungeon;
public class Dungeon
{
    public HashSet<Vector2I> Floor {get; set;}
    public HashSet<Vector2I> Walls {get; set;}
}

public enum GenerationMethod
{
    RandomWalk,
    CorridorFirst,
    RoomFirst,
}
public partial class DungeonGenerator : Node2D
{
    [ExportGroup("Parameters")]
    [Export]
    public GenerationMethod generationMethod = GenerationMethod.RandomWalk;
    [Export]
    public DungeonParameterResource dungeonParameters;
    public RandomNumberGenerator randomNumberGenerator = new();
    RandomWalkDungeonGenerator randomWalkDungeonGenerator;
    CorridorFirstDungeonGenerator corridorFirstDungeonGenerator;
    RoomFirstDungeonGenerator roomFirstDungeonGenerator;
    [ExportGroup("RNG-State")]
    [Export]
    public string Seed;
    [Export]
    public ulong StateBefore;
    [Export]
    public ulong StateAfter;
    public override void _Ready()
    {
        randomNumberGenerator.Randomize();
        randomWalkDungeonGenerator = GetNode<Node>("RandomWalkGenerator") as RandomWalkDungeonGenerator;
        randomWalkDungeonGenerator.SetRandomNumberGenerator(randomNumberGenerator);
        corridorFirstDungeonGenerator = GetNode<Node>("CorridorFirstGenerator") as CorridorFirstDungeonGenerator;
        corridorFirstDungeonGenerator.SetRandomNumberGenerator(randomNumberGenerator);
        roomFirstDungeonGenerator = GetNode<Node>("RoomFirstGenerator") as RoomFirstDungeonGenerator;
        roomFirstDungeonGenerator.SetRandomNumberGenerator(randomNumberGenerator);
    }
    public Dungeon Generate()
    {
        Dungeon dungeon = new();
        Seed = randomNumberGenerator.Seed.ToString();
        StateBefore = randomNumberGenerator.State;
        switch(generationMethod)
        {
            case GenerationMethod.RandomWalk:
                dungeon = randomWalkDungeonGenerator.GenerateDungeon();
                break;
            case GenerationMethod.CorridorFirst:
                dungeon = corridorFirstDungeonGenerator.GenerateDungeon();
                break;
            case GenerationMethod.RoomFirst:
                dungeon = roomFirstDungeonGenerator.GenerateDungeon();
                break;
        }
        StateAfter = randomNumberGenerator.State;
        return dungeon;
    }

    public AbstractDungeonGenerator GetDungeonGenerator()
    {
        switch(generationMethod)
        {
            case GenerationMethod.CorridorFirst:
                return corridorFirstDungeonGenerator;
            case GenerationMethod.RoomFirst:
                return roomFirstDungeonGenerator;
            case GenerationMethod.RandomWalk:
            default:
                return randomWalkDungeonGenerator;
        }
    }

    public Dungeon ApplyPrefab(int id, Vector2I position)
    {
        Seed = $"Prefab_{id}";
        StateBefore = (ulong)position.X;
        StateAfter = (ulong)position.Y;
        AbstractDungeonGenerator dungeonGenerator = GetDungeonGenerator();
        return dungeonGenerator.TileMapVisualizer.ApplyPrefab(id, position);
        
    }
    public Dungeon IsolatePrefab(int id, Vector2I position)
    {
        AbstractDungeonGenerator dungeonGenerator = GetDungeonGenerator();
        dungeonGenerator.TileMapVisualizer.Clear();
        Dungeon dungeon = ApplyPrefab(id, position);
        dungeonGenerator.TileMapVisualizer.Dungeon = dungeon;
        List<Vector2I> tiles = dungeon.Floor.Union(dungeon.Walls).ToList();
        Debug.WriteLine(tiles.Count());
        return dungeon;
    }

    public Dungeon AdvanceToFloor(int floor)
    {
        if(dungeonParameters.FloorCount < floor || floor < 0)
        {
            Debug.WriteLine($"Error: dungeonParameters only supports {dungeonParameters.FloorCount} floors, while Floor is {floor}");
            return null;
        }
        else
        {
            
            Dungeon dungeon = new();
            int floorIndex = floor-1;
            FloorParameterResource floorResource = dungeonParameters.FloorSets[floorIndex];
                
            Seed = randomNumberGenerator.Seed.ToString();
            StateBefore = randomNumberGenerator.State;
            if(floorResource.DungeonGeneratorResource != null)
            {
                GenerationMethod tempGeneration;
                switch(floorResource.DungeonGeneratorResource.GetType().Name.ToLower())
                {
                    case "randomwalkresource":
                        tempGeneration = GenerationMethod.RandomWalk;
                        break;
                    case "roomfirstdungeongeneratorresource":
                        tempGeneration = GenerationMethod.RoomFirst;
                        break;
                    case "corridordungeongeneratorresource":
                        tempGeneration = GenerationMethod.CorridorFirst;
                    break;
                    default:
                        Debug.WriteLine($"Failed to Identify the GenerationMethod for {floorResource.DungeonGeneratorResource.GetType().Name}");
                        return null;
                }
                //Set Temporary Parameters   
                switch(tempGeneration)
                {
                    case GenerationMethod.RandomWalk:
                        randomWalkDungeonGenerator.SetTempParameters(floorResource.DungeonGeneratorResource);
                        dungeon = randomWalkDungeonGenerator.GenerateDungeon();
                        randomWalkDungeonGenerator.ResetDefaultParameters();
                        break;
                    case GenerationMethod.CorridorFirst:
                        corridorFirstDungeonGenerator.SetTempParameters(floorResource.DungeonGeneratorResource);
                        dungeon = corridorFirstDungeonGenerator.GenerateDungeon();
                        corridorFirstDungeonGenerator.ResetDefaultParameters();
                        break;
                    case GenerationMethod.RoomFirst:
                        roomFirstDungeonGenerator.SetTempParameters(floorResource.DungeonGeneratorResource);
                        dungeon = roomFirstDungeonGenerator.GenerateDungeon();
                        roomFirstDungeonGenerator.ResetDefaultParameters();

                        break;
                }
                //Reset to defaults
            }
            else
            {                
                if(floorResource.ValidPrefabs.Count() <= 0)
                {
                    Debug.WriteLine($"No valid prefabs to select from");
                    return null;
                }
                int randPrefab = randomNumberGenerator.RandiRange(0, floorResource.ValidPrefabs.Count()-1);
                return IsolatePrefab(floorResource.ValidPrefabs[randPrefab].PrefabID,floorResource.ValidPrefabs[randPrefab].offset);
            }
            
            StateAfter = randomNumberGenerator.State;
            return dungeon;
        }
    }

    public Godot.Collections.Dictionary SerializeGenerator()
    {
        Godot.Collections.Dictionary result = new()
        {
            {"global_position", new Godot.Collections.Dictionary()
                {
                    {"x",GlobalPosition.X},
                    {"y",GlobalPosition.Y},
                }
            },
            {"seed", Seed},
            {"beforeState", StateBefore.ToString()},
            {"afterState", StateAfter.ToString()}
        };
        return result;
    }
    public bool DeserializeGenerator(Godot.Collections.Dictionary dictionary)
    {
        Debug.WriteLine($"Keys: {dictionary.Keys}");
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("global_position")))
        {
            bool result = DeserializeVectorUtils.DeserializeVector2((Godot.Collections.Dictionary)dictionary.Keys.First(key => key.ToString().ToLower().Equals("global_position")), out Vector2 position);
            if(result)
            {
                GlobalPosition = position;
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("seed")))
        {
            Seed = dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("seed"))].ToString();
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("beforestate")))
        {
            bool success = ulong.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("beforestate"))].ToString(), out ulong value);
            if(success)
            {
                StateBefore = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse StateBefore: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("beforestate"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("afterstate")))
        {
            bool success = ulong.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("afterstate"))].ToString(), out ulong value);
            if(success)
            {
                StateAfter = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse StateAfter: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("afterstate"))].ToString()}");
            }
        }
        return true;
    }
}
