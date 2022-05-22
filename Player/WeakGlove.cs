using Godot;
using System;

public class WeakGlove : StaticBody2D
{
    private CustomSignals cs;

    public override void _Ready() {
        cs = GetNode<CustomSignals>("/root/CS");
        cs.Connect("SetGloveSprite", this, "SetSprite");
    }

    public void SetSprite(string color) {
        GetNode<Sprite>("Sprite").Texture = (Texture)GD.Load($"res://Textures/Gloves/Weak/{color}_weak.png");
    }
}
