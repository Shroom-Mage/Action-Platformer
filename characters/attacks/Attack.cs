using Godot;
using Godot.Collections;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Attack : Area3D {
		private bool _bCanPerform = false;
		private bool _bIsAttackReady = false;
		private bool _bIsPerforming = false;
		private bool _bJustPerformed = false;

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
		private Array<Combatant> _targets;

		public Array<Combatant> Targets {
			get { return _targets; }
		}

		public Attack() {
			_targets = new Array<Combatant>();
		}

		public override void _Ready() {
			Monitoring = true;
		}

		public void Perform() {
			_bIsAttackReady = true;
		}

		public void HitTargets(float damage) {
			foreach (Combatant body in Targets) {
				body.TakeDamage(damage);
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
