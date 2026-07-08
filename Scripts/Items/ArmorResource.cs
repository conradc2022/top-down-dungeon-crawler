using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using Attack;
using Character;

namespace Item;

public partial class ArmorResource : ItemResource
{
    [ExportGroup("StatMods")]
    [Export]
    public  RandomizeValueStatsResource StatModifiersMin {get; set;}
    [ExportGroup("Basic")]
    [Export]
    public ArmorPiece ArmorPiece {get; set;}
    [Export]
    public ArmorWeight ArmorWeight {get; set;}
    [Export]
    public RandomizedValueResource PhysicalArmor {get; set;}
    [Export]
    public RandomizedValueResource MentalArmor {get; set;}
    
    [ExportGroup("Resistances")]
    [Export]
    public RandomizedValueResource HealResist {get; set;}
    
    [Export]
    public RandomizedValueResource BludgeonResist {get; set;}
    
    [Export]
    public RandomizedValueResource PierceResist {get; set;}
    
    [Export]
    public RandomizedValueResource SlashResist {get; set;}
    
    [Export]
    public RandomizedValueResource FireResist {get; set;}
    [Export]
    public RandomizedValueResource ColdResist {get; set;}
    
    [Export]
    public RandomizedValueResource ElectricResist {get; set;}
    
    [Export]
    public RandomizedValueResource PoisonResist {get; set;}
    
    [Export]
    public RandomizedValueResource PsychicResist {get; set;}
    
    [Export]
    public RandomizedValueResource LightResist {get; set;}
    
    [Export]
    public RandomizedValueResource DarkResist {get; set;}
    [Export]
    public RandomizedValueResource WeaponResist {get; set;}

    public ArmorResource(){}

}
