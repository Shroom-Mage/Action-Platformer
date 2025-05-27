using Godot;
using Godot.Collections;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Vision : Area3D {
		private Array<Node3D> _targets;

		[Export]
		public Array<Node3D> Targets {
			get { return _targets; }
			set { }
		}

		public Vision() {
			_targets = new Array<Node3D>();
		}

		public override void _Ready() {
			Monitoring = true;
		}

		private void OnBodyEntered(Node3D node) {
			if (node == null) {
				return;
			}

			_targets.Add(node);
		}

		private void OnBodyExited(Node3D node) {
			if (node == null) {
				return;
			}

			_targets.Remove(node);
		}
	}
}
