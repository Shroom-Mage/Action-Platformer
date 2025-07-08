using Godot;
using System;

namespace ActionPlatformer {
	public partial class CombatantModel : Node3D {
		private AnimationTree _animationTree = null;
		private AnimationNodeStateMachinePlayback _stateMachine = null;
		private String _moveTilt = "parameters/Stand/IdleRunTilt/Tilt/add_amount";
		private String _moveSpeed = "parameters/Stand/IdleRunTilt/IdleRun/blend_position";
		private ExpressionManager _expressionManager = null;

		public override void _Ready() {
			_animationTree = GetNode<AnimationTree>("%AnimationTree");
			_stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
			_expressionManager = GetNode<ExpressionManager>("%ExpressionManager");
		}

		public void SetExpression(ExpressionType expression) {
			_expressionManager.Expression = expression;
		}

		public void PlayIdleRunTilt(float speed, float tilt) {
			speed = Mathf.Clamp(speed, 0.0f, 1.0f);
			tilt = Mathf.Clamp(tilt, -1.0f, 1.0f);
			_animationTree.Set(_moveSpeed, speed);
			_animationTree.Set(_moveTilt, tilt);
			_stateMachine.Travel("Stand");
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
