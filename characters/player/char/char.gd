class_name CharAnim extends Node3D

@onready var animation_tree = %AnimationTree
@onready var state_machine : AnimationNodeStateMachinePlayback = animation_tree.get("parameters/StateMachine/playback")
@onready var move_tilt_path : String = "parameters/StateMachine/Move/tilt/add_amount"
@onready var move_speed : String = "parameters/StateMachine/Move/IdleRun/blend_position"

var run_tilt = 0.0 : set = set_run_tilt

func set_run_tilt(value : float):
	run_tilt = clamp(value, -1.0, 1.0)
	animation_tree.set(move_tilt_path, run_tilt)

func idle():
	state_machine.travel("Idle")

func move(speed : float):
	state_machine.travel("Move")
	speed = clamp(speed, 0.0, 1.0)
	animation_tree.set(move_speed, speed)

func crouch():
	state_machine.travel("Crouch")

func skid():
	state_machine.travel("Skid")

func fall():
	state_machine.travel("Fall")

func jump():
	state_machine.travel("Jump")

func edge_grab():
	state_machine.travel("EdgeGrab")

func wall_slide():
	state_machine.travel("WallSlide")
