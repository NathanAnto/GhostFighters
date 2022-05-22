using Godot;
using System;

public class StrongGloveEnemy : StaticBody2D
{
    private CustomSignals cs;

    public override void _Ready() {
        cs = GetNode<CustomSignals>("/root/CS");
        cs.Connect("SetEnemyGloveSprite", this, "SetSprite");
    }

    public void SetSprite(string color) {
        GetNode<Sprite>("Sprite").Texture = (Texture)GD.Load($"res://Textures/Gloves/Strong/{color}_strong.png");
    }
}
