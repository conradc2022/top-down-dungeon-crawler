using Godot;
using Godot.Collections;
namespace UI.Dialogue;
[GlobalClass]
public partial class DialoguePortraitResource : Resource
{
    [Export]
    //Name of the character
    public string Name {get; set;}
    [Export]
    public Dictionary Expressions {get; set;}
}