using Godot;
using System;

namespace ActionPlatformer {
	public partial class AerialAttack : Attack {
		private AnimatedSprite3D _sprite = null;
		private bool _bIsFollowUp = false;
		private Vector3 _rotationDefault;
		private uint _swordHopCount = 0;

		[Export(PropertyHint.Range, "0,100")]
		public float SwordHopSpeed = 5.0f;
		[Export(PropertyHint.Range, "0,1")]
		public float SwordHopCountMult = 0.5f;

		public uint SwordHopCount {
			get { return _swordHopCount; }
		}

		public override void _Ready() {
			base._Ready();
			_sprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
			_rotationDefault = Rotation;
		}

		public override void _PhysicsProcess(double delta) {
            base._PhysicsProcess(delta);
            IsPerforming = _sprite.IsPlaying();
			JustPerformed = false;

			if (IsAttackReady) {
				// Attempt to attack
				if (!IsPerforming) {
					// Perform attack
					if (!_bIsFollowUp) {
						Rotation = _rotationDefault;
					}
					else {
						Rotation = new Vector3(Rotation.X, Rotation.Y, -Rotation.Z);
					}
					_sprite.Frame = 0;
					_sprite.Play();
					HitTargets();
					_swordHopCount++;
					IsAttackReady = false;
					JustPerformed = true;
					_bIsFollowUp = false;
				}
				else {
					// Next attack will follow up
					_bIsFollowUp = true;
				}
			}
		}

		public void TouchGround() {
			_swordHopCount = 0;
		}
	}
}
