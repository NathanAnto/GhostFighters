extends Node2D

signal scene_changed(levelName);
signal scene_setup(enemyHealth);
signal set_player_health_ui(damage);
signal set_enemy_health_ui(damage);

export var next_level_name = "Fight";
export var next_enemy_health = 5;

# Called when the node enters the scene tree for the first time.

func on_scene_load():
	# $Player.connect("died, self, "on_player_death")
	$Enemy.connect("died", self, "on_enemy_death");
	
	# $Player.connect("damageTaken, self, "on_player_damage_taken")
	$Enemy.connect("damageTaken", self, "on_enemy_damage_taken");
	connect_all();
	

# PLAYER
func on_player_death():
	emit_signal("scene_changed", "GameOver");

func on_player_damage_taken(damage):
	emit_signal("set_player_health_ui", damage);

# ENEMY
func on_enemy_death():
	emit_signal("scene_changed", next_level_name);

func on_enemy_damage_taken(damage):
	emit_signal("set_enemy_health_ui", damage);


func cleanup(anim_player):
	if anim_player.is_playing():
		yield(anim_player, "finished");
	emit_signal("scene_setup", next_enemy_health);
	queue_free();
	
func connect_all():
	connect("scene_setup", get_node("/root/Main/GUI"), "setup_health");
	connect("set_player_health_ui", get_node("/root/Main/GUI"), "set_player_health");
	connect("set_enemy_health_ui", get_node("/root/Main/GUI"), "set_enemy_health");

func disconnect_all():
	disconnect("scene_setup", get_node("/root/Main/GUI"), "setup_health");
	disconnect("set_player_health_ui", get_node("/root/Main/GUI"), "set_player_health");
	disconnect("set_enemy_health_ui", get_node("/root/Main/GUI"), "set_enemy_health");
