using Godot;
using Godot.Collections;
using System;
using ActionPlatformer;

[Tool]
public partial class JumpToDestination : BTAction
{
	[Export]
	public float Height = 0.0f;

	public override string _GenerateName() {
		return "Fly to " + Height + " above destination.";
	}

	public override Status _Tick(double delta) {
		Enemy self = Agent as Enemy;
		Vector3 destination = (Vector3)Blackboard.GetVar("destination");

		// If above offset destination...
		if (self.GlobalPosition.Y < destination.Y + Height) {
			// Ascend from the ground
			self.Input.bJumpPress = true;
		}

		return Status.Success;
	}
}
