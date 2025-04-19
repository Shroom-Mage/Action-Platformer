using Godot;
using Godot.Collections;
using System;

namespace ActionPlatformer {
	public partial class AttackArea : Area3D {
		private Array<Node3D> _targets;

		public Array<Node3D> Targets {
			get { return _targets; }
		}

		public AttackArea() {
			_targets = new Array<Node3D>();
		}

		public override void _Ready() {
			Monitoring = true;
		}

		private void OnBodyEntered(Node3D node) {
			Combatant body = node as Combatant;
			if (body == null) {
				return;
			}

			_targets.Add(node);

			GD.Print(_targets);
		}

		private void OnBodyExited(Node3D node) {
			Combatant body = node as Combatant;
			if (body == null) {
				return;
			}

			_targets.Remove(node);

			GD.Print(_targets);
		}
	}
}
