using Godot;
using System;
using System.Diagnostics;
namespace UI.HUD;
public partial class Healthbar : ProgressBar
{
    public Timer Timer;
    [Export]
    public Color FillColor = new Color(1,0,0,1);
    public bool Percentage = false;
    public ProgressBar HurtBar;
    private double health;
    public double Health {get {return health;} set {SetHealth(value);}}
    public Label Label;
    [Export]
    public bool LabelVisible = false;
    public override void _Ready()
    {
        SelfModulate = FillColor;
        ShowPercentage = Percentage;
        HurtBar = GetNode<ProgressBar>("DamageBar");
        Timer = GetNode<Timer>("Timer");
        Label = GetNode<Label>("Label");
        if(!LabelVisible)
        {
            Label.Visible = false;
        }
        else
        {
            Label.Visible = true;
        }
    }
    public void SetLabel(double initialHealth, double currentHealth)
    {
        Label.Text = $"{currentHealth.ToString()} / {initialHealth.ToString()}";
    }
    public void InitHealth(double health)
    {
        this.health = health;
        MaxValue = health;
        Value = health;
        HurtBar.MaxValue = health;
        HurtBar.Value = health;
        SetLabel(MaxValue, Value);
    }

    public void SetHealth(double health)
    {
        double prevHealth = Health;
        this.health = Mathf.Min(health, MaxValue);
        Value = health;
        SetLabel(MaxValue, Value);
        if(this.health <= 0)
        {
            Visible = false;
            return;
        }
        if(this.health < prevHealth)
        {
            Timer.Start();
        }
        else
        {
            HurtBar.Value = this.health;
        }
    }

    public void _on_timer_timeout()
    {
        HurtBar.Value = Health;
    }
}
