using Godot;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Attack : Node3D {
        private AttackArea _area = null;

        public override void _Ready() {
            _area = GetNode<AttackArea>("AttackArea");
        }

        public virtual void Perform() {
        }

        public virtual bool IsPerforming() {
            return false;
        }

        public virtual bool JustPerformed() {
            return false;
        }

        public virtual void HitTargets(float damage) {
            foreach (Combatant body in _area.Targets) {
                body.TakeDamage(damage);
            }
        }
    }
}
