
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using System.Diagnostics;
using Interactables;
using TurnManagement;
using DataManager;
using System.Text.RegularExpressions;
using Attack;
using System.Threading.Tasks;
using DialogueManagerRuntime;
namespace Dungeon;

public partial class InitDungeon : Node2D
{
    public DungeonGenerator dungeonGenerator;
    public TileMapVisualizer tileMapVisualizer;
    public TurnQueue turnQueue;
    public SaveManager saveManager;

    public Node Interactables;
    public AnimationPlayer TransitionAnimationPlayer;
    public Label TransitionDungeonLabel;
    public Label TransitionFloorLabel;
    public Label FloorLevelLabel;
    public int FloorLevel;
    public Character.Character playerCharacter;
    [Export]
    public string DungeonName = "Crimson Tower";

    public override void _Ready()
    {
        dungeonGenerator = GetNode<DungeonGenerator>("DungeonGenerator");
        tileMapVisualizer = dungeonGenerator.GetDungeonGenerator().TileMapVisualizer;
        turnQueue = GetNode<TurnQueue>("TurnQueue");
        Interactables = GetNode<Node>("Interactables");
        saveManager = GetNode<SaveManager>("SaveManager");
        turnQueue.Initialize();
        FloorLevel = 0;
        FloorLevelLabel = GetNode<Label>("CanvasLayer/Level State");
        TransitionAnimationPlayer = GetNode<AnimationPlayer>("CanvasLayer/TransitionPanel/AnimationPlayer");
        TransitionDungeonLabel = GetNode<Label>("CanvasLayer/TransitionPanel/VBoxContainer/DungeonName");
        TransitionFloorLabel = GetNode<Label>("CanvasLayer/TransitionPanel/VBoxContainer/DungeonFloor");
        playerCharacter = GetNode<Character.Character>("TurnQueue/PlayerCharacter");
        if(playerCharacter != null)
        {
            playerCharacter.Connect(nameof(Character.Character.MoveComplete), new(this, nameof(this._on_player_position_change)));
        }
        UpdateFloorLevel(FloorLevel);
        UpdateDungeonName(DungeonName);
    }
    public void _on_button_pressed()
    {
        Dungeon dungeon = dungeonGenerator.Generate();
        if(FloorLevel <= 0)
        {
            MoveAllEntitiesToDungeon(dungeon);
            UpdateFloorLevel(1);
        }
        else
        {
            RandomizeEntityPositions(dungeon);
        }
    }
    public void _on_default_room_pressed()
    {
        Vector2I defaultRoomPosition = new( -5, -5); 
        Dungeon dungeon = dungeonGenerator.IsolatePrefab(0, defaultRoomPosition);
        if(FloorLevel <= 0)
        {
            MoveAllEntitiesToDungeon(dungeon);
            UpdateFloorLevel(1);
        }
        else
        {
            RandomizeEntityPositions(dungeon);
        }
    }
    public void MoveAllEntitiesToDungeon(Dungeon dungeon)
    {
        //Occasionally has characters and interactables land on the same space, very noticable with the small demo room
        List<Character.Character> characters = turnQueue.GetChildren().OfType<Character.Character>().ToList();
        List<AbstractInteractable> interactables = Interactables.GetChildren().OfType<AbstractInteractable>().ToList();
        foreach(AbstractInteractable entity in interactables)
        { 
            if(!dungeon.Floor.Contains(TranslatePositionToTile(entity.GlobalPosition)) || !IsAlignedToGrid(entity.GlobalPosition))
            {
                Vector2I result = FindUnoccupiedSpace(dungeon.Floor, 
                interactables.Select(character => TranslatePositionToTile(character.GlobalPosition)).Union(interactables.Select(character => TranslatePositionToTile(character.GlobalPosition))).ToList());
                //entity.GlobalPosition = TranslateTileToPosition(result);
                entity.SetPosition(TranslateTileToPosition(result));
            }
        }
        foreach(Character.Character entity in characters)
        {
            if(!dungeon.Floor.Contains(TranslatePositionToTile(entity.GlobalPosition)) || !IsAlignedToGrid(entity.GlobalPosition))
            {
                Vector2I result = FindUnoccupiedSpace(dungeon.Floor, 
                characters.Select(character => TranslatePositionToTile(character.GlobalPosition)).Union(interactables.Select(character => TranslatePositionToTile(character.GlobalPosition))).ToList());
                //entity.GlobalPosition = TranslateTileToPosition(result);
                entity.SetPosition(TranslateTileToPosition(result));
            }
        }
        if(playerCharacter != null)
        {
            //tileMapVisualizer.UpdateVisibility(TranslatePositionToTile(playerCharacter.GlobalPosition), GetWorld2D());
        }
    }
    public void RandomizeEntityPositions(Dungeon dungeon)
    {
        List<Character.Character> characters = turnQueue.GetChildren().OfType<Character.Character>().ToList();
        List<AbstractInteractable> interactables = Interactables.GetChildren().OfType<AbstractInteractable>().ToList();
        foreach(AbstractInteractable entity in interactables)
        {
            Vector2I result = FindUnoccupiedSpace(dungeon.Floor, 
                interactables.Select(character => TranslatePositionToTile(character.GlobalPosition)).Union(interactables.Select(character => TranslatePositionToTile(character.GlobalPosition))).ToList());
                //entity.GlobalPosition = TranslateTileToPosition(result);
                entity.SetPosition(TranslateTileToPosition(result));
        }
        foreach(Character.Character entity in characters)
        {
            Vector2I result = FindUnoccupiedSpace(dungeon.Floor,
            characters.Select(character => TranslatePositionToTile(character.GlobalPosition)).Union(interactables.Select(character => TranslatePositionToTile(character.GlobalPosition))).ToList());
            //entity.GlobalPosition = TranslateTileToPosition(result);
            entity.SetPosition(TranslateTileToPosition(result));
        }
        
        if(playerCharacter != null)
        {
            //tileMapVisualizer.UpdateVisibility(TranslatePositionToTile(playerCharacter.GlobalPosition), GetWorld2D());
        }
    }

    public Vector2I FindUnoccupiedSpace(HashSet<Vector2I> validSpaces, List<Vector2I> otherEntities)
    {
        //Other entities is in the GlobalPosition space while validSpaces is in the Grid space
        List<Vector2I> valid = validSpaces.Except(otherEntities).ToList();
        return valid.Count <= 0 ? Vector2I.Zero : valid[(int)(GD.Randi()%valid.Count)];
    }

    public void _on_deal_damage_pressed()
    {
        Character.Character character = playerCharacter;
        character.SetHealth(-10);
    }

    public void _on_list_all_locations_pressed()
    {
        /*
        Debug.WriteLine($"{(!dungeonGenerator.Seed.StartsWith("Prefab_") ? "Seed" : "Default")}: {dungeonGenerator.Seed} "+
        $"|| StateBefore: {dungeonGenerator.StateBefore} || StateAfter: {dungeonGenerator.StateAfter}");*/
        foreach(Node2D node in turnQueue.GetChildren().Union(Interactables.GetChildren()))
        {
            Debug.WriteLine($"Node: {node}: {node.GlobalPosition} {TranslatePositionToTile(node.GlobalPosition)}");
        }
    }
    public Vector2I TranslatePositionToTile(Vector2 position)
    {
        return tileMapVisualizer.TranslatePositionToTile(position);
    }
    public Vector2 TranslateTileToPosition(Vector2 position)
    {
        return tileMapVisualizer.TranslateTileToPosition(position);
    }

    public bool IsAlignedToGrid(Vector2 position)
    {
        Vector2 resultVector = TranslateTileToPosition(TranslatePositionToTile(position));
        return position == resultVector;
    }

    public void _on_strike_pressed()
    {
        
        Character.Character character = playerCharacter;
        AttackResource resource = GD.Load<AttackResource>("res://Resources/Attacks/Basic Attacks/Strike.tres");
        if(!character.targeting){
        character.EnterTargetingMode(resource);
        }
        else
        {
            character.ExitTargetingMode();
        }
    }
    public void _on_flamethrower_pressed()
    {
        
        Character.Character character = playerCharacter;
        AttackResource resource = GD.Load<AttackResource>("res://Resources/Attacks/Fire Attacks/Flamethrower.tres");
        if(!character.targeting){
        character.EnterTargetingMode(resource);
        }
        else
        {
            character.ExitTargetingMode();
        }
    }
    public void _on_save_state_pressed()
    {
        saveManager.WorldState = this;
        saveManager.SaveData($"{saveManager.SaveDirectory}{saveManager.SaveFileName}");
    }
    public void _on_load_state_pressed()
    {
        saveManager.WorldState = this;
        saveManager.LoadData($"{saveManager.SaveDirectory}{saveManager.SaveFileName}");
        Regex regex = new(@"^Prefab_([\d]+)$");
        if(ulong.TryParse(dungeonGenerator.Seed, out ulong seed))
        {
            dungeonGenerator.randomNumberGenerator.Seed = seed;
            dungeonGenerator.randomNumberGenerator.State = dungeonGenerator.StateBefore;
            Dungeon dungeon = dungeonGenerator.Generate();
            MoveAllEntitiesToDungeon(dungeon);
        }
        else if(regex.IsMatch(dungeonGenerator.Seed))
        {
            int capturedPrefab = int.Parse(regex.Match(dungeonGenerator.Seed).Groups[1].Value); //Groups[0] is always the matching string
            Vector2I defaultRoomPosition = new( (int)dungeonGenerator.StateBefore, (int)dungeonGenerator.StateAfter); 
            Dungeon dungeon = dungeonGenerator.IsolatePrefab(capturedPrefab, defaultRoomPosition);
            MoveAllEntitiesToDungeon(dungeon);
        }
    }

    public async void _on_stairs_confirmed()
    {
        Character.Character character = playerCharacter;
        TransitionAnimationPlayer.Play("Fade-In");
        await ToSignal(TransitionAnimationPlayer, "animation_finished");
        
        character.SetInteracting(true);

        AdvanceToFloor(FloorLevel+1);
        TransitionAnimationPlayer.Play("Fade-Out");
        await ToSignal(TransitionAnimationPlayer, "animation_finished");
        character.SetInteracting(false);

    }
    public async void _on_player_position_change()
    {
        Debug.WriteLine($"MoveComplete: {TranslatePositionToTile(playerCharacter.GlobalPosition)}");
        tileMapVisualizer.UpdateVisibility(TranslatePositionToTile(playerCharacter.GlobalPosition), GetWorld2D());
    }
    
    public async Task<Variant> ConfirmStairs()
    {
        if(DialogueManager.GameStates.Any(e => (Stairs)(e) != null))
        {
            Stairs stairs = (Stairs)DialogueManager.GameStates.First(e => (Stairs)(e) != null);
            await stairs.ConfirmStairs();
        }
        return true;
    }

    public void AdvanceToFloor(int newFloor)
    {
        if(newFloor >= dungeonGenerator.dungeonParameters.FloorCount && Interactables.GetChildren().OfType<Stairs>().ToList().Count > 0)
        {
            foreach(Stairs child in Interactables.GetChildren().OfType<Stairs>().ToList())
            {
                Interactables.RemoveChild(child);
            }
        }
        Dungeon dungeon = dungeonGenerator.AdvanceToFloor(newFloor);
        RandomizeEntityPositions(dungeon);
        
        UpdateFloorLevel(newFloor);
    }
    public void UpdateFloorLevel(int newLevel)
    {
        FloorLevel = newLevel;
        FloorLevelLabel.Text = $"FL - {newLevel}";
        TransitionFloorLabel.Text = $"FL - {newLevel}";
    }
    public void UpdateDungeonName(string name)
    {
        TransitionDungeonLabel.Text = name;
    }
}
