using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;
namespace Character;

[GlobalClass]
public partial class RandomizeValueStatsResource : Resource
{
    [Export]
    //Likelihood to land a basic hit
    public RandomizeValueStatsResource Accuracy {get; set;}
    [Export]
    //Likelihood to notice another creature
    public RandomizeValueStatsResource Awareness {get; set;}
    [Export]
    //Likelihood to land a critical hit
    public RandomizeValueStatsResource CriticalChance {get; set;}
    [Export]
    //Modifier for a critical hit
    public RandomizeValueStatsResource CriticalModifier {get; set;}
    [Export]
    //Likelihood to avoid an incoming attach
    public RandomizeValueStatsResource Evasion {get; set;}
    [Export]
    //Amount of health - damaged by abilities
    public RandomizeValueStatsResource Health {get; set;}
    [Export]
    //Amount of stamina - used by abilities/moving
    public RandomizeValueStatsResource Hunger {get; set;}
    [Export]
    //Likelihood to resist recruitment/desertion
    public RandomizeValueStatsResource Loyalty {get; set;}
    [Export]
    //Damage modifier for magic attacks
    public RandomizeValueStatsResource MentalAttack {get; set;}
    [Export]
    //Damage modifier against magic attacks
    public RandomizeValueStatsResource MentalDefense {get; set;}
    [Export]
    //Damage modifier for physical attacks
    public RandomizeValueStatsResource PhysicalAttack {get; set;}
    [Export]
    //Damage modifier against physical attacks
    public RandomizeValueStatsResource PhysicalDefense {get; set;}
    [Export]
    //Determines turn order
    public RandomizeValueStatsResource Speed {get; set;}
    [Export]
    //Likelihood a character will be unnoticed by others
    public RandomizeValueStatsResource Stealth {get; set;}
}