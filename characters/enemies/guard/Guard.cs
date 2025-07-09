using Godot;
using System;

namespace ActionPlatformer {
	public partial class Guard : Enemy {
		public override void _Ready() {
			base._Ready();
		}

		protected override void PlayMove(float speed, float tilt) {
			Model.PlayIdleRunTilt(speed, tilt);
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
	}
}