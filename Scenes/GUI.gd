extends Control

onready var playerHealthBar = $PlayerHealth;
onready var enemyHealthBar = $EnemyHealth;

# Called when the node enters the scene tree for the first time.
func _ready():
	playerHealthBar.max_value = 20;
	playerHealthBar.value = 20;
	
	enemyHealthBar.max_value = 20;
	enemyHealthBar.value = 20;

func set_player_health(damage):
	playerHealthBar.value -= damage;
	
func set_enemy_health(damage):
	enemyHealthBar.value -= damage;

# ON SCENE CHANGE, RESET HEALTH TO MAX
func setup_health(enemyHealth):
	playerHealthBar.value = playerHealthBar.max_value;
	enemyHealthBar.max_value = enemyHealth;
	enemyHealthBar.value = enemyHealth;
