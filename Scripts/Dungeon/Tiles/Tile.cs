using Godot;
using System;

public abstract class Tile
{
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnInteract();
}
