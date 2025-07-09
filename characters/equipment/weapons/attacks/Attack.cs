using Godot;
using Godot.Collections;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Attack : Area3D {
		private Combatant _attacker;

		private bool _bCanPerform = false;

		[Export]
		public bool Enabled = true;
		[Export]
		public float Damage { get; set; } = 0.0f;
		[Export]
		public float Force { get; set; } = 0.0f;
		[Export(PropertyHint.Range, "0.0,10")]
		public double StartupTime { get; set; } = 0.0f;
		[Export(PropertyHint.Range, "0.017,10")]
		public double ActiveTime { get; set; } = 0.017f;
		[Export(PropertyHint.Range, "0.0,10")]
		public double RecoveryTime { get; set; } = 0.0f;
		[Export]
		public bool IsBlockedHigh { get; set; } = false;
		[Export]
		public bool IsBlockedLow { get; set; } = false;

		public Array<Combatant> Targets { get; private set; }
		public bool IsAttackReady { get; protected set; } = false;
		public bool IsPerforming { get; protected set; } = false;
		public bool JustPerformed { get; protected set; } = false;
		public double AttackTime { get; protected set; } = -1.0;

		public bool CanPerform {
			get { return Enabled && _bCanPerform; }
			set {
				_bCanPerform = value;
				if (!value) {
					IsAttackReady = false;
				}
			}
		}

		public bool IsStartingUp {
			get { return AttackTime >= 0.0 &&
					AttackTime < StartupTime; }
		}

		public bool IsActive {
			get { return AttackTime >= StartupTime &&
					AttackTime < StartupTime + ActiveTime; }
		}

		public bool IsRecovering {
			get { return AttackTime >= StartupTime + ActiveTime &&
					AttackTime < StartupTime + ActiveTime + RecoveryTime; }
		}

		public bool IsFinished {
			get { return AttackTime >= StartupTime + ActiveTime + RecoveryTime; }
		}

		public Attack() {
			Targets = new Array<Combatant>();
		}

		public void Perform(Combatant attacker) {
			IsAttackReady = true;
			_attacker = attacker;
		}

		public void HitTargets() {
			foreach (Combatant body in Targets) {
				body.TakeDamage(_attacker, this);
			}
		}

		private void OnBodyEntered(Node3D node) {
			Combatant target = node as Combatant;
			if (target == null) {
				return;
			}

			Targets.Add(target);
		}

		private void OnBodyExited(Node3D node) {
			Combatant target = node as Combatant;
			if (target == null) {
				return;
			}

			Targets.Remove(target);
		}
	}
}
