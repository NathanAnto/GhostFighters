using Godot;
using System;

public class WeakGloveEnemy : Area2D
{
    [Export] public int damage;

    private CustomSignals cs;

    public override void _Ready() {
        cs = GetNode<CustomSignals>("/root/CS");
        cs.Connect("SetEnemyGloveSprite", this, "SetSprite");
    }

    public void SetSprite(string color) {
        GD.Print(color);
        GetNode<Sprite>("Sprite").Texture = (Texture)GD.Load($"res://Textures/Gloves/Weak/{color}_weak.png");
    }

    public void ColliderHit(Node node) {
        if(node.IsInGroup("Player") && node.HasMethod("TakeDamage")) {
            cs.EmitSignal(nameof(CustomSignals.DealDamageToPlayer), damage);
        }
    }
}
