using Godot;
using System;
using ActionPlatformer;

[Tool]
public partial class MoveTowardDestination : BTAction
{
	public override Status _Tick(double delta) {
		Vector3 destination = (Vector3)Blackboard.GetVar("destination");
		Enemy self = Agent as Enemy;

		// Calculate direction
		Vector3 direction = (destination - self.GlobalPosition);
		self.Input.movement = new Vector2(direction.X, direction.Z);

		return Status.Success;
	}
}
