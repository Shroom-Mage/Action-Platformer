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
		Node3D closest = null;
		float closestDistance = 1000.0f;

		for (int i = 0; i < targets.Count; i++) {
			// Check if closer
			if ((self.GlobalPosition - targets[i].GlobalPosition).Length() >= closestDistance) {
				continue;
			}
			// Check for line of sight
			Vector3 start = self.GlobalPosition;
			Vector3 end = start + targets[i].GlobalPosition;
			PhysicsDirectSpaceState3D space = self.GetWorld3D().DirectSpaceState;
			PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(start, end, 0b1);
			Dictionary result = space.IntersectRay(parameters);
			if (result.Count > 0) {
				continue;
			}
			// Update closest
			closest = targets[i];
		}

		// Return failure if no targets were in sight
		if (closest == null) {
			return Status.Failure;
		}

		Blackboard.SetVar("destination", closest.GlobalPosition);

		// Return success if closest target found
		return Status.Success;
	}
}
