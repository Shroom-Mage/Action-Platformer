using Godot;
using Godot.Collections;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Attack : Area3D {
		private Array<Combatant> _targets;
		private Combatant _attacker;

		private bool _bCanPerform = false;
		private bool _bIsAttackReady = false;
		private bool _bIsPerforming = false;
		private bool _bJustPerformed = false;

		private float _damage = 0.0f;
		private float _force = 0.0f;
		private bool _bIsBlockedHigh = false;
		private bool _bIsBlockedLow = false;

		[Export]
		public float Damage {
			get { return _damage; }
			set { _damage = value; }
		}
		[Export]
		public float Force {
			get { return _force; }
			set { _force = value; }
		}
		[Export]
		public bool IsBlockedHigh {
			get { return _bIsBlockedHigh; }
			set { _bIsBlockedHigh = value; }
		}
		[Export]
		public bool IsBlockedLow {
			get { return _bIsBlockedLow; }
			set { _bIsBlockedLow = value; }
		}

		public Array<Combatant> Targets {
			get { return _targets; }
		}

		public bool CanPerform {
			get { return _bCanPerform; }
			set {
				_bCanPerform = value;
				if (IsAttackReady) {
					_bIsAttackReady = value;
				}
			}
		}

		public bool IsAttackReady {
			get { return _bIsAttackReady; }
			protected set { _bIsAttackReady = value; }
		}

		public bool IsPerforming {
			get { return _bIsPerforming; }
			protected set { _bIsPerforming = value; }
		}

		public bool JustPerformed {
			get { return _bJustPerformed; }
			protected set { _bJustPerformed = value; }
		}

		public Attack() {
			_targets = new Array<Combatant>();
		}

		public override void _Ready() {
			Monitoring = true;
		}

		public void Perform(Combatant attacker) {
			_bIsAttackReady = true;
			_attacker = attacker;
		}

		public void HitTargets(float damage) {
			foreach (Combatant body in Targets) {
				body.TakeDamage(_attacker, this);
			}
		}

		private void OnBodyEntered(Node3D node) {
			Combatant target = node as Combatant;
			if (target == null) {
				return;
			}

			_targets.Add(target);
		}

		private void OnBodyExited(Node3D node) {
			Combatant target = node as Combatant;
			if (target == null) {
				return;
			}

			_targets.Remove(target);
		}
	}
}
