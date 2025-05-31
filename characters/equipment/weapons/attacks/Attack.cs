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

		private double _time = 0.0;

		[Export]
		public float Damage { get; set; } = 0.0f;
		[Export]
		public float Force { get; set; } = 0.0f;
        [Export]
        public float Startup { get; set; } = 0.0f;
        [Export]
        public float Active { get; set; } = 0.0f;
        [Export]
        public float Recovery { get; set; } = 0.0f;
        [Export]
		public bool IsBlockedHigh { get; set; } = false;
        [Export]
		public bool IsBlockedLow { get; set; } = false;

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

        public override void _PhysicsProcess(double delta) {
			if (IsPerforming) {
				_time += delta;
			}
        }

		public void Perform(Combatant attacker) {
			_bIsAttackReady = true;
			_attacker = attacker;
			_time = 0.0;
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
