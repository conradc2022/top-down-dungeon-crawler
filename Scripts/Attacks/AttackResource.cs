using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace Attack;
public partial class AttackResource : Resource
{
    [Export]
    public int AttackID {get; set;}
    [Export]
    public string Name {get; set;}
    [Export]
    public  Array<AttackStage> Stages {get; set;}
    [ExportGroup("Energy")]
    [Export]
    public int Cooldown {get; set;}
    [Export]
    public int HungerCost {get; set;}

    [Export]
    public string Description {get; set;}
    public AttackResource(){}

}
