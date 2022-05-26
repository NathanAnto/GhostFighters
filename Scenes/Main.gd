extends Node

onready var current_level = $CurrentScene;
onready var anim_player = $AnimationPlayer;

var next_level = null;

# Called when the node enters the scene tree for the first time.
func _ready():
	# current_level.connect("scene_changed", self, "change_scene");
	pass


func change_scene(next_level_name: String):
	print(next_level_name)
	next_level = load("res://Scenes/" + next_level_name + ".tscn").instance();
	anim_player.play("fade_in");
	next_level.connect("scene_changed", self, "change_scene");


func on_animation_finished(anim_name: String):
	match anim_name:
		"fade_in":
			current_level.cleanup(anim_player);
			current_level = next_level;
			current_level.name = "CurrentScene";
			add_child(current_level);
			next_level = null;
			anim_player.play("fade_out");
