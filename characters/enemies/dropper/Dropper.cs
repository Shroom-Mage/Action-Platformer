using Godot;
using System;

namespace ActionPlatformer {
	public partial class Dropper : Enemy {
		private CombatantModel _model = null;

		public override void _Ready() {
			base._Ready();
			_model = GetNode<CombatantModel>("Pivot/Model");
        }

        protected override void PlayIdle() {
            _model.PlayIdle();
        }

        protected override void PlayJump() {
            _model.PlayJump();
            Forward = new Vector2(Velocity.X, Velocity.Z);
        }

        protected override void PlayFall() {
            _model.PlayFall();
        }

        protected override void PlayDrop() {
            _model.PlayDrop();
        }

        protected override void PlayDropLand() {
            _model.PlayDropLand();
        }
    }
}
