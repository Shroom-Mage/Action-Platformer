using Godot;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Enemy : Combatant {
		private Vision _vision = null;

		private CombatInput _input;
		private bool _bHoldingMovement = false;
		private bool _bHoldingJump = false;
		private bool _bHoldingCrouch = false;
		private bool _bHoldingAttack = false;
        private bool _bHoldingBlock = false;

        public override void _Ready() {
			base._Ready();
			_vision = GetNode<Vision>("Pivot/Vision");
		}

		public override void _PhysicsProcess(double delta) {
			MoveAndAttack(_input, delta);
			if (!_bHoldingMovement)
				_input.movement = Vector2.Zero;
			_input.bJumpPress = false;
			_input.bJumpHold = _bHoldingJump;
			_input.bCrouchPress = false;
			_input.bCrouchHold = _bHoldingCrouch;
			_input.bAttackPress = false;
			_input.bAttackHold = _bHoldingAttack;
            _input.bBlockPress = false;
            _input.bBlockHold = _bHoldingBlock;
        }

		public void PressMovement(Vector2 direction) {
			_input.movement = direction;
		}
		public void HoldMovement(Vector2 direction) {
			_input.movement = direction;
			_bHoldingMovement = true;
		}
		public void ReleaseMovement() {
			_input.movement = Vector2.Zero;
			_bHoldingMovement = false;
		}

		public void PressJump() {
			_input.bJumpPress = true;
			_input.bJumpHold = true;
		}
		public void HoldJump() {
			if (!_bHoldingJump)
				_input.bJumpPress = true;
			_input.bJumpHold = true;
			_bHoldingJump = true;
		}
		public void ReleaseJump() {
			_input.bJumpHold = false;
			_bHoldingJump = false;
		}

		public void PressCrouch() {
			_input.bCrouchPress = true;
			_input.bCrouchHold = true;
		}
		public void HoldCrouch() {
			if (!_bHoldingCrouch)
				_input.bCrouchPress = true;
			_input.bCrouchHold = true;
			_bHoldingCrouch = true;
		}
		public void ReleaseCrouch() {
			_input.bCrouchHold = false;
			_bHoldingCrouch = false;
		}

		public void PressAttack() {
			_input.bAttackPress = true;
			_input.bAttackHold = true;
		}
		public void HoldAttack() {
			if (!_bHoldingAttack)
				_input.bAttackPress = true;
			_input.bAttackHold = true;
			_bHoldingAttack = true;
		}
		public void ReleaseAttack() {
			_input.bAttackHold = false;
			_bHoldingAttack = false;
        }


        public void PressBlock() {
            _input.bBlockPress = true;
            _input.bBlockHold = true;
        }
        public void HoldBlock() {
            if (!_bHoldingBlock)
                _input.bBlockPress = true;
            _input.bBlockHold = true;
            _bHoldingBlock = true;
        }
        public void ReleaseBlock() {
            _input.bBlockHold = false;
            _bHoldingBlock = false;
        }
    }
}
