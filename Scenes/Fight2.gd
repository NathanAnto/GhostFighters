extends Node2D

signal scene_changed(levelName);

# Called when the node enters the scene tree for the first time.
func _ready():
	$Enemy2.connect("enemyDied", self, "on_enemy_death");

func on_enemy_death():
	emit_signal("scene_changed", "Fight3");

func on_player_death():
	emit_signal("scene_changed", "GameOver");

func cleanup(anim_player: AnimationPlayer):
	if anim_player.is_playing():
		yield(anim_player, "finished");
	queue_free();
