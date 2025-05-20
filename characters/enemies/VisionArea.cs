using Godot;
using Godot.Collections;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class VisionArea : Area3D {
		private Array<Player> _targets;

		public Array<Player> Targets {
			get { return _targets; }
		}

		public VisionArea() {
			_targets = new Array<Player>();
		}

		public override void _Ready() {
			Monitoring = true;
		}

		private void OnBodyEntered(Node3D node) {
			Player target = node as Player;
			if (target == null) {
				return;
			}

			_targets.Add(target);

			GD.Print(_targets);
		}

		private void OnBodyExited(Node3D node) {
			Player target = node as Player;
			if (target == null) {
				return;
			}

			_targets.Remove(target);

			GD.Print(_targets);
		}
	}
}
