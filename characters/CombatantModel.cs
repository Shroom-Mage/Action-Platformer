using Godot;
using System;

namespace ActionPlatformer {
	public partial class CombatantModel : Node3D {
		private AnimationTree _animationTree = null;
		private AnimationNodeStateMachinePlayback _stateMachine = null;
        private AnimationNodeStateMachinePlayback _standStateMachine = null;
        private String _moveTilt = "parameters/Stand/Locomotion/Tilt/add_amount";
		private String _moveSpeed = "parameters/Stand/Locomotion/IdleRun/blend_position";

		public override void _Ready() {
			_animationTree = GetNode<AnimationTree>("%AnimationTree");
			_stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
            _standStateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/Stand/playback");
        }

		public void PlayIdle() {
			_stateMachine.Travel("Stand");
			_standStateMachine.Travel("Idle");
		}

		public void PlayRun() {
			_stateMachine.Travel("Stand");
			_standStateMachine.Travel("Run");
		}

		public void PlayLocomotion(float speed, float tilt) {
			speed = Mathf.Clamp(speed, 0.0f, 1.0f);
			tilt = Mathf.Clamp(tilt, -1.0f, 1.0f);
			_animationTree.Set(_moveSpeed, speed);
			_animationTree.Set(_moveTilt, tilt);
            _stateMachine.Travel("Stand");
            _standStateMachine.Travel("Locomotion");
		}

		public void PlaySkid() {
			_stateMachine.Travel("Skid");
		}

		public void PlayCrouch() {
			_stateMachine.Travel("Crouch");
		}

		public void PlayJump() {
			_stateMachine.Travel("Jump");
		}

		public void PlayFall() {
			_stateMachine.Travel("Fall");
		}

		public void PlayWallSlide() {
			_stateMachine.Travel("WallSlide");
		}

		public void PlayDrop() {
			_stateMachine.Travel("Drop");
        }

        public void PlayDropLand() {
            _stateMachine.Travel("DropLand");
        }
    }
}
