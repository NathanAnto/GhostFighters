using Godot;
using System;

public class Player : KinematicBody2D
{
    public int hp;

    public Vector2 ScreenSize; // Size of the game window.
	public const int MAXSPEED = 50;

    private Sprite sprite;
    private Texture GhostNormal, GhostHit;
    
	private State state = State.MOVE;

    private CustomSignals cs;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        cs = GetNode<CustomSignals>("/root/CS");
        cs.EmitSignal(nameof(CustomSignals.SetGloveSprite), "red");

        ScreenSize = GetViewportRect().Size;
        GhostNormal = (Texture)GD.Load("res://Textures/Ghosts/GhostNormal.png");
        GhostHit = (Texture)GD.Load("res://Textures/Ghosts/GhostHit.png");

        sprite = GetNode<Sprite>("Sprite");
    }

    public override void _Process(float delta)
    {
        switch(state) {
			case State.MOVE: 
				MoveState(delta);
				break;
			case State.WEAK_ATTACK:
				WeakAttackState(delta);
				break;            
			case State.STRONG_ATTACK:
				StrongAttackState(delta);
				break;
		}
    }

    private void MoveState(float delta)
    {
        sprite.Texture = GhostNormal;
	    var inputVector = Vector2.Zero;
        
        if (Input.IsActionPressed("right")) {
            inputVector.x += 1;
        }

        if (Input.IsActionPressed("left")) {
            inputVector.x -= 1;
        }
        
        if (inputVector.Length() > 0) {
            inputVector = inputVector.Normalized() * MAXSPEED;
        }

        Position += inputVector * delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.x, 0, ScreenSize.x),
            y: Mathf.Clamp(Position.y, 0, ScreenSize.y)
        );

		if(Input.IsActionJustPressed("weak_hit")) state = State.WEAK_ATTACK;
		if(Input.IsActionJustPressed("strong_hit")) state = State.STRONG_ATTACK;
    }

    private void WeakAttackState(float delta)
    {
        sprite.Texture = GhostHit;
        var animPlayer = GetNode<StaticBody2D>("WeakGlove").GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Play("hit");
        if(!animPlayer.IsConnected("animation_finished", this, "AttackEnd")) {
            animPlayer.Connect("animation_finished", this, "AttackEnd");
        }
    }

    private void StrongAttackState(float delta)
    {
        sprite.Texture = GhostHit;
        var animPlayer = GetNode<StaticBody2D>("StrongGlove").GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Play("hit");
        if(!animPlayer.IsConnected("animation_finished", this, "AttackEnd")) {
            animPlayer.Connect("animation_finished", this, "AttackEnd");
        }
    }
    
	public void AttackEnd(string name) => state = State.MOVE;    

    public void TakeDamage(int damage) {
        hp -= damage;
        if (hp <= 0) {
            Die();
        }
    }

    private void Die() {
        GD.Print("DEAD");
    }
}

enum State {
	MOVE,
	WEAK_ATTACK,
	STRONG_ATTACK
}