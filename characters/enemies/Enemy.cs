using Godot;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Enemy : Combatant {
		private VisionArea _vision = null;

		public CombatInput Input;
		
		public override void _Ready() {
			base._Ready();
			_vision = GetNode<VisionArea>("VisionArea");
		}

		public override void _PhysicsProcess(double delta) {
			MoveAndAttack(Input, delta);
		}
	}
}
