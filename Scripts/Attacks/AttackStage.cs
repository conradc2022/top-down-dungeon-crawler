using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace Attack;
[GlobalClass]
public partial class AttackStage : Resource
{
    [Export]
    public DamageType damageType;
    [Export]
    public TargetType targets;
    [Export(PropertyHint.Enum, "None,ProjectileCardinal,ProjectileEightDir,Beam,BeamCardinal,BeamEightDir,Area,AreaCardinal,AreaEightDir")]
    public TravelType travelType;
    [Export]
    public DamageStage damageStage;
    [Export]
    public int distance;
    [Export]
    public float accuracy;
    
    public AttackStage(){}
}