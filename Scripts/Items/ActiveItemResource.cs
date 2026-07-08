using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using Attack;

namespace Item;

[GlobalClass]
//Functions Like an attack
public partial class ActiveItemResource : ItemResource
{
    [Export]
    public  Array<AttackStage> Stages {get; set;}
    [ExportGroup("Energy")]
    [Export]
    public int Cooldown {get; set;}
    [Export]
    public int HungerCost {get; set;}
    public ActiveItemResource(){}

}
