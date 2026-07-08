using Godot;
using System;

public abstract class StatusEffect
{
    public abstract void OnStart();
    public abstract void OnEnd();
    public abstract void OnTurnStart();
    public abstract void OnTurnEnd();
}
