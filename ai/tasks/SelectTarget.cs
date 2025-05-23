using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class SelectTarget : BTAction {
	public override string _GenerateName() {
		return "Select nearest target within Vision.";
	}
	public override Status _Tick(double delta) {
		Node3D self = Agent as Node3D;
		Array<Node3D> targets = (Array<Node3D>)Blackboard.GetVar("targets");

		// Return failure if no targets within vision
		if (targets.Count == 0) {
			return Status.Failure;
		}

		// Find the closest target
		Vector3 destination = targets[0].GlobalPosition;
		float closestDistance = (self.GlobalPosition - destination).Length();

		for (int i = 1; i < targets.Count; i++) {
			if ((self.GlobalPosition - targets[i].GlobalPosition).Length() < closestDistance) {
				destination = targets[i].GlobalPosition;
			}
		}
		Blackboard.SetVar("destination", destination);

		// Return success if closest target found
		return Status.Success;
	}
}
