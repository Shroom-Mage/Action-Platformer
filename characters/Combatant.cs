using Godot;
using System;

namespace ActionPlatformer {
	public struct CombatInput {
		public Vector2 movement;
		public bool bJumpPress;
		public bool bJumpHold;
		public bool bCrouchPress;
		public bool bCrouchHold;
		public bool bAttackPress;
		public bool bAttackHold;
	}

	[GlobalClass]
	public partial class Combatant : CharacterBody3D {
		private Node3D _pivot = null;
		private StandingAttack _standingAttack = null;
		private AerialAttack _aerialAttack = null;
		private LowAttack _lowAttack = null;
		private WhirlAttack _whirlAttack = null;
		private DropAttack _dropAttack = null;
		private GpuParticles3D _dust = null;

		[Export, ExportGroup("Combat")]
		public float Power = 1.0f;
		[Export, ExportGroup("Combat")]
		public float Life = 1.0f;

		[Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
		public float GroundSpeed = 7.5f;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
		public float AirSpeed = 7.5f;
		[Export, ExportGroup("Movement")]
		public bool JumpNeedsGround = true;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
		public float JumpSpeed = 15.0f;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
		public float FallSpeed = 20.0f;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
		public float GroundAcceleration = 15.0f;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
		public float GroundDeceleration = 20.0f;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
		public float AirAcceleration = 2.5f;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
		public float AirDeceleration = 5.0f;
		[Export, ExportGroup("Movement")]
		public bool CanWallSlide = false;
		[Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
		public float SlideDeceleration = 10.0f;
		[Export(PropertyHint.Range, "0,10"), ExportGroup("Movement")]
		public float GravityUpMult = 3.0f;
		[Export(PropertyHint.Range, "0,10"), ExportGroup("Movement")]
		public float GravityDownMult = 2.0f;
		[Export(PropertyHint.Range, "0,10"), ExportGroup("Movement")]
		public float QuickFallMult = 2.0f;
		[Export(PropertyHint.Range, "0,10"), ExportGroup("Movement")]
		public float StandingJumpMult = 0.8f;
		[Export(PropertyHint.Range, "0,10"), ExportGroup("Movement")]
		public float FlipSpeedMult = 0.25f;
		[Export(PropertyHint.Range, "0,10"), ExportGroup("Movement")]
		public float FlipJumpMult = 1.1f;
		[Export(PropertyHint.Range, "0,10"), ExportGroup("Movement")]
		public float WallSlideMult = 0.25f;
		[Export(PropertyHint.Range, "0,10"), ExportGroup("Movement")]
		public float WallKickSpeedMult = 0.8f;
		[Export(PropertyHint.Range, "0,10"), ExportGroup("Movement")]
		public float WallKickJumpMult = 0.8f;

		public Vector3 _forward = Vector3.Forward;
		private Vector3 _right = Vector3.Right;
		private Basis _space;

		public Vector2 Forward {
			get { return new Vector2(_forward.X, _forward.Z); }
			set {
				_forward = new Vector3(value.X, 0.0f, value.Y).Normalized();
				_right = _forward.Cross(Vector3.Up);
			}
		}

		public Vector2 Right {
			get { return new Vector2(_right.X, _right.Z); }
			set {
				_right = new Vector3(value.X, 0.0f, value.Y).Normalized();
				_forward = -_right.Cross(Vector3.Up);
			}
		}

		public Basis Space {
			get { return _space; }
			set { _space = value; }
		}

		public override string ToString() {
			return Name;
		}

		public override void _Ready() {
			Space = GlobalBasis;
			// Get Pivot Node
			_pivot = GetNode<Node3D>("Pivot");
			//Forward = new Vector2(_pivot.Rotation.Z, _pivot.Rotation.X);
			// Get Dust Node if it exists
			if (HasNode("Pivot/Dust")) {
				_dust = GetNode<GpuParticles3D>("Pivot/Dust");
				_dust.Emitting = false;
			}
			else {
				_dust = new GpuParticles3D();
			}
			// Get StandingAttack Node if it exists
			if (HasNode("Pivot/StandingAttack")) {
				_standingAttack = GetNode<StandingAttack>("Pivot/StandingAttack");
			}
			else {
				_standingAttack = new StandingAttack();
			}
			// Get AerialAttack Node if it exists
			if (HasNode("Pivot/AerialAttack")) {
				_aerialAttack = GetNode<AerialAttack>("Pivot/AerialAttack");
			}
			else {
				_aerialAttack = new AerialAttack();
			}
			// Get LowAttack Node if it exists
			if (HasNode("Pivot/LowAttack")) {
				_lowAttack = GetNode<LowAttack>("Pivot/LowAttack");
			}
			else {
				_lowAttack = new LowAttack();
			}
			// Get CrouchingSWhirlAttacklash Node if it exists
			if (HasNode("Pivot/WhirlAttack")) {
				_whirlAttack = GetNode<WhirlAttack>("Pivot/WhirlAttack");
			}
			else {
				_whirlAttack = new WhirlAttack();
			}
			// Get DropAttack Node if it exists
			if (HasNode("Pivot/DropAttack")) {
				_dropAttack = GetNode<DropAttack>("Pivot/DropAttack");
			}
			else {
				_dropAttack = new DropAttack();
			}
		}

		public void TakeDamage(float damage) {
			GD.Print(ToString() + " takes " + damage + " damage.");
		}

		public bool MoveAndAttack(CombatInput input, double delta) {
			Vector2 velocityXZ = new Vector2(Velocity.X, Velocity.Z);
			float velocityY = Velocity.Y;
			bool bIsOnGround = IsOnFloor();
			bool bIsOnWall = IsOnWallOnly();
			bool bSwordHop = false;
			_dust.Emitting = false;

			// Ground reset
			if (bIsOnGround) {
				_aerialAttack.TouchGround();
				_dropAttack.TouchGround();
			}

			// Find movement direction
			if (input.movement.Length() > 1.0f)
				input.movement = input.movement.Normalized();
			float moveSpeed = input.movement.Length();
			Vector3 directionXYZ = _space * new Vector3(input.movement.X, 0, input.movement.Y);
			Vector2 directionXZ = new Vector2(directionXYZ.X, directionXYZ.Z);
			directionXYZ.Y = 0.0f;
			directionXZ = directionXZ.Normalized();

			bool bIsSkidding = velocityXZ.Normalized().Dot(directionXZ) < -0.5f;

			// Check attack validity
			bool bCanAttack = !(bIsOnWall && velocityY < 0.0f) && !_dropAttack.IsPerforming;
			_standingAttack.CanPerform = bCanAttack && bIsOnGround && !input.bCrouchHold;
			_lowAttack.CanPerform = bCanAttack && bIsOnGround && input.bCrouchHold;
			_aerialAttack.CanPerform = bCanAttack && !bIsOnGround && !input.bCrouchHold;
			_dropAttack.CanPerform = bCanAttack && !bIsOnGround;
			_whirlAttack.CanPerform = bCanAttack && bIsOnGround && bIsSkidding;

			// Attack
			if (input.bAttackPress && _lowAttack.CanPerform) {
				_lowAttack.Perform();
			}
			else if (input.bAttackPress && _whirlAttack.CanPerform) {
				_whirlAttack.Perform();
			}
			else if (input.bAttackPress && _aerialAttack.CanPerform) {
				_aerialAttack.Perform();
			}
			else if (input.bAttackPress && _standingAttack.CanPerform) {
				_standingAttack.Perform();
			}
			else if (input.bCrouchPress && _dropAttack.CanPerform) {
				_dropAttack.Perform();
			}
			bool bIsAttacking = _standingAttack.IsPerforming || _aerialAttack.IsPerforming || _lowAttack.IsPerforming || _whirlAttack.IsPerforming || _dropAttack.IsPerforming;
			bSwordHop = _aerialAttack.JustPerformed;
			directionXZ *= bIsAttacking ? 0.0f : moveSpeed;

			// Set target velocity
			Vector2 velocityTarget = directionXZ * (bIsOnGround ? GroundSpeed : AirSpeed);

			// Calculate horizontal velocity
			if (velocityTarget != Vector2.Zero && !bIsSkidding && !(input.bCrouchHold && bIsOnGround)) {
				// Accelerate
				if (bIsOnGround && !input.bCrouchHold) {
					// On ground
					velocityXZ = velocityXZ.MoveToward(velocityTarget, GroundAcceleration * (float)delta);
					Forward = velocityXZ;
					PlayMove(velocityXZ.Length(), _right.Dot(directionXYZ));
				}
				else {
					// In air
					velocityXZ = velocityXZ.MoveToward(velocityTarget, AirAcceleration * (float)delta);
				}
			}
			else {
				// Decelerate
				if (bIsOnGround) {
					// On ground
					if (!input.bCrouchHold) {
						// Standing halt
						velocityXZ = velocityXZ.MoveToward(Vector2.Zero, GroundDeceleration * (float)delta);
						PlayIdle();
					}
					else {
						// Slide
						velocityXZ = velocityXZ.MoveToward(Vector2.Zero, SlideDeceleration * (float)delta);
						PlayCrouch();
						if (velocityXZ != Vector2.Zero) {
							_dust.Emitting = true;
						}
					}
					if (bIsSkidding) {
						PlaySkid();
						_dust.Emitting = true;
					}
				}
				else {
					// In air
					velocityXZ = velocityXZ.MoveToward(Vector2.Zero, AirDeceleration * (float)delta);
				}
			}

			// Calculate vertical velocity
			if (input.bJumpPress && !bIsAttacking && (bIsOnGround || !JumpNeedsGround) && velocityY <= 0.0f) {
				// Jump
				if (bIsSkidding) {
					// Side flip
					velocityY = JumpSpeed * FlipJumpMult;
					velocityXZ = directionXZ * AirSpeed * FlipSpeedMult;
				}
				else if (input.bCrouchHold) {
					// Back flip
					velocityY = JumpSpeed * FlipJumpMult;
					velocityXZ = Forward * -AirSpeed * FlipSpeedMult;
				}
				else {
					// Standard jump
					velocityY = Mathf.Lerp(JumpSpeed * StandingJumpMult, JumpSpeed, velocityXZ.Length() / GroundSpeed);
				}
				PlayJump();
			}
			else if (!bIsOnGround) {
				// Sword Hop
				if (bSwordHop) {
					velocityY += _aerialAttack.SwordHopSpeed - (velocityY / _aerialAttack.SwordHopCount);
				}
				// Slam Startup
				else if (_dropAttack.IsInStartup) {
					velocityXZ = Vector2.Zero;
					velocityY = 0.0f;
				}
				// Slam Start
				else if (_dropAttack.JustStartedDescent) {
					velocityXZ = Vector2.Zero;
					velocityY = -_dropAttack.DropSpeed;
				}
				// Rising
				else if (velocityY >= 0.0f) {
					if (input.bJumpHold) {
						// Jump
						velocityY += GetGravity().Y * GravityUpMult * (float)delta;
					}
					else {
						// Short hop
						velocityY += GetGravity().Y * GravityUpMult * QuickFallMult * (float)delta;
					}
				}
				// Falling
				else if (velocityY < 0.0f) {
					if (!bIsOnWall || !CanWallSlide || _dropAttack.IsPerforming) {
						// Falling in air
						velocityY += GetGravity().Y * GravityDownMult * (float)delta;
						velocityY = Mathf.Clamp(velocityY, -FallSpeed, 0.0f);
						PlayFall();
					}
					else if (CanWallSlide) {
						// Sliding on wall
						velocityY += GetGravity().Y * GravityDownMult * WallSlideMult * (float)delta;
						velocityXZ = Vector2.Zero;
						PlaySlide();
						Vector3 wallNormal = GetWallNormal();
						Forward = -new Vector2(wallNormal.X, wallNormal.Z);
						_dust.Emitting = true;
						if (input.bJumpPress) {
							Forward = -Forward;
							velocityY = JumpSpeed * WallKickJumpMult;
							velocityXZ = Forward * GroundSpeed * WallKickSpeedMult;
							PlayJump();
						}
					}
				}
			}

			Velocity = new Vector3(velocityXZ.X, velocityY, velocityXZ.Y);
			_pivot.GlobalRotation = new Vector3(_pivot.GlobalRotation.X,
				Vector3.Back.SignedAngleTo(_forward, Vector3.Up),
				_pivot.GlobalRotation.Z);

			return MoveAndSlide();
		}

		protected virtual void PlayIdle() {
			
		}

		protected virtual void PlayCrouch() {
			
		}

		protected virtual void PlayMove(float speed, float tilt) {
			
		}

		protected virtual void PlayJump() {
			
		}

		protected virtual void PlayFall() {
			
		}

		protected virtual void PlaySkid() {
			
		}

		protected virtual void PlaySlide() {
			
		}
	}
}
