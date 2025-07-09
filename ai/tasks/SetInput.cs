using Godot;
using System;
using ActionPlatformer;

[Tool]
public partial class SetInput : BTAction {
	public enum ButtonName {
		Jump,
		Crouch,
		Attack,
		Block
	}

	public enum PressType {
		Press,
		Release,
		Hold
	}

	[Export]
	public ButtonName Button;
	[Export]
	public PressType Type;

	public override string _GenerateName() {
		return "" + Type + " the " + Button + " button.";
	}

	public override Status _Tick(double delta) {
		Enemy self = Agent as Enemy;

		switch (Button) {
		case ButtonName.Jump:
			switch (Type) {
			case PressType.Press:
				self.PressJump();
				break;
			case PressType.Release:
				self.ReleaseJump();
				break;
			case PressType.Hold:
				self.HoldJump();
				break;
			default:
				return Status.Failure;
			}
			break;
		case ButtonName.Crouch:
			switch (Type) {
			case PressType.Press:
				self.PressCrouch();
				break;
			case PressType.Release:
				self.ReleaseCrouch();
				break;
			case PressType.Hold:
				self.HoldCrouch();
				break;
			default:
				return Status.Failure;
			}
			break;
		case ButtonName.Attack:
			switch (Type) {
			case PressType.Press:
				self.PressAttack();
				break;
			case PressType.Release:
				self.ReleaseAttack();
				break;
			case PressType.Hold:
				self.HoldAttack();
				break;
			default:
				return Status.Failure;
			}
			break;
		case ButtonName.Block:
			switch (Type) {
			case PressType.Press:
				self.PressBlock();
				break;
			case PressType.Release:
				self.ReleaseBlock();
				break;
			case PressType.Hold:
				self.HoldBlock();
				break;
			default:
				return Status.Failure;
			}
			break;
		default:
			return Status.Failure;
		}

		return Status.Success;
	}
}
