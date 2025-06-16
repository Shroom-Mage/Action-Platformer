using Godot;
using System;

namespace ActionPlatformer {
	public partial class StandingAttack : Attack {
		private AnimatedSprite3D _sprite0 = null;
		private AnimatedSprite3D _sprite1 = null;
		private bool _bIsFollowUp = false;
		private bool _bPlayedFollowUp = false;

		public override void _Ready() {
			base._Ready();
			_sprite0 = GetNode<AnimatedSprite3D>("Sprite0");
			_sprite1 = GetNode<AnimatedSprite3D>("Sprite1");
		}

		public override void _PhysicsProcess(double delta) {
			JustPerformed = false;

			// Initiate
			if (IsAttackReady) {
				if (IsStartingUp) {
					// Starting up, input should be eaten
					IsAttackReady = false;
				}
				else if (!IsPerforming) {
                    // Attack is not already performing and may begin
                    IsAttackReady = false;
					IsPerforming = true;
					JustPerformed = true;
					AttackTime = 0.0;
					if (_bIsFollowUp && !_bPlayedFollowUp) {
						_sprite1.Frame = 0;
						_sprite1.Play();
						_bPlayedFollowUp = true;
					}
					else {
                        _sprite0.Frame = 0;
						_sprite0.Play();
						_bPlayedFollowUp = false;
					}
					_bIsFollowUp = false;
                }
				else {
					// Queue follow up
					_bIsFollowUp = true;
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
