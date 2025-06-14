using Godot;
using System;

namespace ActionPlatformer {
	public partial class Dropper : Enemy {

		public override void _Ready() {
			base._Ready();
		}

		protected override void PlayMove(float speed, float tilt) {
			Model.PlayIdle();
		}

		protected override void PlayJump() {
			Model.PlayJump();
			Forward = new Vector2(Velocity.X, Velocity.Z);
		}

		protected override void PlayFall() {
			Model.PlayFall();
		}

		protected override void PlayDrop() {
			Model.SetExpression(ExpressionType.Fierce);
			Model.PlayDrop();
		}

		protected override void PlayDropLand() {
			Model.SetExpression(ExpressionType.Fierce);
			Model.PlayDropLand();
		}

		protected override void PlayStunned() {
			Model.SetExpression(ExpressionType.Hurt);
		}
	}
}
