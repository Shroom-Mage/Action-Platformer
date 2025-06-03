using Godot;
using System;

namespace ActionPlatformer {
	public partial class WhirlAttack : Attack {
		private AnimatedSprite3D _sprite = null;

		public override void _Ready() {
			base._Ready();
			_sprite = GetNode<AnimatedSprite3D>("Sprite");
		}

		public override void _PhysicsProcess(double delta) {
            JustPerformed = false;

            // Initiate
            if (IsAttackReady) {
                if (IsStartingUp) {
                    IsAttackReady = false;
                }
                else if (!IsPerforming) {
                    IsAttackReady = false;
                    IsPerforming = true;
                    JustPerformed = true;
                    AttackTime = 0.0;
                    _sprite.Frame = 0;
                    _sprite.Play();
                }
            }

            // Startup
            if (IsStartingUp) {
                AttackTime += delta;
            }

            // Active
            if (IsActive) {
                AttackTime += delta;
                Monitoring = true;
                HitTargets();
            }

            // Recovery
            if (IsRecovering) {
                AttackTime += delta;
                Monitoring = false;
            }

            // Finish
            if (IsFinished) {
                AttackTime = -1.0;
                Monitoring = false;
                IsPerforming = false;
            }
        }
	}
}