using Godot;
using System;

namespace ActionPlatformer {
	public partial class AirSlam : Attack {
		private GpuParticles3D _particles = null;
		private AnimatedSprite3D _sprite = null;
		private double _startupTimeRemaining = 0.0;
		private double _recoveryTimeRemaining = 0.0;
		private bool _bJustStartedDescent = false;
		private bool _bIsDescending = false;
		private bool _bTouchedGround = false;

		[Export(PropertyHint.Range, "0,10")]
		public double SlamStartupTime = 0.5;
		[Export(PropertyHint.Range, "0,10")]
		public double SlamRecoveryTime = 0.5;
		[Export(PropertyHint.Range, "0,100")]
		public float SlamSpeed = 10.0f;

		public bool IsInStartup {
			get { return _startupTimeRemaining > 0.0; }
		}

		public bool IsInRecovery {
			get { return _recoveryTimeRemaining > 0.0; }
		}

		public bool JustStartedDescent {
			get { return _bJustStartedDescent; }
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
			_bJustStartedDescent = false;

			if (IsAttackReady) {
				IsAttackReady = false;
				IsPerforming = true;
				JustPerformed = true;
				_startupTimeRemaining = SlamStartupTime;
				_recoveryTimeRemaining = SlamRecoveryTime;
				_bTouchedGround = false;
				_particles.Emitting = true;
			}

			if (_startupTimeRemaining > 0.0) {
				_startupTimeRemaining -= delta;
				if (_startupTimeRemaining <= 0.0) {
					_bIsDescending = true;
					_bJustStartedDescent = true;
				}
			}

			if (_bIsDescending) {
				HitTargets(10.0f);
			}

			if (_recoveryTimeRemaining > 0.0 && _bTouchedGround) {
				_recoveryTimeRemaining -= delta;
				if (_recoveryTimeRemaining <= 0.0) {
					IsPerforming = false;
				}
			}
		}

		public void TouchGround() {
			if (!IsPerforming || _bTouchedGround) {
				return;
			}

			_bTouchedGround = true;
			_bIsDescending = false;
			_particles.Emitting = false;
			_sprite.Frame = 0;
			_sprite.Play();
		}
	}
}
