using Godot;
using System;

namespace Dungeon.Resources;
[GlobalClass]
public partial class RandomWalkResource : DungeonGeneratorResource
{
    [Export]
    public int Iterations = 10;
    [Export]
    public int WalkLength = 10;
    [Export]
    public bool StartRandomly = true;
    public RandomWalkResource(){}
    public RandomWalkResource(int iter, int walk, bool randStart)
    {
        Iterations = iter;
        WalkLength = walk;
        StartRandomly = randStart;
    }
    
    public override string ToString()
    {
        return $"Iter: {Iterations}, Walk: {WalkLength}, StartRand: {StartRandomly}";
    }
}
