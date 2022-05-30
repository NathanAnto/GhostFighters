using Godot;
using System;

public class CustomSignals : Node
{
    [Signal] public delegate void SetGloveSprite(string color);
    [Signal] public delegate void SetEnemyGloveSprite(string color);
    
    [Signal] public delegate void DealDamageToEnemy(int damage);
    [Signal] public delegate void DealDamageToPlayer(int damage);

    [Signal] public delegate void ChangeScene(string scenePath);
    [Signal] public delegate void GameOver();    
}
