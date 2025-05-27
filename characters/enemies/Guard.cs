using Godot;
using System;

namespace ActionPlatformer {
	public partial class Guard : Enemy {
        private Node3D _model = null;

        public override void _Ready() {
            base._Ready();
            _model = GetNode<Node3D>("Pivot/Model");
        }

        protected override void PlayIdle() {
            _model.Call("idle");
        }

        protected override void PlayCrouch() {
            _model.Call("crouch");
        }

        protected override void PlayMove(float speed, float tilt) {
            _model.Call("move", speed / GroundSpeed);
            _model.Set("run_tilt", tilt);
        }

        protected override void PlayJump() {
            _model.Call("jump");
        }

        protected override void PlayFall() {
            _model.Call("fall");
        }

        protected override void PlaySkid() {
            _model.Call("skid");
        }

        protected override void PlaySlide() {
            _model.Call("wall_slide");
        }
    }
}