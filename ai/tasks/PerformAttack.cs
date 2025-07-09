using Godot;
using Godot.Collections;
using System;
using ActionPlatformer;

[Tool]
public partial class PerformAttack : BTAction {
	[Export]
	public float Range = 1.0f;

	public override string _GenerateName() {
		return "Perform a Standing Attack on a target within " + Range + " meters.";
	}

	public override Status _Tick(double delta) {
		Enemy self = Agent as Enemy;
		Array<Node3D> targets = (Array<Node3D>)Blackboard.GetVar("targets");

		// Return failure if no targets within vision
		if (targets.Count == 0) {
			return Status.Failure;
		}

		// Attack if any target is in range
		for (int i = 0; i < targets.Count; i++) {
			if ((self.GlobalPosition - targets[i].GlobalPosition).Length() < Range) {
				self.PressAttack();
				return Status.Success;
			}
		}

		return Status.Failure;
	}
}
