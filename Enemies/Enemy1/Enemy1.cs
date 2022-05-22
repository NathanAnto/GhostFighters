using Godot;
using System;

public class Enemy1 : KinematicBody2D
{
    public int hp;

    private Sprite sprite;
    private Texture GhostNormal, GhostHit;

    private CustomSignals cs;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        cs = GetNode<CustomSignals>("/root/CS");
        cs.EmitSignal(nameof(CustomSignals.SetEnemyGloveSprite), "blue");
        
        GhostNormal = (Texture)GD.Load("res://Textures/Ghosts/GhostNormal.png");
        GhostHit = (Texture)GD.Load("res://Textures/Ghosts/GhostHit.png");

        sprite = GetNode<Sprite>("Sprite");
        sprite.Texture = GhostNormal;
    }

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
