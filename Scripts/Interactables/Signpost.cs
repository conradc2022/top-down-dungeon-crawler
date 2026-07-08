using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DialogueManagerRuntime;
using System.Runtime.CompilerServices;
namespace Interactables;

public partial class Signpost : AbstractInteractable
{
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
        if(Sprites.Count > 0 && (PreferredSprite < 0 || PreferredSprite >= Sprites.Count))
        {
            PreferredSprite = (int)(GD.Randi() % Sprites.Count);
        }
        if(Sprites.Count > 0)
        {
            Sprite.Texture = Sprites[PreferredSprite];
        }
        DialogueManager.DialogueEnded += (resource) => SetOpen(false, resource);
    }
    public void SetOpen(bool open, Resource resource = null)
    {
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
            DialogueManager.ShowDialogueBalloon(dialogResource, dialogStart);
            SetOpen(true);
            Debug.WriteLine($"Agent: {agent.Name} : {(Character.Character)agent != null}");
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
        return false;
    }
}
