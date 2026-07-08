using Godot;
using System;
using System.Collections.Generic;
namespace Dungeon;

[GlobalClass]
public partial class ItemGenerationResource : GenerationResource
{
    [Export]
    public string ItemType;
    [Export]
    public string Name;
    [Export]
    public int MinRarity = 1;
    [Export]
    public int MaxRarity = 100;
    [Export]
    public float AveragePercentage = 0.5f;
}
