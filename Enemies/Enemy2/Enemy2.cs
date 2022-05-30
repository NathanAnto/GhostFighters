using Godot;
using System;

public class Enemy2 : KinematicBody2D
{
    [Export] public int health = 10;
	[Export] public string glovesColor = "blue";

	[Signal] public delegate void died();
	[Signal] public delegate void damageTaken(int damage);

	private State state = State.MOVE;
	
	
	private const float MAXSPEED = 50f;
	private Sprite sprite;
	private Texture GhostNormal, GhostHit, GhostStun;
	private Node2D player;
	private Position2D edge;
	private Timer timer;

	private Vector2 velocity;
	private float difference;

	private AnimationPlayer weakAnimPlayer;
	private AnimationPlayer strongAnimPlayer;
	
	private CustomSignals cs;

		// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		cs = GetNode<CustomSignals>("/root/CS");
		cs.EmitSignal(nameof(CustomSignals.SetEnemyGloveSprite), glovesColor);
		cs.Connect("DealDamageToEnemy", this, "TakeDamage");
		
		GhostNormal = (Texture)GD.Load("res://Textures/Ghosts/GhostNormal.png");
		GhostHit = (Texture)GD.Load("res://Textures/Ghosts/GhostHit.png");
		GhostStun = (Texture)GD.Load("res://Textures/Ghosts/GhostStun.png");
		
		sprite = GetNode<Sprite>("Sprite");
		sprite.Texture = GhostNormal;
		player = GetParent().GetNode<KinematicBody2D>("Player");
		edge = GetNode<Position2D>("Edge");
		timer = GetNode<Timer>("Timer");
		timer.Connect("timeout", this, "TimerTimeout");
		
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
		difference = player.GlobalPosition.DistanceTo(GlobalPosition);
	}

	private void MoveState(float delta)
	{
		// sprite.Texture = GhostNormal;
		velocity = Vector2.Zero;
		
		// Go towards
		if (difference > 55f) {
			velocity = GlobalPosition.DirectionTo(player.Position);
			sprite.Texture = GhostHit;
		}
		else if (difference < 35f) {
			velocity = GlobalPosition.DirectionTo(edge.Position);
			sprite.Texture = GhostNormal;
		}

		velocity.y = 0;
		Position += velocity * MAXSPEED * delta;

		var rand = GD.Randf() * 10;
		// make higher if more aggressive
		var willAct = rand <= 4f;

		if (willAct) {
			if(rand <= 1)
				state = State.WEAK_ATTACK;
			if (rand > 3) {
				weakAnimPlayer.Play("block");
				strongAnimPlayer.Play("block");
				state = State.BLOCK;
			}
		}
	}

	private void BlocKState(float delta)
	{
		sprite.Texture = GhostHit;
		if(timer.TimeLeft == 0) {
			timer.Start(GD.Randf()+0.5f);
		}
	}

	private void WeakAttackState(float delta)
	{
		sprite.Texture = GhostHit;
		velocity = GlobalPosition.DirectionTo(player.Position);
		
		if (difference < 36f) {
			velocity = GlobalPosition.DirectionTo(edge.Position);
			weakAnimPlayer.Play("hit");
		}
		
		velocity.y = 0;
		Position += velocity * MAXSPEED * delta;
		
		if(!weakAnimPlayer.IsConnected("animation_finished", this, "AttackEnd")) {
			weakAnimPlayer.Connect("animation_finished", this, "AttackEnd");
		}
	}

	private void StrongAttackState(float delta)
	{
		sprite.Texture = GhostHit;
		velocity = GlobalPosition.DirectionTo(player.Position);
		// Move towards player to hit
		
		if (difference < 30f) {
			velocity = GlobalPosition.DirectionTo(edge.Position);
			strongAnimPlayer.Play("hit");
		}
		
		velocity.y = 0;
		Position += velocity * MAXSPEED * delta;
		
		if(!strongAnimPlayer.IsConnected("animation_finished", this, "AttackEnd")) {
			strongAnimPlayer.Connect("animation_finished", this, "AttackEnd");
		}
	}

	private void StunState(float delta) {
		sprite.Texture = GhostStun;
		if(timer.TimeLeft == 0) {
			timer.Start(2f);
		}
	}

	private void DeadState(float delta)
	{
		sprite.Texture = GhostStun;
	}

	public void AttackEnd(string name) {
		if(weakAnimPlayer.IsConnected("animation_finished", this, "AttackEnd"))
			weakAnimPlayer.Disconnect("animation_finished", this, "AttackEnd");
		if(strongAnimPlayer.IsConnected("animation_finished", this, "AttackEnd"))
			strongAnimPlayer.Disconnect("animation_finished", this, "AttackEnd");

		if (state == State.WEAK_ATTACK)
			state = State.STRONG_ATTACK;
		else
			state = State.MOVE;
	}

	private void TimerTimeout() {
		timer.Stop();
		switch (state)
		{
			case State.BLOCK:
				weakAnimPlayer.PlayBackwards("block");
				strongAnimPlayer.PlayBackwards("block");
				break;
			case State.WEAK_ATTACK:
				break;
			case State.STRONG_ATTACK:
				break;
		}
		state = State.MOVE;
	}

	public void TakeDamage(int damage) {
		if(state == State.BLOCK) damage -= 1;
		health -= damage;

		if (damage == 2 && state != State.BLOCK) state = State.STUN;

		EmitSignal("damageTaken", damage);

		if (health <= 0) Die();
	}

	private void Die()
	{
		state = State.DEAD;
		GD.Print("DEAD");
		cs.Disconnect("DealDamageToEnemy", this, "TakeDamage");
		EmitSignal("died");
	}
}
