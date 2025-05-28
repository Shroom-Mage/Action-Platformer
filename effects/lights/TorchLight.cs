using Godot;
using System;

public partial class TorchLight : OmniLight3D
{
	private float _energyTarget = 8.0f;

	public override void _PhysicsProcess(double delta)
	{
		LightEnergy = Mathf.Clamp(Mathf.Lerp(LightEnergy, _energyTarget, (float)delta), 0.0f, 16.0f);
	}

	public void OnTimerTimeout() {
		_energyTarget = (float)GD.RandRange(-16.0, 32.0);
	}
}
