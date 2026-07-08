using System;
using System.Diagnostics;
using System.Linq;
using Godot;
namespace Character;

public partial class CharacterStats : Node
{
    
    [Export(PropertyHint.Range,"1,100")]
    public int Level =1;

    [Export]
    //Represents the base (level 1) stats for the species
    public StatsResource SpeciesStats;
    [Export]
    //Represents the base (level 1) stats for the job
    public StatsResource JobStats = new();
    [Export]
    //Represents the stats gained through leveling up - Starts as all 0 at Level 1
    public StatsResource LevelStats = new(); 
    [Export]
    //Represents the average growth per level for the species
    public StatsResource SpeciesGrowth = new();
    [Export]
    //Represents the stats gained through permanent potions - Starts as all 0 at Level 1
    public StatsResource PotionStats = new();
    [Export]
    //Represents maximum stats for the Character in its current state
    public StatsResource MaxStats = new(); 
    [Export]
    //Represents the stats temporarily changed/modified during play
    public StatsResource CurrentStats;  
    [Export]
    //Represents the temporary effects during play
    public Godot.Collections.Array<StatsResource> Modifiers; 

    public void Initialize()
    {
        MaxStats = new(); 
        if(SpeciesStats != null)
        {
            MaxStats += SpeciesStats;
        }
        else
        {
            Debug.WriteLine($"{GetParent().Name}.{Name} SpeciesStats is undefined.");
        }
        
        if(JobStats != null)
        {
            MaxStats += JobStats;
        }
        if(PotionStats != null)
        {
            MaxStats += PotionStats;
        }
        if(LevelStats != null)
        {
            MaxStats += LevelStats;
        }
        CurrentStats = new StatsResource() + MaxStats;
    }
    public void SetMaxHealth(int amount)
    {
        MaxStats.Health = amount;
    }
    public void SetMaxHunger(int amount)
    {
        MaxStats.Hunger = amount;
    }
    public void TakeDamage(double amount)
    {
        
        CurrentStats.Health -= amount;
        CurrentStats.Health = Math.Max(CurrentStats.Health, 0);
        if(CurrentStats.Health <= 0)
        {
            Debug.WriteLine("Health Depleted");
        }
    }
    public void Heal(double amount)
    {
        CurrentStats.Health += amount;
        CurrentStats.Health = Math.Min(CurrentStats.Health, MaxStats.Health);
    }
    public void Starve(int amount)
    {
        
        CurrentStats.Hunger -= amount;
        CurrentStats.Hunger = Math.Max(CurrentStats.Hunger, 0);
        if(CurrentStats.Hunger <= 0)
        {
            Debug.WriteLine("Hunger Depleted");
        }
    }
    public void Eat(int amount)
    {
        CurrentStats.Hunger += amount;
        CurrentStats.Hunger = Math.Min(CurrentStats.Hunger, MaxStats.Hunger);
    }
    public void AddModifier()
    {
        
    }
    public void RemoveModifier()
    {
        
    }

    public Godot.Collections.Dictionary SerializeCharacterStats()
    {
        Godot.Collections.Array modifiers = new();
        modifiers.AddRange(Modifiers.ToList().Select<StatsResource,Godot.Collections.Dictionary>(stats => stats.SerializeStats()));
        Godot.Collections.Dictionary result = new()
        {
            {"level", Level},
            {"max_stats", MaxStats.SerializeStats()},
            {"current_stats", CurrentStats.SerializeStats()},
            {"species_stats", SpeciesStats.SerializeStats()},
            {"species_growth", SpeciesStats.SerializeStats()},
            {"job_stats", JobStats.SerializeStats()},
            {"potion_stats", PotionStats.SerializeStats()},
            {"modifiers", modifiers},
        };
        return result;
    }
    public bool DeserializeCharacterStats(Godot.Collections.Dictionary dictionary)
    {
        
        Debug.WriteLine($"Keys: {dictionary.Keys}");
        {
            Level = int.Parse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))].ToString());
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("level")))
        {
            bool success = int.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))].ToString(), out int value);
            if(success)
            {
                Level = value;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Level: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("max_stats")))
        {
            StatsResource newMax = new();
            bool success = newMax.DeserializeStats((Godot.Collections.Dictionary)dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))]);
            if(success)
            {
                MaxStats = newMax;
            }
            else
            {
                Debug.WriteLine($"Failed to parse MaxStats: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("max_stats"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("current_stats")))
        {
            StatsResource newCurrent = new();
            bool success = newCurrent.DeserializeStats((Godot.Collections.Dictionary)dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))]);
            if(success)
            {
                CurrentStats = newCurrent;
            }
            else
            {
                Debug.WriteLine($"Failed to parse CurrentStats: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("current_stats"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("species_stats")))
        {
            StatsResource newSpecies = new();
            bool success = newSpecies.DeserializeStats((Godot.Collections.Dictionary)dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))]);
            if(success)
            {
                SpeciesStats = newSpecies;
            }
            else
            {
                Debug.WriteLine($"Failed to parse SpeciesStats: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("species_stats"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("job_stats")))
        {
            StatsResource newJob = new();
            bool success = newJob.DeserializeStats((Godot.Collections.Dictionary)dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))]);
            if(success)
            {
                JobStats = newJob;
            }
            else
            {
                Debug.WriteLine($"Failed to parse JobStats: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("job_stats"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("potion_stats")))
        {
            StatsResource newPotion = new();
            bool success = newPotion.DeserializeStats((Godot.Collections.Dictionary)dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))]);
            if(success)
            {
                PotionStats = newPotion;
            }
            else
            {
                Debug.WriteLine($"Failed to parse PotionStats: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("potion_stats"))].ToString()}");
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("species_growth")))
        {
            StatsResource newGrowth = new();
            bool success = newGrowth.DeserializeStats((Godot.Collections.Dictionary)dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))]);
            if(success)
            {
                SpeciesGrowth = newGrowth;
            }
            else
            {
                Debug.WriteLine($"Failed to parse SpeciesGrowth: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("species_growth"))].ToString()}");
            }
        }
        
        return true;
    }
}