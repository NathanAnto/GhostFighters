using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export] public int health;

	[Signal] public delegate void died();
	[Signal] public delegate void damageTaken(int damage);
	
	public Vector2 ScreenSize; // Size of the game window.
	public const int MAXSPEED = 50;
	public State state = State.MOVE;
	
	private Sprite sprite;
	private Texture GhostNormal, GhostHit, GhostStun;
	private Timer timer;


	private CustomSignals cs;

	private AnimationPlayer weakAnimPlayer;
	private AnimationPlayer strongAnimPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		cs = GetNode<CustomSignals>("/root/CS");
		cs.EmitSignal(nameof(CustomSignals.SetGloveSprite), "red");
		cs.Connect("DealDamageToPlayer", this, "TakeDamage");

		ScreenSize = GetViewportRect().Size;
		GhostNormal = (Texture)GD.Load("res://Textures/Ghosts/GhostNormal.png");
		GhostHit = (Texture)GD.Load("res://Textures/Ghosts/GhostHit.png");
		GhostStun = (Texture)GD.Load("res://Textures/Ghosts/GhostStun.png");
		timer = GetNode<Timer>("Timer");
		timer.Connect("timeout", this, "TimerTimeout");

		sprite = GetNode<Sprite>("Sprite");

		weakAnimPlayer = GetNode<Area2D>("WeakGlove").GetNode<AnimationPlayer>("AnimationPlayer");
		strongAnimPlayer = GetNode<Area2D>("StrongGlove").GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(float delta)
	{
		switch(state) {
			case State.MOVE:
				MoveState(delta);
				break;
			case State.BLOCK:
				BlocKState(delta);
				break;
			case State.WEAK_ATTACK:
				WeakAttackState(delta);
				break;
			case State.STRONG_ATTACK:
				StrongAttackState(delta);
				break;
			case State.STUN:
				StunState(delta);
				break;
			case State.DEAD:
				DeadState(delta);
				break;
		}
	}

	private void MoveState(float delta)
	{
		sprite.Texture = GhostNormal;

		var movement = new Vector2(
			Input.GetActionStrength("right") - Input.GetActionStrength("left"), 0
		);

		MoveAndSlide(movement * MAXSPEED);

		if(Input.IsActionJustPressed("weak_hit")) state = State.WEAK_ATTACK;
		if(Input.IsActionJustPressed("strong_hit")) state = State.STRONG_ATTACK;

		if(Input.IsActionJustPressed("block")) {
			weakAnimPlayer.Play("block");
			strongAnimPlayer.Play("block");
			state = State.BLOCK;
		}
	}

	private void BlocKState(float delta)
	{
		sprite.Texture = GhostHit;
		if (Input.IsActionJustReleased("block")) {
			GD.Print("Block released");
			weakAnimPlayer.PlayBackwards("block");
			strongAnimPlayer.PlayBackwards("block");
			state = State.MOVE;
		}
	}

	private void WeakAttackState(float delta)
	{
		sprite.Texture = GhostHit;
		weakAnimPlayer.Play("hit");
		if(!weakAnimPlayer.IsConnected("animation_finished", this, "AttackEnd")) {
			weakAnimPlayer.Connect("animation_finished", this, "AttackEnd");
		}
	}

	private void StrongAttackState(float delta)
	{
		sprite.Texture = GhostHit;
		strongAnimPlayer.Play("hit");
		if(!strongAnimPlayer.IsConnected("animation_finished", this, "AttackEnd")) {
			strongAnimPlayer.Connect("animation_finished", this, "AttackEnd");
		}
	}

	public void AttackEnd(string name) {
		if(weakAnimPlayer.IsConnected("animation_finished", this, "AttackEnd"))
			weakAnimPlayer.Disconnect("animation_finished", this, "AttackEnd");
		if(strongAnimPlayer.IsConnected("animation_finished", this, "AttackEnd"))
			strongAnimPlayer.Disconnect("animation_finished", this, "AttackEnd");

		state = State.MOVE;
	}

	private void StunState(float delta) {
		sprite.Texture = GhostStun;
		if(timer.TimeLeft == 0) {
			timer.Start(1.5f);
		}
	}

	private void DeadState(float delta)
	{
		sprite.Texture = GhostStun;
	}

	public void TakeDamage(int damage) {
		if(state == State.BLOCK) damage -= 1;
		health -= damage;

		if (damage == 2 && state != State.BLOCK) state = State.STUN;

		EmitSignal("damageTaken", damage);

		if (health <= 0) {
			Die();
			cs.EmitSignal(nameof(CustomSignals.GameOver));
		}
	}
	
	private void TimerTimeout() {
		timer.Stop();
		state = State.MOVE;
	}

	private void Die() {
		GD.Print("DEAD");
		cs.EmitSignal(nameof(CustomSignals.GameOver));
	}
}

public enum State {
	MOVE,
	BLOCK,
	WEAK_ATTACK,
	STRONG_ATTACK,
	STUN,
	DEAD
}
