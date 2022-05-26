using Godot;
using System;

public class Enemy1 : KinematicBody2D
{
    [Export] public int health = 5;
    [Export] public string glovesColor = "blue";

    [Signal] public delegate void died();
    [Signal] public delegate void damageTaken(int damage);

    private Sprite sprite;
    private Texture GhostNormal, GhostHit;

    private CustomSignals cs;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        cs = GetNode<CustomSignals>("/root/CS");
        cs.EmitSignal(nameof(CustomSignals.SetEnemyGloveSprite), glovesColor);
        cs.Connect("DealDamageToEnemy", this, "TakeDamage");
        
        GhostNormal = (Texture)GD.Load("res://Textures/Ghosts/GhostNormal.png");
        GhostHit = (Texture)GD.Load("res://Textures/Ghosts/GhostHit.png");

        sprite = GetNode<Sprite>("Sprite");
        sprite.Texture = GhostNormal;
    }

    public void TakeDamage(int damage) {
        health -= damage;

        EmitSignal("damageTaken", damage);

        if (health <= 0) Die();
    }

    private void Die() {
        GD.Print("DEAD");
        cs.Disconnect("DealDamageToEnemy", this, "TakeDamage");
        // cs.EmitSignal(nameof(CustomSignals.ChangeScene), "res://Scenes/Fight2.tscn");
        EmitSignal("died");
    }
}
