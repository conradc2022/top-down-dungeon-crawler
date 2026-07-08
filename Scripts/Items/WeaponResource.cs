using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using Attack;
using Character;
namespace Item;


[GlobalClass]
public partial class WeaponResource : ItemResource
{
    [ExportGroup("StatMods")]
    [Export]
    public  RandomizeValueStatsResource StatModifiersMin {get; set;}
    [ExportGroup("Basic")]
    [Export]
    public WeaponPiece WeaponPiece {get; set;}
    [Export]
    public WeaponSize WeaponSize {get; set;}
    [Export]
    public RandomizedValueResource PhysicalAttack {get; set;}
    [Export]
    public RandomizedValueResource MentalAttack{get; set;}
    
    [ExportGroup("Strikes")]
    [Export]
    public RandomizedValueResource BludgeonStrike {get; set;}
    
    [Export]
    public RandomizedValueResource PierceStrike {get; set;}
    
    [Export]
    public RandomizedValueResource SlashStrike {get; set;}
    
    [Export]
    public RandomizedValueResource FireStrike {get; set;}
    [Export]
    public RandomizedValueResource ColdStrike {get; set;}
    
    [Export]
    public RandomizedValueResource ElectricStrike {get; set;}
    
    [Export]
    public RandomizedValueResource PoisonStrike {get; set;}
    
    [Export]
    public RandomizedValueResource PsychicStrike {get; set;}
    
    [Export]
    public RandomizedValueResource LightStrike {get; set;}
    
    [Export]
    public RandomizedValueResource DarkStrike {get; set;}

    public WeaponResource(){}

}

