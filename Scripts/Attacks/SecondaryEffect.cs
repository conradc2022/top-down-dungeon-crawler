using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace Attack;

[GlobalClass]
public partial class SecondaryEffect: Resource
{
    [Export]
    public Event secondaryEvent {get; set;}
    [Export]
    public StatChange statChange {get; set;}
    [Export(PropertyHint.Range,"0.0f, 1f")]
    public float probability {get; set;}
}