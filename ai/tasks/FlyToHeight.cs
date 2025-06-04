using Godot;
using Godot.Collections;
using System;
using ActionPlatformer;

[Tool]
public partial class FlyToHeight : BTAction
{
	[Export]
	public float Height = 0.0f;

	public override string _GenerateName() {
		return "Fly to " + Height + " meters above ground.";
	}

	public override Status _Tick(double delta) {
		Enemy self = Agent as Enemy;

		if (self.IsAttacking || self.IsStunned) {
			return Status.Failure;
		}
		
		Vector3 destination = self.GlobalPosition;
		destination.Y -= Height;

		// Choose a point
		Vector3 start = self.GlobalPosition;
		Vector3 end = start + new Vector3(0.0f, -Height, 0.0f);

		// Check point
		PhysicsDirectSpaceState3D space = self.GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(start, end, 0b1);
		Dictionary result = space.IntersectRay(parameters);

		if (result.Count > 0) {
			destination = (Vector3)result["position"];
		}

		// If above offset destination...
		if (self.GlobalPosition.Y < destination.Y + Height) {
			// Ascend from the ground
			self.PressJump();
		}

		return Status.Success;
	}
}
