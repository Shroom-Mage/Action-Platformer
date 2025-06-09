using Godot;
using System;

namespace ActionPlatformer {
	public partial class DropAttack : Attack {
		private GpuParticles3D _particles = null;
		private AnimatedSprite3D _sprite = null;
		private bool _bTouchedGround = false;

		[Export(PropertyHint.Range, "0,100")]
		public float DropSpeed = 15.0f;

		public bool JustStartedDescent { get; private set; }

		public override void _Ready() {
			base._Ready();
			_particles = GetNode<GpuParticles3D>("GPUParticles3D");
			_particles.Emitting = false;
			_sprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		}

		public override void _PhysicsProcess(double delta) {
			JustPerformed = false;
			JustStartedDescent = false;

			// Initiate
			if (IsAttackReady) {
				IsAttackReady = false;
				IsPerforming = true;
				JustPerformed = true;
				AttackTime = 0.0;
				_bTouchedGround = false;
				_particles.Emitting = true;
			}

			// Startup
			if (IsStartingUp) {
				AttackTime += delta;
				if (!IsStartingUp) {
					JustStartedDescent = true;
				}
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

		public void TouchGround() {
			if (!IsPerforming || _bTouchedGround) {
				return;
			}

			_bTouchedGround = true;
			AttackTime = StartupTime + ActiveTime;
			_particles.Emitting = false;
			_sprite.Frame = 0;
			_sprite.Play();
		}
	}
}
