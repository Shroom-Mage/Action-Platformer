using Godot;
using System;
using ActionPlatformer;

[Tool]
public partial class MoveToDestination : BTAction {
	[Export]
	public double Duration = 0.0f;

	private double _timePassed = 0.0f;

	public override string _GenerateName() {
		return "Move toward destination" + (Duration != 0.0f ? " for " + Duration + " second(s)." : ".");
	}

	public override void _Enter() {
		_timePassed = 0.0f;
	}

	public override Status _Tick(double delta) {
		Enemy self = Agent as Enemy;

		if (self.IsAttacking || self.IsStunned) {
			return Status.Failure;
		}

		// Calculate direction
		Vector3 destination = (Vector3)Blackboard.GetVar("destination");
		Vector3 direction = (destination - self.GlobalPosition);
		self.PressMovement(new Vector2(direction.X, direction.Z));

		if (_timePassed < Duration) {
			_timePassed += delta;
			return Status.Running;
		}

		return Status.Success;
	}
}
