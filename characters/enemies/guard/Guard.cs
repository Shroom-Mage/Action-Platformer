using Godot;
using System;

namespace ActionPlatformer {
	public partial class Guard : Enemy {
		private CombatantModel _model = null;

		public override void _Ready() {
			base._Ready();
			_model = GetNode<CombatantModel>("Pivot/Model");
		}

		protected override void PlayIdle() {
            _model.PlayIdle();
        }

		protected override void PlayCrouch() {
            _model.PlayCrouch();
        }

		protected override void PlayRun(float speed, float tilt) {
            _model.PlayRun(speed, tilt);
        }

		protected override void PlayJump() {
            _model.PlayJump();
        }

		protected override void PlayFall() {
            _model.PlayFall();
        }

		protected override void PlaySkid() {
            _model.PlaySkid();
        }

		protected override void PlaySlide() {
            _model.PlayWallSlide();
        }
	}
}