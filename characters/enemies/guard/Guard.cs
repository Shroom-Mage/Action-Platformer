using Godot;
using System;

namespace ActionPlatformer {
	public partial class Guard : Enemy {

		public override void _Ready() {
			base._Ready();
		}

		protected override void PlayIdle() {
            Model.PlayLocomotion(0.0f, 0.0f);
        }

		protected override void PlayCrouch() {
            Model.PlayCrouch();
        }

		protected override void PlayRun(float speed, float tilt) {
            Model.PlayLocomotion(speed, tilt);
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
	}
}