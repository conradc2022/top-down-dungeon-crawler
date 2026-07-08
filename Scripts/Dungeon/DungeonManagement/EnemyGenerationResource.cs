using Godot;
using System;
using System.Collections.Generic;
namespace Dungeon;

[GlobalClass]
public partial class EnemyGenerationResource : GenerationResource
{
    [Export]
    public string Name;
    [Export(PropertyHint.Range,"1,100")]
    public int MinLevel = 1;
    [Export(PropertyHint.Range,"1,100")]
    public int MaxLevel = 100;
    [Export(PropertyHint.Range,"0.0f,1.0f")]
    public float AveragePercentage = 0.5f;
    [Export]
    public string Species;
    [Export]
    public Godot.Collections.Dictionary<string, float> ClassOptions;
}
