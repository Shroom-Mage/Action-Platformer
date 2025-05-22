using Godot;
using Godot.Collections;
using System;
using ActionPlatformer;

[Tool]
public partial class SelectTarget : BTAction
{
	public override Status _Tick(double delta) {

		// Get all visible targets
		Array<Node3D> targets = (Array<Node3D>)Blackboard.GetVar("targets");

		// Return failure if no targets within vision
		if (targets.Count == 0) {
			return Status.Failure;
		}

		// Find the closest target
		Node3D self = Agent as Node3D;
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
