using Godot;
using ActionPlatformer;
using System;

[Tool]
public partial class SetExpression : BTAction {
	[Export]
	public ExpressionType Expression;

	public override string _GenerateName() {
		return "Set Expression to " + Expression + ".";
	}

	public override Status _Tick(double delta) {
		Enemy self = Agent as Enemy;

		self.Model.SetExpression(Expression);

		return Status.Success;
	}
}
