using Godot;
using System;

namespace ActionPlatformer {
	public partial class StandingAttack : Attack {
		private AnimatedSprite3D _sprite = null;
		private bool _bIsFollowUp = false;
		private Vector3 _rotationDefault;

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
						_sprite.FlipH = false;
						Rotation = _rotationDefault;
					}
					else {
						_sprite.FlipH = !_sprite.FlipH;
						Rotation = new Vector3(Rotation.X, Rotation.Y, -Rotation.Z);
					}
					_sprite.Frame = 0;
					_sprite.Play();
					HitTargets();
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
	}
}
