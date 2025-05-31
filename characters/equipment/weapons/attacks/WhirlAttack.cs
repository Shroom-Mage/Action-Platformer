using Godot;
using System;

namespace ActionPlatformer {
	public partial class WhirlAttack : Attack {
        private AnimatedSprite3D _sprite = null;

        public override void _Ready() {
            base._Ready();
            _sprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
        }

        public override void _PhysicsProcess(double delta) {
            base._PhysicsProcess(delta);
            IsPerforming = _sprite.IsPlaying();
            JustPerformed = false;

            if (IsAttackReady && !IsPerforming) {
                // Attempt to attack
                _sprite.Frame = 0;
                _sprite.Play();
                HitTargets();
                IsAttackReady = false;
                JustPerformed = true;
            }
        }
    }
}