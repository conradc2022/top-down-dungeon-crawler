using Godot;
using System;
using System.Collections.Generic;
namespace Dungeon;

[GlobalClass]
public partial class GenerationResource : Resource
{
    [Export(PropertyHint.Range, "0.0f,1.0f")]
    public float Frequency = 1;
    [Export]
    public Vector2I GridPosition;
    [Export(PropertyHint.Range,"0,100")]
    public int MinCount = 0;
    [Export(PropertyHint.Range,"0,100")]
    public int MaxCount = 100;
    [Export]
    public bool IgnorePosition = true;
    [Export]
    public bool IgnoreMax = true;
    [Export]
    public bool IgnoreMin = true;
}
