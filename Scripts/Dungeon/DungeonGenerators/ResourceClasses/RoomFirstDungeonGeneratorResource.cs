using Godot;
using System;

namespace Dungeon.Resources;
[GlobalClass]
public partial class RoomFirstDungeonGeneratorResource : DungeonGeneratorResource
{
    [Export]
    public int minRoomWidth = 4, minRoomHeight = 4;
    [Export]
    public int dungeonWidth = 20, dungeonHeight = 20;
    [Export(PropertyHint.Range, "0,10")]
    public int offset = 1;
    [Export]
    public bool randomWalkRoom = false;
    
    public override string ToString()
    {
        return $"minRoom: [{minRoomWidth}, {minRoomHeight}], dungeon: [{dungeonWidth}, {dungeonHeight}], off: {offset}, randWalkRoom: {randomWalkRoom}";
    }
}
