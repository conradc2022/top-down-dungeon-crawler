using Dungeon;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Attack;
//Elemental type
public enum DamageType 
{
    None,
    Weapon,
    Heal,
    Bludgeon,
    Pierce,
    Slash,
    Fire,
    Cold,
    Electric,
    Poision,
    Psychic,
    Dark,
    Light,
}
public static class DamageTypes
{
    public static List<DamageType> Physical {get;} = new() {DamageType.Pierce, DamageType.Bludgeon, DamageType.Slash};
    public static List<DamageType> Elemental {get;} = new() {DamageType.Poision, DamageType.Fire, DamageType.Cold, DamageType.Electric, DamageType.Psychic, DamageType.Dark, DamageType.Light};
    public static bool IsPhysical(this DamageType type)
    {
        return Physical.Contains(type);
    }
    public static bool IsElementaal(this DamageType type)
    {
        return Elemental.Contains(type);
    }
}
//Who can it hit
public enum TargetType 
{
    None,
    Self = 1,
    Allies = 2,
    Friendlies = Self + Allies,
    Enemies = 4,
    AllEntities = Self + Allies + Enemies,
    Downed = 8, //For when the target is not alive, if not ticked, downed enemies are not considered
    DownedSelf = Downed + Self,
    DownedAllies = Downed + Allies,
    DownedFriendlies = Downed + Friendlies,
    DownedEnemies = Downed + Enemies,
    DownedAll = Downed + AllEntities,
    Walls = 16,
    Obstactles = Walls + AllEntities
}
//How does it travel
public enum TravelType 
{
    None,
    Cardinal = 1,
    EightDir = 2,
    IgnoreObstacles = 4,
    Projectile = 8,
    ProjectileCardinal= Projectile+Cardinal,
    ProjectileEightDir= Projectile+EightDir,
    Beam = Projectile + IgnoreObstacles,
    BeamCardinal= Beam+Cardinal,
    BeamEightDir= Beam+EightDir,
    Area = 16,
    AreaCardinal=Area+Cardinal,
    AreaEightDir=Area+EightDir,

}

public enum Event
{
    None,
    Burn,
    Poison,
    Paralyze,
    Freeze,
    Blind,
    Confuse,
    Enrage,
    StatChange,
    Revivify,
    Zombify,
    Consume //Corpse-Eater abilities
}

public enum StackRule
{
    /// <summary>
    /// Each event is determined individually in order.
    /// </summary>
    Default,
    /// <summary>
    /// Each event is determined in order, the first event to trigger is used.
    /// </summary>
    FirstPulled,
    /// <summary>
    /// Each event is determined in order, if one event is triggered, the next event is attempted.
    /// </summary>
    ChainPulled,
    /// <summary>
    /// A single random value is generated and all
    /// events with thresholds below the value are triggered.
    /// </summary>
    ThresholdDrive,
    /// <summary>
    /// All effect probabilities are combined, and each take
    /// a relative proportion. One random value is generated to 
    /// determine which event occurs.
    /// </summary>
    CollectiveSplit,
}

public static class AttackChecks
{
    public static RandomNumberGenerator randomNumberGenerator = new();
    public static Dictionary<int, float> AccuracyStage {get;} = new()
    {
        {-6,0.25f},
        {-5,0.28f},
        {-4,0.33f},
        {-3,0.4f},
        {-2,0.5f},
        {-1,0.66f},
        {0,1},
        {1,1.50f},
        {2,2f},
        {3,2.5f},
        {4,3f},
        {5,3.5f},
        {6,4f},

    };
    public static void Randomize()
    {
        randomNumberGenerator.Randomize();
    }
    public static float GetAccuracyStage(int stage)
    {
        if(stage > AccuracyStage.Keys.Max())
        {
            return AccuracyStage[AccuracyStage.Keys.Max()];
        }
        else if(stage < AccuracyStage.Keys.Min())
        {
            return AccuracyStage[AccuracyStage.Keys.Min()];
        }
        else
        {
            return AccuracyStage[stage];
        }
    }
    public static bool AccuracyCheck(float moveAccuracy=1, float attackerAccuracy=1, float defenderEvasion=0, int attackerAccuracyStage =0, int defenderEvasionStage =0, int modifierStage =0)
    {
        float accuracy = moveAccuracy * ((attackerAccuracy - defenderEvasion)/attackerAccuracy)* GetAccuracyStage(attackerAccuracyStage + defenderEvasionStage) * GetAccuracyStage(modifierStage);
        float result = (randomNumberGenerator.Randf() + randomNumberGenerator.Randf())/2;
        Debug.WriteLine($"Acc: {accuracy} || Result: {result} || Hit: {accuracy >= result}");
        return accuracy >= result;
    }
    public static bool IsCritical(float criticalChance)
    {
        float result = (randomNumberGenerator.Randf() + randomNumberGenerator.Randf())/2;
        Debug.WriteLine($"Crit: {criticalChance} || Result: {result} || Hit: {criticalChance >= result}");
        return criticalChance >= result;
    }
    public static float DamageDealt(float power, int level, int offenseStat,  int defenseStat, int offenseStage = 0,int defenseStage = 0, float screen=1, float targets=1, float effects=1, float weather=1, float stockpile =1, float critical = 1, float doubleDamage = 1, float assist = 1, float stab = 1, float defenderType1 = 1, float defenderType2 = 1, float min = 0.85f, float max = 1)
    {
        float damageConstant = ((((2*level/5) + 2 * power * (offenseStat*GetAccuracyStage(offenseStage)/defenseStat*GetAccuracyStage(defenseStage)))/50)*screen*targets*effects*weather*stockpile+2)*critical*doubleDamage*assist*stab*defenderType1*defenderType2;
        float roll =  randomNumberGenerator.RandfRange(min, max);
        Debug.WriteLine($"DamageConstant: {damageConstant} || Roll: {roll}");
        return damageConstant * roll;
    }

    public static bool IsInRange(Vector2 tileLocation, AttackResource attackResource)
    {
        //Is the tile in range for the first phase of the attack resource
        if(attackResource == null || attackResource.Stages.Count <= 0)
        {
            Debug.WriteLine($"This attack: {(attackResource == null ? "UNKNOWN ATTACK"  :attackResource.Name)} has no stages, nothing will be in range");
            return false;
        }
        else
        {
            bool cardinal = true;
            bool distance = true;
            if((attackResource.Stages[0].travelType & TravelType.Cardinal) == attackResource.Stages[0].travelType)
            {
                cardinal = tileLocation.X == 0 || tileLocation.Y == 0;
                distance = tileLocation.DistanceTo(Vector2.Zero) <= attackResource.Stages[0].distance;
            }
            else if((attackResource.Stages[0].travelType & TravelType.EightDir) == attackResource.Stages[0].travelType)
            {
                distance = tileLocation.DistanceTo(Vector2.Zero) <= attackResource.Stages[0].distance;
            }

            return distance && cardinal;
        }
    }

    public static HashSet<Vector2> GetImpactedTiles(Vector2 globalStartPosition, Vector2 relativeTargetPosition, int tileSize, AttackResource attackResource)
    {
        HashSet<Vector2> currentSet = new();
        Vector2 currentGlobalPosition = globalStartPosition;
        for(int i = 0; i<attackResource.Stages.Count(); i++)
        {
            Debug.WriteLine($"Stage: {i} || {currentGlobalPosition}|| {relativeTargetPosition} || {attackResource.Stages[i].travelType} || {attackResource.Stages[i].distance}");
            HashSet<Vector2> stageTiles = GetImpactedTiles(currentGlobalPosition, relativeTargetPosition, tileSize, attackResource.Stages[i]);
            currentSet = currentSet.Union(stageTiles).ToHashSet();
            Debug.WriteLine($"Impacted Tiles:{currentSet.Count}: {string.Join('|', currentSet)}");
        }
        return currentSet;
    }
    
    public static HashSet<Vector2> GetImpactedTiles(Vector2 relativePosition, Vector2 relativeTargetPosition, int tileSize, AttackStage attackResource)
    {
        HashSet<Vector2> currentSet = new();
        Debug.WriteLine($"Type: {attackResource.travelType} {attackResource.distance}");
        if((attackResource.travelType | TravelType.Projectile) == attackResource.travelType){
            Vector2 currentPoint = relativePosition;
            Vector2 delta = relativePosition.DirectionTo(relativeTargetPosition).Normalized();
            double dx = Math.Abs(delta.X);
            double dy = Math.Abs(delta.Y);
            int sx = Math.Sign(delta.X);
            int sy = Math.Sign(delta.Y);
            double err = dx-dy;
            double e2 = 0;
            while(true)
            {
                Vector2I cell = new Vector2I((int)Math.Floor(currentPoint.X), (int)Math.Floor(currentPoint.Y));
                if(!currentSet.Contains(cell))
                {
                    currentSet.Add(cell);
                }
                if(cell.X == Math.Floor(relativeTargetPosition.X) && cell.Y == Math.Floor(relativeTargetPosition.Y))
                {
                    break;
                }
                e2 = 2*err;
                if(e2 > -dy)
                {
                    err -= dy;
                    currentPoint.X += sx;
                }
                if(e2 < dx)
                {   
                    err +=dx;
                    currentPoint.Y += sy;
                }
            }
        }
        else if(attackResource.travelType == TravelType.AreaCardinal)
        {
            foreach(Vector2 direction in Direction2D.CardinalDirections)
            {
                for(int i = 1; i <= attackResource.distance; i++)
                {
                    currentSet.Add(relativePosition + (direction*i));
                }
            }
        }
        else if(attackResource.travelType == TravelType.AreaEightDir)
        {
            for(int row = - attackResource.distance; row <= attackResource.distance; row ++)
            {
                int rowDifference = (int)Math.Round(row - relativePosition.X);
                double columnRange = Math.Sqrt(attackResource.distance * attackResource.distance - rowDifference * rowDifference);

                for(int column = (int)Math.Ceiling(- columnRange); column <= (int)Math.Floor(columnRange); column++)
                {
                    currentSet.Add(new Vector2(row,column));
                }
            }
        }
        return currentSet;
    }
}