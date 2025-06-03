using Godot;
using System;
using static Godot.TextServer;

namespace ActionPlatformer {
	public struct CombatInput {
		public Vector2 movement;
		public bool bJumpPress;
		public bool bJumpHold;
		public bool bCrouchPress;
		public bool bCrouchHold;
		public bool bAttackPress;
		public bool bAttackHold;
		public bool bBlockPress;
		public bool bBlockHold;
	}

	[GlobalClass]
	public partial class Combatant : CharacterBody3D {
		private Node3D _pivot = null;
		private Weapon _weapon = null;
		private GpuParticles3D _dust = null;

		[Export, ExportGroup("Combat")]
		public float Life = 1.0f;
		[Export, ExportGroup("Combat")]
		public float Defense = 1.0f;
		[Export, ExportGroup("Combat")]
		public float Poise = 1.0f;
		[Export, ExportGroup("Combat")]
		public PackedScene DeathBurst = null;

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

		private bool _bIsBlocking = false;
		private bool _bIsStunned = false;
		private bool _bJustStunned = false;

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
			// Get Weapon Node
			_weapon = GetNode<Weapon>("Pivot/Weapon");
			// Get Dust Node if it exists
			if (HasNode("Pivot/Dust")) {
				_dust = GetNode<GpuParticles3D>("Pivot/Dust");
				_dust.Emitting = false;
			}
			else {
				_dust = new GpuParticles3D();
			}
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
				_weapon.Aerial.TouchGround();
				_weapon.Drop.TouchGround();
			}

			// Find movement direction
			if (input.movement.Length() > 1.0f)
				input.movement = input.movement.Normalized();
			float moveSpeed = input.movement.Length();
			Vector3 directionXYZ = _space * new Vector3(input.movement.X, 0, input.movement.Y);
			Vector2 directionXZ = new Vector2(directionXYZ.X, directionXYZ.Z);
			directionXYZ.Y = 0.0f;
			directionXZ = directionXZ.Normalized();

			// Sharp turn
			bool bIsSkidding = velocityXZ.Normalized().Dot(directionXZ) < -0.5f;

			// End stun on ground
			if (_bIsStunned && bIsOnGround && !_bJustStunned) {
				_bIsStunned = false;
				if (Life <= 0) {
					GpuParticles3D burst = (GpuParticles3D)DeathBurst.Instantiate();
					GetParent().AddChild(burst);
					burst.GlobalPosition = GlobalPosition;
					burst.Restart();
					QueueFree();
				}
			}
			if (_bJustStunned) _bJustStunned = false;

			// Block
			_bIsBlocking = bIsOnGround && input.bBlockHold;

			// Check attack validity
			bool bCanAttack = !_bIsBlocking &&
				!_bIsStunned &&
				!(CanWallSlide && bIsOnWall &&
				velocityY < 0.0f) && !_weapon.Drop.IsPerforming;
			_weapon.Standing.CanPerform = bCanAttack && bIsOnGround && !input.bCrouchHold;
			_weapon.Low.CanPerform = bCanAttack && bIsOnGround && input.bCrouchHold;
			_weapon.Aerial.CanPerform = bCanAttack && !bIsOnGround && !input.bCrouchHold;
			_weapon.Drop.CanPerform = bCanAttack && !bIsOnGround;
			_weapon.Whirl.CanPerform = bCanAttack && bIsOnGround && bIsSkidding;

			// Attack
			if (input.bAttackPress && _weapon.Low.CanPerform) {
				_weapon.Low.Perform(this);
			}
			else if (input.bAttackPress && _weapon.Whirl.CanPerform) {
				_weapon.Whirl.Perform(this);
			}
			else if (input.bAttackPress && _weapon.Aerial.CanPerform) {
				_weapon.Aerial.Perform(this);
			}
			else if (input.bAttackPress && _weapon.Standing.CanPerform) {
				_weapon.Standing.Perform(this);
			}
			else if (input.bCrouchPress && _weapon.Drop.CanPerform) {
				_weapon.Drop.Perform(this);
			}
			bool bIsAttacking = _weapon.Standing.IsPerforming ||
				_weapon.Aerial.IsPerforming ||
				_weapon.Low.IsPerforming ||
				_weapon.Whirl.IsPerforming ||
				_weapon.Drop.IsPerforming;
			bSwordHop = _weapon.Aerial.JustPerformed;
			directionXZ *= (bIsAttacking || _bIsStunned) ? 0.0f : moveSpeed;

			// Set target velocity
			Vector2 velocityTarget = directionXZ * (bIsOnGround ? GroundSpeed : AirSpeed);

			// Calculate horizontal velocity
			if (velocityTarget != Vector2.Zero && !bIsSkidding && !(input.bCrouchHold && bIsOnGround)) {
				// Accelerate
				if (bIsOnGround && !input.bCrouchHold) {
					// On ground
					velocityXZ = velocityXZ.MoveToward(velocityTarget, GroundAcceleration * (float)delta);
					if (!_bIsBlocking)
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
			if (input.bJumpPress && !bIsAttacking && !_bIsStunned && (bIsOnGround || !JumpNeedsGround) && velocityY <= 0.0f) {
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
					velocityY += _weapon.Aerial.SwordHopSpeed - (velocityY / _weapon.Aerial.SwordHopCount);
				}
				// Drop Startup
				else if (_weapon.Drop.IsStartingUp) {
					velocityXZ = Vector2.Zero;
					velocityY = 0.0f;
				}
				// Drop Active
				else if (_weapon.Drop.JustStartedDescent) {
					velocityXZ = Vector2.Zero;
					velocityY = -_weapon.Drop.DropSpeed;
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
					if (!bIsOnWall || !CanWallSlide || _weapon.Drop.IsPerforming) {
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

		public void TakeDamage(Combatant attacker, Attack attack) {
			// Check both attack position and attacker facing
			if (_bIsBlocking && _forward.Dot(attack.GlobalPosition - GlobalPosition) > 0.0f) {
				// Attack was blocked
				// ForceTaken = AttackForce * WeaponImpact / ArmorPoise
				float forceTaken = attack.Force * attacker._weapon.Impact / Poise;
				Velocity = new Vector3(attacker.Forward.X, 0.0f, attacker.Forward.Y).Normalized() * forceTaken;
				GD.Print(ToString() + " blocks the attack from " + attacker + ".");
			}
			else {
				// Attack was not blocked
				// DamageTaken = AttackDamage * WeaponPower / ArmorDefense
				float damageTaken = attack.Damage * attacker._weapon.Power / Defense;
				if (!_bIsStunned) {
					Life -= damageTaken;
					GD.Print(ToString() + " takes " + damageTaken + " damage from " + attacker + ".");
				}
				// ForceTaken = AttackForce * WeaponImpact / ArmorPoise
				float forceTaken = attack.Force * attacker._weapon.Impact / Poise;
				Velocity = new Vector3(attacker.Forward.X, 1.0f, attacker.Forward.Y).Normalized() * forceTaken;
				_bIsStunned = true;
				_bJustStunned = true;
			}
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

		protected virtual void PlayBlock() {

		}
	}
}
