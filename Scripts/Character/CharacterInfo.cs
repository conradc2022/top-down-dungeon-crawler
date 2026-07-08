using Godot;
namespace Character;

public enum Allegence
{
    Default,
    Neutral,
}
public partial class CharacterInfo : Node
{
    
    [Export]
    public string CharacterName;
    [Export]
    public string Team;
    [Export]
    public Allegence Allegence;
}