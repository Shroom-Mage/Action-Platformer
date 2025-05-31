using Godot;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Weapon : Node3D {
		[Export]
		public float Power = 1.0f;
		[Export]
		public float Impact = 1.0f;
		[Export]
		public float Speed = 0.0f;
		[Export]
		public float Reach = 1.0f;

		public StandingAttack Standing {
			get; private set;
		}

		public LowAttack Low {
			get; private set;
		}

		public AerialAttack Aerial {
			get; private set;
		}

		public DropAttack Drop {
			get; private set;
		}

		public WhirlAttack Whirl {
			get; private set;
		}

		public override void _Ready() {
			// Get StandingAttack Node
			Standing = GetNode<StandingAttack>("StandingAttack");
			// Get LowAttack Node
			Low = GetNode<LowAttack>("LowAttack");
			// Get AerialAttack Node
			Aerial = GetNode<AerialAttack>("AerialAttack");
			// Get DropAttack Node
			Drop = GetNode<DropAttack>("DropAttack");
			// Get CrouchingSWhirlAttacklash Node
			Whirl = GetNode<WhirlAttack>("WhirlAttack");
		}
	}
}
