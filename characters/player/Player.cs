using Godot;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Player : Combatant {
        private Node3D _model = null;
        private Node3D _cameraPivot = null;
		private SpringArm3D _cameraArm = null;
		private Camera3D _camera = null;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Camera")]
		public float CameraHorizontalSpeed = 5.0f;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Camera")]
		public float CameraVerticalSpeed = 5.0f;
		[Export, ExportGroup("Camera")]
		public float CameraUpperLimit = -45.0f;
		[Export, ExportGroup("Camera")]
		public float CameraLowerLimit = 0.0f;

		public override void _Ready() {
			base._Ready();
            _model = GetNode<Node3D>("Pivot/Model");
            _cameraPivot = GetNode<Node3D>("CameraPivot");
			_cameraArm = GetNode<SpringArm3D>("CameraPivot/CameraArm");
			_camera = GetNode<Camera3D>("CameraPivot/CameraArm/Camera3D");
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

			// Move and attack
			MoveAndAttack(input, delta);

			// Get look input
			Vector2 lookInput = Input.GetVector("look_left", "look_right", "look_up", "look_down");

			// Rotate camera
			_cameraPivot.RotateObjectLocal(new Vector3(0.0f, 1.0f, 0.0f), -lookInput.X * CameraHorizontalSpeed * (float)delta);
			_cameraArm.RotateObjectLocal(new Vector3(1.0f, 0.0f, 0.0f), -lookInput.Y * CameraVerticalSpeed * (float)delta);
			_cameraArm.Rotation = new Vector3(Mathf.Clamp(_cameraArm.Rotation.X, Mathf.DegToRad(CameraUpperLimit), Mathf.DegToRad(CameraLowerLimit)), _cameraArm.Rotation.Y, _cameraArm.Rotation.Z);
			base.Space = _camera.GlobalBasis;
        }

        protected override void PlayIdle() {
            _model.Call("idle");
        }

        protected override void PlayCrouch() {
            _model.Call("crouch");
        }

        protected override void PlayMove(float speed, float tilt) {
            _model.Call("move", speed / GroundSpeed);
            _model.Set("run_tilt", tilt);
        }

        protected override void PlayJump() {
            _model.Call("jump");
        }

        protected override void PlayFall() {
            _model.Call("fall");
        }

        protected override void PlaySkid() {
            _model.Call("skid");
        }

        protected override void PlaySlide() {
            _model.Call("wall_slide");
        }
    }
}
