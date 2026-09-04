using Godot;
using Godot.Collections;
using System.Diagnostics;
using DialogueManagerRuntime;
using System.Threading.Tasks;
using Dungeon;
using System.ComponentModel;
namespace Interactables;

public partial class Stairs : AbstractInteractable
{
    [Signal]
    public delegate void StairsConfirmedEventHandler();
    [Export]
    Array<Texture2D> Sprites;
    [Export]
    public int PreferredSprite = -1;
    Sprite2D Sprite;
    [Export]
    Resource dialogResource;
    [Export]
    string dialogStart;
    bool dialogOpen = false;
    Character.Character interactingAgent;
    public override void _Ready()
    {
        base._Ready();
        Sprite = GetNode<Sprite2D>("Sprite2D");
        //Choose random sign if -1 or out of range, otherwise use the provided value
        if(Sprites != null && Sprites.Count > 0 && (PreferredSprite < 0 || PreferredSprite >= Sprites.Count))
        {
            PreferredSprite = (int)(GD.Randi() % Sprites.Count);
        }
        if(Sprites != null && Sprites.Count > 0)
        {
            Sprite.Texture = Sprites[PreferredSprite];
        }
        DialogueManager.DialogueEnded += (resource) => SetOpen(false, resource);
    }
    public void SetOpen(bool open, Resource resource = null)
    {
        Debug.WriteLine($"SetDialog Open: Stairs {open}");
        if(resource == null || resource == dialogResource)
        {
            dialogOpen = open;
            if(interactingAgent != null)
            {
                interactingAgent.SetInteracting(false);
                interactingAgent = null;
            }
        }
    }
    public override bool Interact(CharacterBody2D agent)
    {
        if(dialogResource != null && dialogStart != null && !dialogOpen)
        {
            DialogueManager.ShowDialogueBalloon(dialogResource, dialogStart,new(){this});
            SetOpen(true);
            Character.Character character = (Character.Character)agent;
            if(character != null)
            {
                character.SetInteracting(true);
                interactingAgent = character;
            }
        }
        return true;
    }
    public override bool CanWalkThrough(CharacterBody2D agent)
    {
        return true;
    }

    // Called when the node is added to the scene tree
    public override void _EnterTree()
    {
        base._EnterTree();

        Node parent = GetParent();
        if (parent != null)
        {
            parent = parent.GetParent();
            InitDungeon ParentNode = parent as InitDungeon;
            
            if (parent.HasMethod("_on_stairs_confirmed"))
            {
                var error = Connect(nameof(StairsConfirmed), new(ParentNode, nameof(ParentNode._on_stairs_confirmed)));
                if (error == Error.Ok)
                {
                    GD.Print("Successfully connected to StairsConfirmed signal");
                }
                else
                {
                    GD.PrintErr("Failed to connect to StairsConfirmed signal");
                }
            }
        }
    }
    // Called when the node is removed from the scene tree
    public override void _ExitTree()
    {
        // Disconnect all signals connected to this instance
        /*
        foreach (Godot.Collections.Dictionary target in GetSignalConnectionList(SignalName.StairsConfirmed))
        {
            Disconnect(SignalName.StairsConfirmed, (Godot.Callable)target["callable"]);
        }
        */
        base._ExitTree();

    }
    public async Task<Variant> ConfirmStairs()
    {
        EmitSignal(SignalName.StairsConfirmed);
        SetOpen(false);
        return true;
    }
}
