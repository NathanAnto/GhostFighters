extends Control

onready var playerHealthBar = $PlayerHealth;
onready var enemyHealthBar = $EnemyHealth;

# Called when the node enters the scene tree for the first time.
func _ready():
	playerHealthBar.max_value = 50;
	playerHealthBar.value = 50;
	
	enemyHealthBar.max_value = 5;
	enemyHealthBar.value = 5;

func set_player_health(damage):
	playerHealthBar.value -= damage;
	
func set_enemy_health(damage):
	enemyHealthBar.value -= damage;
	print("Enemy took " + damage as String);

# ON SCENE CHANGE, RESET HEALTH TO MAX
func setup_health(enemyHealth):
	print("setting new health " + enemyHealth as String);
	enemyHealthBar.max_value = enemyHealth;
	enemyHealthBar.value = enemyHealth;
