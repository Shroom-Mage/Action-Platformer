using Godot;
using System;

namespace ActionPlatformer {
    [GlobalClass]
    public partial class Combatant : CharacterBody3D {

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

        [Export, ExportGroup("Combat")]
        public float Power = 1.0f;
        [Export, ExportGroup("Combat")]
        public float Life = 1.0f;

        public void TakeDamage(float damage) {
            GD.Print(ToString() + " takes " + damage + " damage.");
        }
    }
}
