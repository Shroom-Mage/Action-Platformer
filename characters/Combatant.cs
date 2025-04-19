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
        private Node3D _model = null;
        private Attack _attack = null;
        private Camera3D _camera = null;
        private GpuParticles3D _dust = null;
        private GpuParticles3D _slam = null;

        [Export, ExportGroup("Combat")]
        public float Power = 1.0f;
        [Export, ExportGroup("Combat")]
        public float Life = 1.0f;

        [Export(PropertyHint.Range, "0,100"), ExportGroup("Movement")]
        public float RunSpeed = 7.5f;
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

        [Export(PropertyHint.Range, "0,100"), ExportGroup("Attacks")]
        public float SwordHopSpeed = 5.0f;
        [Export(PropertyHint.Range, "0,1"), ExportGroup("Attacks")]
        public float SwordHopCountMult = 0.5f;
        [Export(PropertyHint.Range, "0,10"), ExportGroup("Attacks")]
        public double SlamTime = 0.5;
        [Export(PropertyHint.Range, "0,100"), ExportGroup("Attacks")]
        public float SlamSpeed = 10.0f;

        private Vector3 _forward = Vector3.Forward;
        private Vector3 _right = Vector3.Right;
        private float _tilt = 0.0f;
        private double _slamTime = 0.0f;
        private bool _bIsSlamming = false;
        private uint _swordHopCount = 0;

        public override void _Ready() {
            base._Ready();
			_model = GetNode<Node3D>("Model");
			_dust = GetNode<GpuParticles3D>("Model/Dust");
			_dust.Emitting = false;
			_slam = GetNode<GpuParticles3D>("Model/Slam");
			_attack = GetNode<Attack>("Model/StandingSlash");
            _camera = GetNode<Camera3D>("CameraPivot/CameraArm/Camera3D");
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

            // Attack
            bool bCanAttack = !(bIsOnWall && velocityY < 0.0f) && !_bIsSlamming && _slamTime <= 0.0f;
            if (input.bAttackPress && bCanAttack) {
                _attack.Perform();
            }
            bool bIsAttacking = _attack.IsPerforming();
            bSwordHop = _attack.JustPerformed();

            // Ground reset
            if (bIsOnGround) {
                _swordHopCount = 0;
                _bIsSlamming = false;
                _slam.Emitting = false;
            }

            // Find movement direction
            if (input.movement.Length() > 1.0f)
                input.movement = input.movement.Normalized();
            float moveSpeed = input.movement.Length();
            Vector3 directionXYZ = _camera.GlobalBasis * new Vector3(input.movement.X, 0, input.movement.Y);
            Vector2 directionXZ = new Vector2(directionXYZ.X, directionXYZ.Z);
            directionXYZ.Y = 0.0f;
            directionXZ = directionXZ.Normalized();
            directionXZ *= bIsAttacking ? 0.0f : moveSpeed;

            // Set target velocity
            Vector2 velocityTarget = directionXZ * RunSpeed;

            bool bIsSkidding = velocityXZ.Normalized().Dot(directionXZ) < -0.5f;
            _dust.Emitting = false;

            // Calculate horizontal velocity
            if (velocityTarget != Vector2.Zero && !bIsSkidding && !(input.bCrouchHold && bIsOnGround)) {
                // Accelerate
                if (bIsOnGround && !input.bCrouchHold) {
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
                    if (!input.bCrouchHold) {
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
                else if (input.bCrouchPress && bCanAttack) {
                    velocityXZ = Vector2.Zero;
                    velocityY = 0.0f;
                    _slamTime = SlamTime;
                }
                // Rising
                else if (velocityY > 0.0f) {
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
                        if (input.bJumpPress) {
                            _forward = -_forward;
                            velocityY = JumpSpeed * WallKickJumpMult;
                            velocityXZ = new Vector2(_forward.X, _forward.Z) * RunSpeed * WallKickSpeedMult;
                            _model.Call("jump");
                        }
                    }
                }
            }
            else if (input.bJumpPress && !bIsAttacking) {
                // Jump
                if (bIsSkidding) {
                    // Side flip
                    velocityY = JumpSpeed * FlipJumpMult;
                    velocityXZ = directionXZ * RunSpeed * FlipSpeedMult;
                }
                else if (input.bCrouchHold) {
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

            return MoveAndSlide();
        }
    }
}
