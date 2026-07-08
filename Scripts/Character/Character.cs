using Godot;
using UI.HUD;
using Interactables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Attack;
namespace Character;

public enum Controller
{
    None,
    Player,
    Computer
}
public enum MoveSchema
{
    EightDirection,
    Grid
}
public enum PartnerRole
{
    Leader=0, //Either is the player or a leader of their faction
    Defender=1, //Gets between Enemies and the lower defense party members
    Attacker = 2, //Focuses on damaging enemies
    Supporter = 3, //Focuses on buffing allies
    Controller = 4, //Focuses on debuffing enemies
    Retriever = 5, //Focuses on retrieving items/resources
}
public enum TravelRole
{
    Leader = 0,
    Follower = 1,
    SplitUp = 2,
    Patrol = 3,
}

public partial class Character : CharacterBody2D
{
    [Signal]
    public delegate void MoveCompleteEventHandler();

    [Export]
    public CharacterInfo characterInfo;
    [ExportGroup("Movement")]
    [Export]
    public Controller controller = Controller.None;
    public AStar2D aStar2D = new();
    [Export]
    public Vector2 location = Vector2.Zero;
    [Export]
    public MoveSchema moveSchema = MoveSchema.EightDirection;
    public Vector2 facing = Vector2.Zero;

    [Export]
    public float speed = 1;
    [Export]
    public int tileSize = 16;
    public bool moving = false;
    public Vector2 GoalDirection = Vector2.Zero;
    [ExportGroup("Health")]
    [Export]
    public bool ShowDebugHealth = false;
    [Export]
    public Healthbar PrimaryHealth;
    [Export]
    public double DefaultMaxHealth = 100; //Overwrite with the maxHealth from the CharacterStats if possible
    public Healthbar DebugHealth;
    [Export]
    public Label LevelState;

    [ExportGroup("State")]
    //Interacting with any elements that would require the character movement to be disabled
    [Export]
    private bool interacting = false;
    [Export]
    public bool IsAlive = true;
    //Iteracting with the targeting system
    [Export]
    public bool targeting = false;

    public CharacterStats Statistics;
    private AnimationPlayer uiAnimationPlayer;
    private AnimationPlayer animationPlayer;
    private AnimationTree animationTree;
    private AnimationNodeStateMachinePlayback animationState;
    private Dictionary<Vector2I, Node2D> IsColliding = new()
    {
        {Vector2I.Up, null},
        {Vector2I.Down, null},
        {Vector2I.Left, null},
        {Vector2I.Right, null}
    };

    private Node2D CenterCollision;

    private TargetingSystem targetingSystem;
    private Label hitIndicator;

    public override void _Ready()
    {
        this.GlobalPosition = location;
        uiAnimationPlayer = GetNode<AnimationPlayer>("UIAnimationPlayer");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationState = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
        
        Debug.WriteLine($"Hello! I'm {characterInfo.CharacterName} from team {characterInfo.Team}");
        DebugHealth = GetNode<Healthbar>("DebugHealth");
        Statistics = GetNode<CharacterStats>("Statistics");
        Statistics.Initialize();
        Statistics.MaxStats.Health = Statistics.MaxStats.Health > 0 ? Statistics.MaxStats.Health : DefaultMaxHealth;
        
        DebugHealth.InitHealth(Statistics.MaxStats.Health);
        DebugHealth.Visible = ShowDebugHealth;
        if(PrimaryHealth != null)
        {
            PrimaryHealth._Ready();
            PrimaryHealth.InitHealth(Statistics.MaxStats.Health);
        }
        
        if(LevelState != null)
        {
            Debug.WriteLine($"Setting Level to: {Statistics.Level}");
            LevelState.Text = $"LV - {Statistics.Level}";
        }
        targetingSystem = GetNode<TargetingSystem>("TargetingSystem");
        hitIndicator = GetNode<Label>("HitIndicator");
        hitIndicator.Visible = false;
    }
    public void SetPosition(Godot.Vector2 globalPosition)
    {
        GlobalPosition = globalPosition;
        moving = true;
        Tween tween = CreateTween();
        tween.TweenProperty(this,"global_position", globalPosition, 0.01f);
        tween.TweenCallback(new Callable(this, MethodName.MoveGridComplete));
    }

    public void SetHealth(double difference)
    {
        //Statistics.CurrentStats.Health = value;
        if(difference == 0)
        {
            return; //Nothing Happened
        }
        else if(difference > 0)
        {
            Statistics.Heal(difference);
            hitIndicator.Text = difference.ToString();
            Debug.WriteLine($"Yay! I'm at {Statistics.CurrentStats.Health} health");
        }
        else if(difference < 0)
        {
            Statistics.TakeDamage(-difference);
            hitIndicator.Text = difference.ToString();
            Debug.WriteLine($"Ouch! I'm at {Statistics.CurrentStats.Health} health");
            animationState.Travel("hurt");
        }
        if(Statistics.CurrentStats.Health <= 0 && IsAlive)
        {
            Debug.WriteLine($"Blegh!");
            IsAlive = false;
            //animationState.Set("parameters/conditions/alive", IsAlive);
            //animationState.Set("parameters/conditions/dead", !IsAlive);
        }
        uiAnimationPlayer.Play("hit");
        DebugHealth.Health = Statistics.CurrentStats.Health;
        if(PrimaryHealth != null)
        {
            PrimaryHealth.Health = Statistics.CurrentStats.Health;
        }
    }

    public async Task PlayTurn()
    {
        Debug.WriteLine($"{Name} is starting their turn.");
        Debug.WriteLine($"{Name} is ending their turn.");
    }

    public override void _PhysicsProcess(double delta)
    {
        if(IsAlive){
        if(moveSchema == MoveSchema.EightDirection)
        {
            Vector2 inputVector = Vector2I.Zero;
            if(!interacting){
                if(controller == Controller.Player)
                {
                    inputVector = GetPlayerDirectionalInput();
                }
                else if(controller == Controller.Computer)
                {
                    //Get Move from AI module
                }
                if(inputVector != Vector2.Zero)
                {
                    animationTree.Set("parameters/walk/blend_position", inputVector);
                    animationTree.Set("parameters/pause/blend_position", inputVector);
                    animationTree.Set("parameters/hurt/blend_position", inputVector);
                    animationTree.Set("parameters/die/blend_position", inputVector);
                    animationState.Travel("walk");
                }
                else 
                {
                    if(animationState.GetCurrentNode() == "walk"){
                    animationState.Travel("pause");
                    }
                }
            }
            Velocity = inputVector * speed * 10000 * (float)delta;
            MoveAndSlide();
            
        }
        else if(moveSchema == MoveSchema.Grid)
        {
            Vector2 inputVector = Vector2I.Zero;
            Vector2 checkedVector = Vector2I.Zero;
            if(!interacting)
            {
                if(controller == Controller.Player)
                {
                    inputVector = GetPlayerGridInput();
                    if(!targeting){
                        checkedVector = CheckSwapSpots(inputVector);
                        checkedVector = MoveGrid(checkedVector);
                    }
                    else if(inputVector != Vector2.Zero)
                    {
                        inputVector= targetingSystem.MoveCursor(inputVector);
                    }
                }
                else if(controller == Controller.Computer)
                {
                    //Get Move from AI module
                    inputVector = GetComputerGridInput();
                    if(!targeting){
                    checkedVector = CheckSwapSpots(inputVector);
                    checkedVector = MoveGrid(checkedVector);
                    }
                    else if(inputVector != Vector2.Zero)
                    {
                        inputVector=targetingSystem.MoveCursor(inputVector);
                    }
                }
            }
            if(inputVector != Vector2.Zero)
            {
                animationTree.Set("parameters/walk/blend_position", inputVector);
                animationTree.Set("parameters/pause/blend_position", inputVector);
                animationTree.Set("parameters/hurt/blend_position", inputVector);
                animationTree.Set("parameters/die/blend_position", inputVector);
                facing = inputVector;
                
            }
            if(checkedVector != Vector2.Zero)
            {
                animationState.Travel("walk");
            }
            else
            {
                if(animationState.GetCurrentNode() == "walk" && IsAlive){
                    animationState.Travel("pause");
                }
            }

            location = GlobalPosition;
        }
        }
        else
        {
            animationState.Travel("die");
        }
        base._PhysicsProcess(delta);
    }

    private Vector2 GetPlayerDirectionalInput()
    {
        Vector2 input = new Vector2(Input.GetActionStrength("ui_right")-Input.GetActionStrength("ui_left"), 
            Input.GetActionStrength("ui_down")-Input.GetActionStrength("ui_up") ).Normalized();
        
        if(TestMove(Transform, new Vector2(input.X, 0)))
        {
            input.X = 0;
        }
        if(TestMove(Transform, new Vector2(0, input.Y)))
        {
            input.Y = 0;
        }
        return input;
    }
    private Vector2 GetPlayerGridInput()
    {
        Vector2 input = Vector2.Zero;
        if(Input.IsActionPressed("ui_down"))
        {
            input = Vector2.Down;
        }
        if(Input.IsActionPressed("ui_up"))
        {
            input = Vector2.Up;
        }
        if(Input.IsActionPressed("ui_left"))
        {
            input = Vector2.Left;
        }
        if(Input.IsActionPressed("ui_right"))
        {
            input = Vector2.Right;
        }
        return input;
    }
    private Vector2 GetComputerGridInput()
    {
        return GoalDirection;
    }
    public void SetGoalDirection(Vector2 goalDirection)
    {
        GoalDirection = goalDirection;
    }
    public Vector2 CheckSwapSpots(Vector2 direction)
    {
        Vector2I directionI = (Vector2I)direction;
        if(!direction.IsEqualApprox(Vector2.Zero) && !moving)
        {
            if(IsColliding[directionI] != null)
            {
                Character character = IsColliding[directionI] as Character;
                if(character != null)
                {
                    if(!character.CanISwitchSpots(characterInfo.Team))
                    {
                        Debug.WriteLine($"Character: Denied");
                        direction = Vector2.Zero;
                    }
                    else
                    {
                        Debug.WriteLine($"Character: Allowed");
                        character.SetGoalDirection(-direction);
                    }
                }
                AbstractInteractable interactable = IsColliding[directionI] as AbstractInteractable;
                
                if(interactable != null)
                {
                    if(!interactable.CanWalkThrough(this))
                    {
                        Debug.WriteLine($"Interactable: Denied");
                        direction = Vector2.Zero;
                        interactable.Interact(this);
                    }
                }
            }
        }
        return direction;
    }
    public bool CanISwitchSpots(string team)
    {
        return team.Equals(characterInfo.Team);
    }
    private Vector2 MoveGrid(Vector2 direction)
    {
        if(direction == Vector2.Zero || direction.Length() != 1)
        {
            return direction;
        }
        if(!moving)
        {
            KinematicCollision2D kc = new();
            if(!TestMove(Transform, direction * tileSize, kc)){
                moving = true;
                Tween tween = CreateTween();
                tween.TweenProperty(this,"position",Transform.Origin + direction * tileSize, 1.0/speed);
                tween.TweenCallback(new Callable(this, MethodName.MoveGridComplete));
            }
            else
            {
                direction = Vector2.Zero;
            }
        }
        return direction;
    }
    private void MoveGridComplete()
    {
        moving = false;
        location = GlobalPosition;
        if(GoalDirection != Vector2.Zero)
        {
            GoalDirection = Vector2.Zero;
        }
        if(CenterCollision != null)
        {
            AbstractInteractable interactable = CenterCollision as AbstractInteractable;            
            if(interactable != null)
            {
                Debug.WriteLine($"Interactable: Entered");
                interactable.Interact(this);
            }
        }
        if(controller == Controller.Player && moveSchema == MoveSchema.Grid)
        {
            EmitSignal(nameof(MoveComplete));
        }
    }

    public void SetInteracting(bool interact)
    {
        interacting = interact;
    }
    //Can use areas for Dialog options
    //Direction matters, but the more powerful direction will be the resulting tile
    private void _on_collider_body_entered(Node2D node)
    {
        Vector2 direction = (node.GlobalPosition - GlobalPosition).Normalized();
        //Assume its on the vertical
        if(Math.Abs(direction.X) > Math.Abs(direction.Y))
        {
            IsColliding[direction.X > 0 ? Vector2I.Right : Vector2I.Left] = node;
        }
        else
        {
            IsColliding[direction.Y > 0 ? Vector2I.Down : Vector2I.Up] = node;
        }
    }
    private void _on_collider_body_exited(Node2D node)
    {
        //Vector2 direction = (node.GlobalPosition - GlobalPosition).Normalized();
        
        List<Vector2I> list = IsColliding.Where( entity => node.Equals(entity.Value)).Select(kvp => kvp.Key).ToList();
        foreach(Vector2I vector in list)
        {
            IsColliding[vector] = null;
        }
    }

    private void _on_center_collider_body_entered(Node2D node)
    {
        CenterCollision = node;
    }
    private void _on_center_collider_body_exited(Node2D node)
    {
        if(CenterCollision == node)
        {
            CenterCollision = null;
        }
    }

    public Godot.Collections.Dictionary SerializeCharacterData()
    {
        return new Godot.Collections.Dictionary() {
            { "global_position", new Godot.Collections.Dictionary ()
                {
                    {"x", GlobalPosition.X},
                    {"y", GlobalPosition.Y},
                }
            },
            { "facing", new Godot.Collections.Dictionary ()
                {
                    {"x", facing.X},
                    {"y", facing.Y},
                }
            },
            { "stats", Statistics.SerializeCharacterStats()
            },

        };
    }
    public void DeserializeCharacterData(Godot.Collections.Dictionary dictionary)
    {
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("global_position")))
        {
            bool result = DeserializeVectorUtils.DeserializeVector2((Godot.Collections.Dictionary)dictionary.Keys.First(key => key.ToString().ToLower().Equals("global_position")), out Vector2 position);
            if(result)
            {
                GlobalPosition = position;
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("facing")))
        {
            bool result = DeserializeVectorUtils.DeserializeVector2((Godot.Collections.Dictionary)dictionary.Keys.First(key => key.ToString().ToLower().Equals("facing")), out Vector2 facingResult);
            if(result)
            {
                facing = facingResult;
                animationTree.Set("parameters/walk/blend_position", facing);
                animationTree.Set("parameters/pause/blend_position", facing);
                animationTree.Set("parameters/hurt/blend_position", facing);
                animationTree.Set("parameters/die/blend_position", facing);
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("stats")))
        {
            CharacterStats newStats = new();
            bool success = newStats.DeserializeCharacterStats((Godot.Collections.Dictionary)dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("level"))]);
            if(success)
            {
                Statistics = newStats;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Stats: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("stats"))].ToString()}");
            }
        }
    }
    public void ToggleTargeting(bool? state)
    {
        if(state == null)
        {
            targeting = !targeting;
        }
        else
        {
            targeting = state.Value;
        }
    }
    public void EnterTargetingMode(AttackResource attack)
    {
        Debug.WriteLine($"Starting to aim for :{attack.Name}");
        ToggleTargeting(true);
        
        targetingSystem.Visible = true;
        targetingSystem.SetAttackPattern(attack);
        targetingSystem.Start(facing);
        
    }
    public void ExitTargetingMode()
    {
        ToggleTargeting(false);
        targetingSystem.Reset();
        targetingSystem.Visible = false;
        
    }
}
