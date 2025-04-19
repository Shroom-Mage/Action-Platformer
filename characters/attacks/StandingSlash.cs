using Godot;
using System;

namespace ActionPlatformer {
	public partial class StandingSlash : Attack {
        private AnimatedSprite3D _sprite = null;
        private bool _bIsPerforming = false;
        private bool _bJustPerformed = false;
        private bool _bIsAttackReady = false;
        private bool _bIsFollowUp = false;
        private Vector3 _rotationDefault;

        public AnimatedSprite3D Sprite {
            get { return _sprite; }
        }

        public override void _Ready() {
            base._Ready();
            _sprite = GetNode<AnimatedSprite3D>("Sprite");
            _rotationDefault = _sprite.Rotation;
        }

        public override void _PhysicsProcess(double delta) {
            _bIsPerforming = _sprite.IsPlaying();
            _bJustPerformed = false;

            if (_bIsAttackReady) {
                // Attempt to attack
                if (!_bIsPerforming) {
                    // Perform attack
                    if (!_bIsFollowUp) {
                        _sprite.FlipH = false;
                        _sprite.Rotation = _rotationDefault;
                    }
                    else {
                        _sprite.FlipH = !_sprite.FlipH;
                        _sprite.Rotation = new Vector3(_sprite.Rotation.X, _sprite.Rotation.Y, -_sprite.Rotation.Z);
                    }
                    _sprite.Frame = 0;
                    _sprite.Play();
                    HitTargets(5.0f);
                    _bIsAttackReady = false;
                    _bJustPerformed = true;
                }
                else {
                    // Next attack will follow up
                    _bIsFollowUp = true;
                }
            }
        }

        public override void Perform() {
            _bIsAttackReady = true;
        }

        public override bool IsPerforming() {
            return _bIsPerforming;
        }

        public override bool JustPerformed() {
            return _bJustPerformed;
        }
    }
}
