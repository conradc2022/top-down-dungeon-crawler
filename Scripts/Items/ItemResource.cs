using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using Attack;

namespace Item;
public partial class ItemResource : Resource
{
    [Export]
    public int ItemID {get; set;}
    [Export]
    public string Name {get; set;}
    [Export]
    public ItemType ItemType {get; set;}
    [Export]
    public int ItemTriggers {get; set;}
    [Export]
    public string Description {get; set;}
    [Export]
    public bool Stackable {get; set;}
    public ItemResource(){}

}
