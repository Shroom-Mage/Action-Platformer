using Godot;
using System;

namespace ActionPlatformer {
    [GlobalClass]
    public partial class Enemy : Combatant {
		// Called when the node enters the scene tree for the first time.
		public override void _Ready() {
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta) {
		}

		public override string ToString() {
			return Name;
		}
	}
}
