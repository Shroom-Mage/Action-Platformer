using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class CheckForTarget : BTCondition {

	public override string _GenerateName() {
		return "Check if a target exists.";
	}

	public override Status _Tick(double delta) {
		Array<Node3D> targets = (Array<Node3D>)Blackboard.GetVar("targets");

		if (targets.Count > 0) {
			return Status.Success;
		}
		else {
			return Status.Failure;
		}
	}
}
