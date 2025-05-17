using Godot;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Attack : Node3D {
		private AttackArea _area = null;
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

		public override void _Ready() {
			_area = GetNode<AttackArea>("AttackArea");
		}

		public void Perform() {
			_bIsAttackReady = true;
		}

		public void HitTargets(float damage) {
			foreach (Combatant body in _area.Targets) {
				body.TakeDamage(damage);
			}
		}
	}
}
