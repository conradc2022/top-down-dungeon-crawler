using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;
namespace Character;

public enum Stats
{
    Accuracy = 1 >> 1,
    Awareness = 1,
    CriticalChance = 1 << 1,
    CriticalModifier = 1 << 2,
    Evasion = 1 << 3,
    Health = 1 << 4,
    Hunger = 1 << 5,
    Loyalty = 1 << 6,
    MentalAttack = 1 << 7,
    MentalDefense = 1 << 8,
    PhysicalAttack = 1 << 9,
    PhysicalDefense = 1 << 10,
    Speed = 1 << 11,
    Stealth = 1 << 12
}
public enum SourceType
{
    None = 0,
    Species = 1,
    Job = 2,
    Equipment = 4,
    Potion = 8,
    Temporary = 16,
    Ability = 32,
    Level = 64,
    TemporaryPotion = Potion & Temporary,
    LevelSpecies = Level & Species,
    LevelJob = Level & Job,

}
[GlobalClass]
public partial class StatsResource : Resource
{
    [Export]
    //Source of the Stats
    public string SourceName {get; set;}
    [Export]
    public bool IsStage {get ;set;}
    [Export]
    //Source of the Stats
    public SourceType SourceType {get; set;}
    [Export]
    //Likelihood to land a basic hit
    public int Accuracy {get; set;}
    [Export]
    //Likelihood to notice another creature
    public int Awareness {get; set;}
    [Export]
    //Likelihood to land a critical hit
    public int CriticalChance {get; set;}
    [Export]
    //Modifier for a critical hit
    public float CriticalModifier {get; set;}
    [Export]
    //Likelihood to avoid an incoming attach
    public int Evasion {get; set;}
    [Export]
    //Amount of health - damaged by abilities
    public double Health {get; set;}
    [Export]
    //Amount of stamina - used by abilities/moving
    public int Hunger {get; set;}
    [Export]
    //Likelihood to resist recruitment/desertion
    public int Loyalty {get; set;}
    [Export]
    //Damage modifier for magic attacks
    public int MentalAttack {get; set;}
    [Export]
    //Damage modifier against magic attacks
    public int MentalDefense {get; set;}
    [Export]
    //Damage modifier for physical attacks
    public int PhysicalAttack {get; set;}
    [Export]
    //Damage modifier against physical attacks
    public int PhysicalDefense {get; set;}
    [Export]
    //Determines turn order
    public int Speed {get; set;}
    [Export]
    //Likelihood a character will be unnoticed by others
    public int Stealth {get; set;}
    
    public static StatsResource operator +(StatsResource a, StatsResource b)
    {
        StatsResource result = new();
        result.Awareness = a.Awareness + b.Awareness;
        result.Accuracy = a.Accuracy + b.Accuracy;
        result.CriticalChance = a.CriticalChance + b.CriticalChance;
        result.CriticalModifier = a.CriticalModifier + b.CriticalModifier;
        result.Evasion = a.Evasion + b.Evasion;
        result.Health = a.Health + b.Health;
        result.Hunger = a.Hunger + b.Hunger;
        result.Loyalty = a.Loyalty + b.Loyalty;
        result.MentalAttack = a.MentalAttack + b.MentalAttack;
        result.MentalDefense = a.MentalDefense + b.MentalDefense;
        result.PhysicalAttack = a.PhysicalAttack + b.PhysicalAttack;
        result.PhysicalDefense = a.PhysicalDefense + b.PhysicalDefense;
        result.Speed = a.Speed + b.Speed;
        result.Stealth = a.Stealth + b.Stealth;
        return result;
    }
    public static StatsResource operator *(StatsResource a, StatsResource b){
        StatsResource result = new();
        result.Awareness = a.Awareness * b.Awareness;
        result.Accuracy = a.Accuracy * b.Accuracy;
        result.CriticalChance = a.CriticalChance * b.CriticalChance;
        result.CriticalModifier = a.CriticalModifier * b.CriticalModifier;
        result.Evasion = a.Evasion * b.Evasion;
        result.Health = a.Health * b.Health;
        result.Hunger = a.Hunger * b.Hunger;
        result.Loyalty = a.Loyalty * b.Loyalty;
        result.MentalAttack = a.MentalAttack * b.MentalAttack;
        result.MentalDefense = a.MentalDefense * b.MentalDefense;
        result.PhysicalAttack = a.PhysicalAttack * b.PhysicalAttack;
        result.PhysicalDefense = a.PhysicalDefense * b.PhysicalDefense;
        result.Speed = a.Speed * b.Speed;
        result.Stealth = a.Stealth * b.Stealth;
        return result;
    }
    public static StatsResource operator -(StatsResource a)
    {
        StatsResource result = new();
        result.SourceName = a.SourceName;
        result.SourceType = a.SourceType;
        result.Awareness = - a.Awareness;
        result.Accuracy = - a.Accuracy;
        result.CriticalChance = - a.CriticalChance;
        result.CriticalModifier = - a.CriticalModifier;
        result.Evasion = - a.Evasion;
        result.Health = - a.Health;
        result.Hunger = - a.Hunger;
        result.Loyalty = - a.Loyalty;
        result.MentalAttack = - a.MentalAttack;
        result.MentalDefense = - a.MentalDefense;
        result.PhysicalAttack = - a.PhysicalAttack;
        result.PhysicalDefense = - a.PhysicalDefense;
        result.Speed = - a.Speed;
        result.Stealth = - a.Stealth;
        return result;
    }
    public static StatsResource operator -(StatsResource a, StatsResource b)
    {
        return a + -b;
    }
    public static StatsResource operator /(StatsResource a, StatsResource b){
        StatsResource result = new();
        bool allZero = true;
        if(b.Awareness != 0)
        {
            result.Awareness = a.Awareness / b.Awareness;
            allZero = false;
        }
        if(b.Accuracy != 0)
        {
            result.Accuracy = a.Accuracy / b.Accuracy;
            allZero = false;
        }
        if(b.CriticalChance != 0)
        {
            result.CriticalChance = a.CriticalChance / b.CriticalChance;
            allZero = false;
        }
        if(b.CriticalModifier != 0)
        {
            result.CriticalModifier = a.CriticalModifier / b.CriticalModifier;
            allZero = false;
        }
        if(b.Evasion != 0)
        {
            result.Evasion = a.Evasion / b.Evasion;
            allZero = false;
        }
        if(b.Health != 0)
        {
            result.Health = a.Health / b.Health;
            allZero = false;
        }
        if(b.Hunger != 0)
        {
            result.Hunger = a.Hunger / b.Hunger;
            allZero = false;
        }
        if(b.Loyalty != 0)
        {
            result.Loyalty = a.Loyalty / b.Loyalty;
            allZero = false;
        }
        if(b.MentalAttack != 0)
        {
            result.MentalAttack = a.MentalAttack / b.MentalAttack;
            allZero = false;
        }
        if(b.MentalDefense != 0)
        {
            result.MentalDefense = a.MentalDefense / b.MentalDefense;
            allZero = false;
        }
        if(b.PhysicalAttack != 0)
        {
            result.PhysicalAttack = a.PhysicalAttack / b.PhysicalAttack;
            allZero = false;
        }
        if(b.PhysicalDefense != 0)
        {
            result.PhysicalDefense = a.PhysicalDefense / b.PhysicalDefense;
            allZero = false;
        }
        if(b.Speed != 0)
        {
            result.Speed = a.Speed / b.Speed;
            allZero = false;
        }
        if(b.Stealth != 0)
        {
            result.Stealth = a.Stealth / b.Stealth;
            allZero = false;
        }

        if(allZero)
        {
            throw new DivideByZeroException();
        }
        return result;
    }

    public Godot.Collections.Dictionary SerializeStats()
    {
        Dictionary result = new()
        {
            {"stage", IsStage},
            {"name", SourceName},
            {"type", SourceType.ToString()},
            {"awareness", Awareness},
            {"accuracy", Accuracy},
            {"critical_chance", CriticalChance},
            {"critical_modifier", CriticalModifier},
            {"evasion", Evasion},
            {"health", Health},
            {"hunger", Hunger},
            {"loyalty", Loyalty},
            {"mental_attack", MentalAttack},
            {"mental_defense", MentalDefense},
            {"physical_attack", PhysicalAttack},
            {"physical_defense", PhysicalDefense},
            {"speed", Speed},
            {"stealth", Stealth},
        };
        return result;
    }
    public bool DeserializeStats(Godot.Collections.Dictionary dictionary)
    {
        Debug.WriteLine($"Keys: {dictionary.Keys}");
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("stage")))
        {
            bool success = bool.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("stage"))].ToString(), out bool value);
            if(success)
            {
                IsStage = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Stage: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("stage"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("awareness")))
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("name")))
        {
            SourceName = dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("name"))].ToString();
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("type")))
        {
            SourceType = SourceType.Parse<SourceType>(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("type"))].ToString());
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("awareness")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("awareness"))].ToString(), out int value);
            if(success)
            {
                Awareness = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Awareness: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("awareness"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("accuracy")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("accuracy"))].ToString(), out int value);
            if(success)
            {
                Accuracy = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Accuracy: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("accuracy"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("critical_chance")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("critical_chance"))].ToString(), out int value);
            if(success)
            {
                CriticalChance = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse CriticalChance: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("critical_chance"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("critical_modifier")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("critical_modifier"))].ToString(), out int value);
            if(success)
            {
                CriticalModifier = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse CriticalModifier: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("critical_modifier"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("evasion")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("evasion"))].ToString(), out int value);
            if(success)
            {
                Evasion = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Evasion: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("evasion"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("health")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("health"))].ToString(), out int value);
            if(success)
            {
                Health = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Health: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("health"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("hunger")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("hunger"))].ToString(), out int value);
            if(success)
            {
                Hunger = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Hunger: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("hunger"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("loyalty")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("loyalty"))].ToString(), out int value);
            if(success)
            {
                Loyalty = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Loyalty: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("loyalty"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("mental_attack")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("mental_attack"))].ToString(), out int value);
            if(success)
            {
                MentalAttack = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse MentalAttack: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("mental_attack"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("mental_defense")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("mental_defense"))].ToString(), out int value);
            if(success)
            {
                MentalDefense = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse MentalDefense: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("mental_defense"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("physical_attack")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("physical_attack"))].ToString(), out int value);
            if(success)
            {
                PhysicalAttack = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse PhysicalAttack: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("physical_attack"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("physical_defense")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("physical_defense"))].ToString(), out int value);
            if(success)
            {
                PhysicalDefense = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse PhysicalDefense: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("physical_defense"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("speed")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("speed"))].ToString(), out int value);
            if(success)
            {
                Speed = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Speed: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("speed"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("stealth")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("stealth"))].ToString(), out int value);
            if(success)
            {
                Stealth = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Stealth: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("stealth"))].ToString()}");
            }
        }
        return true;
    }
}