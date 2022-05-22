using Godot;
using System;

public class CustomSignals : Node
{
    [Signal] public delegate void SetGloveSprite(string color);
    [Signal] public delegate void EndHitAnim(string color);

    [Signal] public delegate void SetEnemyGloveSprite(string color);
    [Signal] public delegate void EndEnemyHitAnim(string color);
}
