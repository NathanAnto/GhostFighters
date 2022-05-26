using Godot;

public class StrongGlove : Area2D
{
    [Export] public int damage;

    private CustomSignals cs;

    public override void _Ready() {
        cs = GetNode<CustomSignals>("/root/CS");
        cs.Connect("SetGloveSprite", this, "SetSprite");
    }

    public void SetSprite(string color) {
        GetNode<Sprite>("Sprite").Texture = (Texture)GD.Load($"res://Textures/Gloves/Strong/{color}_strong.png");
    }

    public void ColliderHit(Node node) {
        if(node.IsInGroup("Enemy") && node.HasMethod("TakeDamage")) {
            cs.EmitSignal(nameof(CustomSignals.DealDamageToEnemy), damage);
        }
    }
}
