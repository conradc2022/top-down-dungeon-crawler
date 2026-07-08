using Godot;
using System;

public partial class CharacterOverview : Control
{
    [Export]
    public Texture2D Icon;
    [Export]
    public string CharacterName;
    [Export]
    public int Level;
    [Export]
    public int CurrentHealth;
    [Export]
    public int MaxHealth;
    [Export]
    public int CurrentHunger;
    [Export]
    public int MaxHunger;
    [Export]
    public string Status;

    private TextureRect iconRect;
    private Label characterNameLabel;
    private Label levelLabel;
    private Label currentHealthLabel;
    private Label maxHealthLabel;
    private Label currentHungerLabel;
    private Label maxHungerLabel;
    private Label statusLabel;

    public override void _Ready()
    {
        iconRect = GetNode<TextureRect>("Icon");
        characterNameLabel = GetNode<Label>("Name");
        levelLabel = GetNode<Label>("Level/LevelText");
        currentHealthLabel = GetNode<Label>("Health/HPCurrent");
        maxHealthLabel = GetNode<Label>("Health/HPTotal");
        currentHungerLabel = GetNode<Label>("Hunger/HungerCurrent");
        maxHungerLabel = GetNode<Label>("Hunger/HungerTotal");
        statusLabel = GetNode<Label>("Status/StatusText");
        SetIcon(Icon);
        SetName(CharacterName);
        SetLevel(Level);
        SetHealth(CurrentHealth, MaxHealth);
        SetHunger(CurrentHunger, MaxHunger);
        SetStatus(Status);
        
    }
    public void SetIcon(Texture2D icon)
    {
        if(icon != null)
        {
            iconRect.Texture = icon;
        }
    }
    public void SetName(string name)
    {
        characterNameLabel.Text = name;
    }
    public void SetLevel(int level)
    {
        if(level > 0)
        {
            levelLabel.Text = level.ToString();
        }
    }
    public void SetHealth(int? current = null, int? max = null)
    {
        if(current != null && current >= 0)
        {
            currentHealthLabel.Text = current.ToString();
        }
        if(max != null && max > 0)
        {
            maxHealthLabel.Text = max.ToString();
        }
    }
    public void SetHunger(int? current = null, int? max = null)
    {
        if(current != null && current >= 0)
        {
            currentHungerLabel.Text = current.ToString();
        }
        if(max != null && max > 0)
        {
            maxHungerLabel.Text = max.ToString();
        }
    }
    
    public void SetStatus(string name)
    {
        statusLabel.Text = name;
    }
}
