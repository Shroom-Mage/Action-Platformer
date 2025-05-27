using Godot;
using Godot.Collections;
using System;
using ActionPlatformer;

[Tool]
public partial class PerformDropAttack : BTAction {
	[Export]
	public float Distance = 10.0f;
	public override string _GenerateName() {
		return "Perform a Drop Attack on a player up to " + Distance + " meters below.";
	}

	public override Status _Tick(double delta) {
		Enemy self = Agent as Enemy;

		// Look below
		Vector3 start = self.GlobalPosition;
		Vector3 end = start + new Vector3(0.0f, -Distance, 0.0f);
		PhysicsDirectSpaceState3D space = self.GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(start, end, 0b10);
		Dictionary result = space.IntersectRay(parameters);

		if (result.Count > 0) {
			self.PressCrouch();
			return Status.Success;
		}

		return Status.Failure;
	}
}
