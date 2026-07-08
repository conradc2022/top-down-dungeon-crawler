using Godot;
using System;
namespace Interactables;

public abstract partial class AbstractInteractable : StaticBody2D
{
    [Export]
    public Vector2 location = Vector2.Zero;
    public override void _Ready()
    {
        
        this.GlobalPosition = location;
        base._Ready();
    }
    public abstract bool Interact(CharacterBody2D agent);
    public abstract bool CanWalkThrough(CharacterBody2D agent);
}
