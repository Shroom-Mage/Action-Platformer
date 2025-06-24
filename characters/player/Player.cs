using Godot;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Player : Combatant {
		private FollowCamera _camera = null;

		[Export, ExportGroup("Camera")]
		public bool Interior {
			get {
				return _camera != null ? _camera.Interior : false;
			}
			set {
				if (_camera != null) {
                    _camera.Interior = value;
                }
			}
		}

		public override void _Ready() {
			base._Ready();
			_camera = GetNode<FollowCamera>("%FollowCamera");
		}

		public override void _PhysicsProcess(double delta) {
			// Get input
			CombatInput input;
			input.movement = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			input.bJumpPress = Input.IsActionJustPressed("jump");
			input.bJumpHold = Input.IsActionPressed("jump");
			input.bCrouchPress = Input.IsActionJustPressed("crouch");
			input.bCrouchHold = Input.IsActionPressed("crouch");
			input.bAttackPress = Input.IsActionJustPressed("attack");
			input.bAttackHold = Input.IsActionPressed("attack");
			input.bBlockPress = Input.IsActionJustPressed("block");
			input.bBlockHold = Input.IsActionPressed("block");
			Vector2 lookInput = Input.GetVector("look_left", "look_right", "look_up", "look_down");

			// Look
			_camera.Rotate(lookInput, delta);
			Space = _camera.GetCameraBasis();

			// Move and attack
			MoveAndAttack(input, delta);
		}

		protected override void PlayMove(float speed, float tilt) {
			Model.PlayLocomotion(speed, tilt);
		}

		protected override void PlayCrouch() {
			Model.PlayCrouch();
		}

		protected override void PlayJump() {
			Model.PlayJump();
		}

		protected override void PlayFall() {
			Model.PlayFall();
		}

		protected override void PlaySkid() {
			Model.PlaySkid();
		}

		protected override void PlaySlide() {
			Model.PlayWallSlide();
		}

		protected override void PlayDrop() {
			Model.PlayFall();
		}

		protected override void PlayDropLand() {
			Model.PlayFall();
		}
	}
}
