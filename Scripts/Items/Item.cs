using Dungeon;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Item;
public enum ItemType 
{
    None,
    Passive = 1,
    Active = 2,
    Multitrigger = Passive + Active,
    Equipment = 4,
    Armor = Equipment + Passive,
    Weapon = Equipment + Active,
}
public enum ItemTrigger
{
    None = 0,
    Use = 1,
    Throw = 2,
    Attack = 4,
    OnTakeDamage = 8,
    OnDeath = 16,
}

public enum ArmorWeight 
{
    None,
    Light,
    Medium,
    Heavy
}

public enum ArmorPiece 
{
    None,
    Head,
    Chest,
    Offhand, //Prevents use of 2 handed weapons
    Legs,
}
public enum WeaponSize
{
    None,
    OneHand = 1,
    TwoHand = 2,
}
public enum WeaponPiece
{
    Sword,
    Mace,
    Spear,
    Whip,
    Dagger,
    Bow,
    Crossbow,
    Sling
}

//How does it travel
public static class WeaponChecks
{
    public static bool IsRanged(WeaponPiece weaponPiece)
    {
        return weaponPiece == WeaponPiece.Bow || weaponPiece == WeaponPiece.Crossbow || weaponPiece == WeaponPiece.Sling;
    }
    public static bool IsReach(WeaponPiece weaponPiece)
    {
        return weaponPiece == WeaponPiece.Spear || weaponPiece == WeaponPiece.Whip;
    }
    public static bool IsFinesse(WeaponPiece weaponPiece)
    {
        return weaponPiece == WeaponPiece.Whip || weaponPiece == WeaponPiece.Dagger;
    }
}