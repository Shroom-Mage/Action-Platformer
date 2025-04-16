using Godot;
using System;

namespace ActionPlatformer {
	[GlobalClass]
	public partial class Player : Combatant {

		private Node3D _model = null;
		private Attack _attack = null;
		private GpuParticles3D _dust = null;
		private GpuParticles3D _slam = null;
		private Node3D _cameraPivot = null;
		private SpringArm3D _cameraArm = null;
		private Camera3D _camera = null;

        [Export(PropertyHint.Range, "0,100")]
        public float SwordHopSpeed = 5.0f;
        [Export(PropertyHint.Range, "0,1")]
        public float SwordHopCountMult = 0.5f;
        [Export(PropertyHint.Range, "0,10")]
        public double SlamTime = 0.5;
        [Export(PropertyHint.Range, "0,100")]
        public float SlamSpeed = 10.0f;
        [Export(PropertyHint.Range, "0,100"), ExportGroup("Camera")]
        public float CameraHorizontalSpeed = 5.0f;
        [Export(PropertyHint.Range, "0,100"), ExportGroup("Camera")]
        public float CameraVerticalSpeed = 5.0f;
        [Export, ExportGroup("Camera")]
        public float CameraUpperLimit = -45.0f;
		[Export, ExportGroup("Camera")]
		public float CameraLowerLimit = 0.0f;

        private Vector3 _forward = Vector3.Forward;
		private Vector3 _right = Vector3.Right;
		private float _tilt = 0.0f;
		private bool _bIsAttackReady = false;
		//private Vector3 _slashRotationDefault;
		private double _slamTime = 0.0f;
		private bool _bIsSlamming = false;
		private uint _swordHopCount = 0;

		public override void _Ready() {
			_model = GetNode<Node3D>("Model");
			_dust = GetNode<GpuParticles3D>("Model/Dust");
			_dust.Emitting = false;
			//_slash = GetNode<AnimatedSprite3D>("Model/Slash");
			//_slashRotationDefault = _slash.Rotation;
			_slam = GetNode<GpuParticles3D>("Model/Slam");
			_attack = GetNode<Attack>("Model/StandingSlash");
			//_slashRotationDefault = _attack.SpriteRotation;
			_cameraPivot = GetNode<Node3D>("CameraPivot");
			_cameraArm = GetNode<SpringArm3D>("CameraPivot/CameraArm");
			_camera = GetNode<Camera3D>("CameraPivot/CameraArm/Camera3D");
		}

		public override void _PhysicsProcess(double delta) {
			Vector2 velocityXZ = new Vector2(Velocity.X, Velocity.Z);
			float velocityY = Velocity.Y;
			bool bIsOnGround = IsOnFloor();
			bool bIsOnWall = IsOnWallOnly();

			// Get boolean input
			bool bJustCrouched = Input.IsActionJustPressed("crouch");
			bool bIsCrouching = Input.IsActionPressed("crouch");
			bool bJustJumped = Input.IsActionJustPressed("jump");
			bool bIsJumping = Input.IsActionPressed("jump");
			bool bJustAttacked = Input.IsActionJustPressed("attack");

			// Attack
            bool bSwordHop = false;
            bool bIsAttacking = _attack.IsPlaying();
			bool bCanAttack = !(bIsOnWall && velocityY < 0.0f) && !_bIsSlamming && _slamTime <= 0.0f;
			if (bJustAttacked && bIsAttacking && bCanAttack) {
				_bIsAttackReady = true;
			}
			if (bIsAttacking) {
				bJustAttacked = false;
			}
            if (bJustAttacked && bCanAttack) {
                // First slash
                _attack.Sprite.Frame = 0;
                //_attack.SpriteRotation = _slashRotationDefault;
                _attack.Sprite.FlipH = false;
                _attack.Play();
                _attack.DamageTargets(5.0f);
                bIsAttacking = true;
				_bIsAttackReady = false;
				bSwordHop = true;
            }
			else if (_bIsAttackReady && !bIsAttacking && !(bIsOnWall && velocityY < 0.0f)) {
                // Followup slash
                _attack.Sprite.Frame = 0;
                //_attack.SpriteRotation = new Vector3(_attack.Rotation.X, _attack.Rotation.Y, -_attack.Rotation.Z);
                _attack.Sprite.FlipH = !_attack.Sprite.FlipH;
                _attack.Play();
				_attack.DamageTargets(5.0f);
				bIsAttacking = true;
				_bIsAttackReady = false;
				bSwordHop = true;
			}

			// Ground reset
			if (bIsOnGround) {
                _swordHopCount = 0;
				_bIsSlamming = false;
				_slam.Emitting = false;
            }

			// Get move input
			Vector2 moveInput = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            if (moveInput.Length() > 1.0f)
				moveInput = moveInput.Normalized();
            float moveSpeed = moveInput.Length();

            // Find movement direction
            Vector3 directionXYZ = _camera.GlobalBasis * new Vector3(moveInput.X, 0, moveInput.Y);
			Vector2 directionXZ = new Vector2(directionXYZ.X, directionXYZ.Z);
			directionXYZ.Y = 0.0f;
			directionXZ = directionXZ.Normalized();
			directionXZ *= bIsAttacking ? 0.0f : moveSpeed;

			// Set target velocity
			Vector2 velocityTarget = directionXZ * RunSpeed;

			bool bIsSkidding = velocityXZ.Normalized().Dot(directionXZ) < -0.5f;
            _dust.Emitting = false;

            // Calculate horizontal velocity
            if (velocityTarget != Vector2.Zero && !bIsSkidding && !(bIsCrouching && bIsOnGround)) {
				// Accelerate
				if (bIsOnGround && !bIsCrouching) {
					// On ground
					velocityXZ = velocityXZ.MoveToward(velocityTarget, GroundAcceleration * (float)delta);
					_forward = new Vector3(velocityXZ.X, 0.0f, velocityXZ.Y).Normalized();
					_right = _forward.Cross(Vector3.Up);
					_tilt = _right.Dot(directionXYZ);
					_model.Call("move", velocityXZ.Length() / RunSpeed);
                    _model.Set("run_tilt", _tilt);
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
					if (!bIsCrouching) {
						// Standing halt
						velocityXZ = velocityXZ.MoveToward(Vector2.Zero, GroundDeceleration * (float)delta);
						_model.Call("idle");
					}
					else {
						// Slide
                        velocityXZ = velocityXZ.MoveToward(Vector2.Zero, SlideDeceleration * (float)delta);
                        _model.Call("crouch");
						if (velocityXZ != Vector2.Zero) {
							_dust.Emitting = true;
						}
                    }
					if (bIsSkidding) {
						_model.Call("skid");
						_dust.Emitting = true;
					}
				}
				else {
					// In air
                    velocityXZ = velocityXZ.MoveToward(Vector2.Zero, AirDeceleration * (float)delta);
                }
			}

			// Calculate vertical velocity
			if (!bIsOnGround) {
				// Sword Hop
				if (bSwordHop) {
					_swordHopCount++;
					velocityY += SwordHopSpeed - (velocityY / _swordHopCount);
				}
				// Slam Down
                else if (_slamTime > 0.0) {
                    _slamTime -= delta;
                    if (_slamTime <= 0.0) {
                        velocityY = -SlamSpeed;
						_bIsSlamming = true;
						_slam.Emitting = true;
                    }
                }
                // Slam Start
                else if (bJustCrouched && bCanAttack) {
					velocityXZ = Vector2.Zero;
                    velocityY = 0.0f;
                    _slamTime = SlamTime;
				}
				// Rising
				else if (velocityY > 0.0f) {
					if (bIsJumping) {
						// Jump
						velocityY += GetGravity().Y * GravityUpMult * (float)delta;
					}
					else {
						// Short hop
						velocityY += GetGravity().Y * GravityUpMult * QuickFallMult * (float)delta;
					}
				}
				// Falling
				else if (velocityY <= 0.0f) {
					if (!bIsOnWall) {
						// Falling in air
						velocityY += GetGravity().Y * GravityDownMult * (float)delta;
						velocityY = Mathf.Clamp(velocityY, -FallSpeed, 0.0f);
						_model.Call("fall");
                    }
					else {
						// Sliding on wall
						velocityY += GetGravity().Y * GravityDownMult * WallSlideMult * (float)delta;
						velocityXZ = Vector2.Zero;
						_model.Call("wall_slide");
						_forward = -GetWallNormal();
						_dust.Emitting = true;
						if (bJustJumped) {
							_forward = -_forward;
							velocityY = JumpSpeed * WallKickJumpMult;
							velocityXZ = new Vector2(_forward.X, _forward.Z) * RunSpeed * WallKickSpeedMult;
							_model.Call("jump");
						}
					}
				}
			}
			else if (bJustJumped && !bIsAttacking) {
				// Jump
				if (bIsSkidding) {
					// Side flip
					velocityY = JumpSpeed * FlipJumpMult;
					velocityXZ = directionXZ * RunSpeed * FlipSpeedMult;
				}
				else if (bIsCrouching) {
					// Back flip
					velocityY = JumpSpeed * FlipJumpMult;
					velocityXZ = new Vector2(_forward.X, _forward.Z) * -RunSpeed * FlipSpeedMult;
				}
				else {
					// Standard jump
					velocityY = Mathf.Lerp(JumpSpeed * StandingJumpMult, JumpSpeed, velocityXZ.Length() / RunSpeed);
				}
				_model.Call("jump");

			}

			Velocity = new Vector3(velocityXZ.X, velocityY, velocityXZ.Y);
			_model.GlobalRotation = new Vector3(_model.GlobalRotation.X,
				Vector3.Back.SignedAngleTo(_forward, Vector3.Up),
				_model.GlobalRotation.Z);

			// Move
			MoveAndSlide();

			// Get look input
            Vector2 lookInput = Input.GetVector("look_left", "look_right", "look_up", "look_down");

			// Rotate camera
            _cameraPivot.RotateObjectLocal(new Vector3(0.0f, 1.0f, 0.0f), -lookInput.X * CameraHorizontalSpeed * (float)delta);
            _cameraArm.RotateObjectLocal(new Vector3(1.0f, 0.0f, 0.0f), -lookInput.Y * CameraVerticalSpeed * (float)delta);
            _cameraArm.Rotation = new Vector3(Mathf.Clamp(_cameraArm.Rotation.X, Mathf.DegToRad(CameraUpperLimit), Mathf.DegToRad(CameraLowerLimit)), _cameraArm.Rotation.Y, _cameraArm.Rotation.Z);
        }

        public override string ToString() {
			return Name;
        }
    }
}
