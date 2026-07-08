using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace Attack;
[GlobalClass]
public partial class DamageStage: Resource
{
    //Assuming standard stats what are the min and max
    //Typically Damage = ((OFF. STAT CALC)*(RandRange(DRMIN, DRMAX))/(DEF. STAT CALC)) + FIXED
    
    [Export]
    public float Power {get; set;}
    
    [Export]
    public float FixedDamage {get; set;}
    [Export]
    public Array<SecondaryEffect> secondaryEffect {get; set;}
    [Export]
    public StackRule secondaryEffectStacking {get; set;} = StackRule.Default;
    
    public DamageStage(){}
}
