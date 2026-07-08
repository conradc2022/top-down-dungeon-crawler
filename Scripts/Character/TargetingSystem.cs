using Attack;
using Godot;
using Interactables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Character;

public enum TargetState
{
    None,
    Self,
    Ally,
    Neutral,
    Enemy,
    Interactable
}
public partial class TargetingSystem : CharacterBody2D
{
    [Export]
    public float speed = 1;
    [Export]
    public int tileSize = 16;
    bool moving = false;
    Vector2 location;
    Node2D pathTiles;
    Sprite2D boxReticule;
    Texture2D boxTexture;
    Sprite2D crossReticule;
    private Dictionary<Vector2I, Node2D> IsColliding = new()
    {
        {Vector2I.Up, null},
        {Vector2I.Down, null},
        {Vector2I.Left, null},
        {Vector2I.Right, null}
    };
    AttackResource attackPattern;
    Vector2 relativeGridPosition;
    [Export]
    Color AllyTargeted;
    [Export]
    Color EnemyTargeted;
    [Export]
    Color NeutralTargeted;
    [Export]
    Color NothingTargeted;
    [Export]
    Color InteractableTargeted;
    
    TargetState targetState = TargetState.None;
    HashSet<Vector2> impactedTiles;
    Timer timer;
    public override void _Ready()
    {
        pathTiles = GetNode<Node2D>("PathNodes");
        boxReticule = GetNode<Sprite2D>("PathNodes/BoxReticle");
        boxTexture = boxReticule.Texture;
        crossReticule = GetNode<Sprite2D>("Area2D/CrossReticle");
        relativeGridPosition = Vector2.Zero;
        SetTargetState(targetState);
        Visible = false;
        timer = GetNode<Timer>("Timer");
        timer.Timeout += MoveGridComplete;
        timer.WaitTime = 1.0/speed;
        timer.OneShot = true;
    }
    public void SetAttackPattern(AttackResource attackResource)
    {
        attackPattern = attackResource;
    }
    public void Start(Vector2? facing = null)
    {
        Visible = true;
        if(facing == null)
        {
            Position = tileSize* Vector2.Zero;
            relativeGridPosition = Vector2.Zero;
        }
        else
        {
            Position = tileSize* facing.Value;
            relativeGridPosition = facing.Value;
        }
        UpdateImpactedTiles(new());
    }
    public void Reset()
    {
        Position = tileSize* Vector2.Zero;
        relativeGridPosition = Vector2.Zero;
        UpdateImpactedTiles(new());
        Visible = false;

    }
    public void UpdateImpactedTiles(HashSet<Vector2> tiles)
    {
        if(tiles.Count == 0)
        {
            foreach(Node node in pathTiles.GetChildren())
            {
                pathTiles.RemoveChild(node);
            }
        }
        else
        {
            foreach(Vector2 tile in tiles)
            {
                if(impactedTiles.Contains(tile))
                {
                    continue;
                }
                else
                {
                    Sprite2D newSprite = new();
                    newSprite.Texture = boxTexture;
                    newSprite.Position = tileSize*tile;
                    newSprite.Scale = crossReticule.Scale;
                    pathTiles.AddChild(newSprite);
                }
            }
            List<Node2D> removedChildren = pathTiles.GetChildren().Where(x => x is Node2D).Select(node => node as Node2D).ToList();
            removedChildren = removedChildren.Where(node => impactedTiles.Contains(node.Position/tileSize) && ! tiles.Contains(node.Position/tileSize)).ToList();
            Debug.WriteLine($"From: {string.Join(',',impactedTiles)}\nTo: {string.Join(',',tiles)}");
            foreach(Node child in removedChildren)
            {
                pathTiles.RemoveChild(child);
            }

        }
        impactedTiles = tiles;
    }
    public Vector2 MoveCursor(Vector2 direction)
    {
        if(!moving){
            if(AttackChecks.IsInRange(relativeGridPosition + direction, attackPattern))
            {
                direction = MoveGrid(direction);
                relativeGridPosition += direction;
                HashSet<Vector2> tiles = AttackChecks.GetImpactedTiles(Vector2.Zero, -relativeGridPosition, tileSize, attackPattern);
                UpdateImpactedTiles(tiles);
                return relativeGridPosition.Normalized();
            }
            else
            {
                Debug.WriteLine($"Met limit of range in {direction}: {relativeGridPosition} {GlobalPosition}");
            }
        }
        return Vector2.Zero;
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
            Transform2D transform = Transform;
            transform.Origin = (transform.Origin + GetParent<Character>().Transform.Origin);
            if(!TestMove(transform, direction * tileSize,kc)){
                moving = true;
                timer.Start();
                GlobalPosition +=direction * tileSize;                
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
    }
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
        List<Vector2I> list = IsColliding.Where( entity => node.Equals(entity.Value)).Select(kvp => kvp.Key).ToList();
        foreach(Vector2I vector in list)
        {
            IsColliding[vector] = null;
        }
    }
    private void _entity_body_entered(Node2D node)
    {
        Character character = node as Character;
        if(character != null)
        {
            Character parentAsCharacter = GetParent<Character>();
            if(character.Equals(parentAsCharacter))
            {
                SetTargetState(TargetState.Self);
            }
            else if(character.characterInfo.Team == parentAsCharacter.characterInfo.Team)
            {
                SetTargetState(TargetState.Ally);
            }
            else if(character.characterInfo.Team != parentAsCharacter.characterInfo.Team && character.characterInfo.Allegence == Allegence.Neutral)
            {
                SetTargetState(TargetState.Neutral);
            }
            else if(character.characterInfo.Team != parentAsCharacter.characterInfo.Team && character.characterInfo.Allegence != Allegence.Neutral)
            {
                SetTargetState(TargetState.Enemy);
            }
        }
        AbstractInteractable interactable = node as AbstractInteractable;
        if(interactable != null)
        {
            SetTargetState(TargetState.Interactable);
        }
    }
    private void _entity_body_exited(Node2D node)
    {
        SetTargetState(TargetState.None);
    }
    private void SetTargetState(TargetState state)
    {
        //Debug.WriteLine($"TargetState: {state}");
        targetState = state;
        switch(targetState)
        {
            case TargetState.Ally:
            case TargetState.Self:
                Modulate = AllyTargeted;
                break;
            case TargetState.Neutral:
                Modulate = NeutralTargeted;
                break;
            case TargetState.Enemy:
                Modulate = EnemyTargeted;
                break;
            case TargetState.Interactable:
                Modulate = InteractableTargeted;
                break;
            default:
                Modulate = NothingTargeted;
                break;
        }
    }
}
