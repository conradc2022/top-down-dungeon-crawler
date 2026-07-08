using Godot;
using System;

namespace Dungeon.Resources;
[GlobalClass]
public partial class CorridorDungeonGeneratorResource : DungeonGeneratorResource
{
    [Export]
    public int corridorLength = 15;
    [Export]
    public int corridorCount = 5;
    [Export (PropertyHint.Range,"0.1,1")]
    public float roomPercent = 0.8f;

    [Export (PropertyHint.Range, "1,10")]
    public int corridorWidth = 1;
    
    public override string ToString()
    {
        return $"CL: {corridorLength}, CC: {corridorCount}, CW: {corridorWidth}, R%: {roomPercent}";
    }
}
