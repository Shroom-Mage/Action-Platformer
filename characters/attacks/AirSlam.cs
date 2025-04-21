using Godot;
using System;

namespace ActionPlatformer {
	public partial class AirSlam : Attack {
		private GpuParticles3D _particles = null;
		private AnimatedSprite3D _sprite = null;
		private double _startupTimeRemaining = 0.0;
		private bool _bIsDescending = false;

        [Export(PropertyHint.Range, "0,10")]
        public double SlamStartTime = 0.5;
        [Export(PropertyHint.Range, "0,100")]
        public float SlamSpeed = 10.0f;

        public bool IsInStartup {
			get { return _startupTimeRemaining > 0.0; }
		}

		public bool IsDescending {
			get { return _bIsDescending; }
		}

		public override void _Ready() {
			base._Ready();
			_particles = GetNode<GpuParticles3D>("GPUParticles3D");
			_particles.Emitting = false;
			_sprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		}

		public override void _PhysicsProcess(double delta) {
			JustPerformed = false;

			if (IsAttackReady) {
				IsPerforming = true;
				_startupTimeRemaining = SlamStartTime;
                _particles.Emitting = true;
				IsAttackReady = false;
				JustPerformed = true;
			}

			if (_startupTimeRemaining > 0.0) {
				_startupTimeRemaining -= delta;
				if (_startupTimeRemaining <= 0.0) {
					_bIsDescending = true;
				}
			}

			if (_bIsDescending) {
				HitTargets(10.0f);
			}
		}

		public void TouchGround() {
			if (!IsPerforming) {
				return;
			}

			IsPerforming = false;
			_bIsDescending = false;
			_particles.Emitting = false;
            _sprite.Frame = 0;
            _sprite.Play();
        }
	}
}
