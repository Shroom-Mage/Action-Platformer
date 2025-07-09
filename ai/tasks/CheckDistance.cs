using Godot;
using Godot.Collections;
using System;
using ActionPlatformer;

[Tool]
public partial class CheckDistance : BTCondition {
	[Export]
	public float Distance = 1.0f;
	[Export]
	public bool RequireLineOfSight = false;

	public override string _GenerateName() {
		return "Check distance from destination up to " + Distance + " meters" + (RequireLineOfSight ? ", requiring line of sight." : ".");
	}

	public override Status _Tick(double delta) {
		Enemy self = Agent as Enemy;

		Vector3 destination = (Vector3)Blackboard.GetVar("destination");
		Vector3 direction = (destination - self.GlobalPosition);
		float distance = direction.Length();

		if (!RequireLineOfSight) {
			if (distance <= Distance) {
				return Status.Success;
			}
			else {
				return Status.Failure;
			}
		}
		else {
			// Choose a point
			Vector3 start = self.GlobalPosition;
			Vector3 end = start + direction;

			// Check point
			PhysicsDirectSpaceState3D space = self.GetWorld3D().DirectSpaceState;
			PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(start, end, 0b1);
			Dictionary result = space.IntersectRay(parameters);

			if (result.Count == 0) {
				return Status.Success;
			}
			else {
				return Status.Failure;
			}
		}
	}
}
