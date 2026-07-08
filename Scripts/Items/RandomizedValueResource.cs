using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using Attack;

namespace Item;
public partial class RandomizedValueResource : Resource
{
    [Export]
    public int ItemID {get; set;}
    [Export]
    public string ValueName {get; set;}
    [Export]
    public double MaxValue {get; set;}
    [Export]
    public double MinValue {get; set;}
    [Export(PropertyHint.Range,"0f,1f")]
    public double Distribution {get; set;} = 0.5;
    [Export]
    public double ActualValue {get; set;}
    public RandomizedValueResource(){}

}
