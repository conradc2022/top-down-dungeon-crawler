using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace Attack;

[GlobalClass]
public partial class StatChange: Resource
{
    [Export]
    public Event secondaryEvent {get; set;}
    
    [Export(PropertyHint.Range,"0, 10f")]
    public int stages {get; set;}
}