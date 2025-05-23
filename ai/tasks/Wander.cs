using Godot;
using Godot.Collections;
using System;
using ActionPlatformer;


[Tool]
public partial class Wander : BTAction {
	[Export]
	public float RangeMin = 10.0f;
	[Export]
	public float RangeMax = 10.0f;

	public override string _GenerateName() {
		return "Wander between " + RangeMin + " and " + RangeMax + " meters.";
	}

	public override Status _Tick(double delta) {
		Enemy self = Agent as Enemy;

		// Choose a point
		float angle = GD.Randf() * float.Tau;
		float distance = (float)GD.RandRange(RangeMin, RangeMax);
		Vector3 start = self.GlobalPosition;
		Vector3 end = start + new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle)) * distance;

		// Check point
		PhysicsDirectSpaceState3D space = self.GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(start, end, 0b1);
		Dictionary result = space.IntersectRay(parameters);

		if (result.Count > 0) {
			end = (Vector3)result["position"];
		}

		Blackboard.SetVar("destination", end);

		return Status.Success;
	}
}
