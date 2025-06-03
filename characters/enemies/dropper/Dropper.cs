using Godot;
using System;

namespace ActionPlatformer {
	public partial class Dropper : Enemy {
		private Node3D _model = null;

		public override void _Ready() {
			base._Ready();
			_model = GetNode<Node3D>("Pivot/Model");
		}

		protected override void PlayJump() {
			Forward = new Vector2(Velocity.X, Velocity.Z);
		}
	}
}