using Godot;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Enemy : Combatant {
		private Combatant _target;

		[Export]
		public Combatant Target {
			get { return _target; }
			set { _target = value; }
		}

		public override void _Ready() {
			base._Ready();

		}
	}
}
