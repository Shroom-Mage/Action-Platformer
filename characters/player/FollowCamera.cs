using Godot;
using System;

namespace ActionPlatformer {
	[Tool]
	public partial class FollowCamera : Node3D {
		private SpringArm3D _arm = null;
		private Camera3D _camera = null;

		private bool _bInterior = false;
		private float _upperLimit = 45.0f;
		private float _lowerLimit = 15.0f;
		private float _rotationLimit = 360.0f;

		private float _exteriorHeight = 1.5f;
		private float _exteriorDistance = 15.0f;
		private float _exteriorFOV = 30.0f;
		private float _exteriorUpperLimit = 45.0f;
		private float _exteriorLowerLimit = 15.0f;
		private float _exteriorRotationLimit = 360.0f;
		private float _interiorHeight = 2.0f;
		private float _interiorDistance = 30.0f;
		private float _interiorFOV = 15.0f;
		private float _interiorUpperLimit = 15.0f;
		private float _interiorLowerLimit = 0.0f;
		private float _interiorRotationLimit = 30.0f;

		[Export(PropertyHint.Range, "0,100")]
		public float HorizontalSpeed = 5.0f;
		[Export(PropertyHint.Range, "0,100"),]
		public float VerticalSpeed = 5.0f;
		[Export]
		public bool Interior {
			get { return _bInterior; }
			set {
				if (value != _bInterior) {
					_bInterior = value;
					if (_bInterior) {
						Position = new Vector3(0.0f, _interiorHeight, 0.0f);
						_arm.SpringLength = _interiorDistance;
						_camera.Fov = _interiorFOV;
						_upperLimit = _interiorUpperLimit;
						_lowerLimit = _interiorLowerLimit;
						_rotationLimit = _interiorRotationLimit;
					}
					else {
						Position = new Vector3(0.0f, _exteriorHeight, 0.0f);
						_arm.SpringLength = _exteriorDistance;
						_camera.Fov = _exteriorFOV;
						_upperLimit = _exteriorUpperLimit;
						_lowerLimit = _exteriorLowerLimit;
						_rotationLimit = _exteriorRotationLimit;
					}
				}
			}
		}
		// Exterior
		[Export, ExportGroup("Exterior")]
		public float HeightExterior {
			get { return _exteriorHeight; }
			set {
				_exteriorHeight = value;
				if (!Interior) {
					Position = new Vector3(0.0f, _exteriorHeight, 0.0f);
				}
			}
		}
		[Export, ExportGroup("Exterior")]
		public float DistanceExterior {
			get { return _exteriorDistance; }
			set {
				_exteriorDistance = value;
				if (!Interior && _arm != null) {
					_arm.SpringLength = _exteriorDistance;
				}
			}
		}
		[Export(PropertyHint.Range, "1.0f,179.0f"), ExportGroup("Exterior")]
		public float FieldOfViewExterior {
			get { return _exteriorFOV; }
			set {
				_exteriorFOV = value;
				if (!Interior && _camera != null) {
					_camera.Fov = _exteriorFOV;
				}
			}
		}
		[Export(PropertyHint.Range, "0.0f, 90.0f"), ExportGroup("Exterior")]
		public float UpperLimitExterior {
			get { return _exteriorUpperLimit; }
			set {
				_exteriorUpperLimit = value;
				if (!Interior) {
					_upperLimit = _exteriorUpperLimit;
				}
			}
		}
		[Export(PropertyHint.Range, "0.0f, 90.0f"), ExportGroup("Exterior")]
		public float LowerLimitExterior {
			get { return _exteriorLowerLimit; }
			set {
				_exteriorLowerLimit = value;
				if (!Interior) {
					_lowerLimit = _exteriorLowerLimit;
				}
			}
		}
		[Export(PropertyHint.Range, "0.0f, 360.0f"), ExportGroup("Exterior")]
		public float RotationLimitExterior {
			get { return _exteriorRotationLimit; }
			set {
				_exteriorRotationLimit = value;
				if (!Interior) {
					_rotationLimit = _exteriorRotationLimit;
				}
			}
		}
		// Interior
		[Export, ExportGroup("Interior")]
		public float HeightInterior {
			get { return _interiorHeight; }
			set {
				_interiorHeight = value;
				if (Interior) {
					Position = new Vector3(0.0f, _interiorHeight, 0.0f);
				}
			}
		}
		[Export, ExportGroup("Interior")]
		public float DistanceInterior {
			get { return _interiorDistance; }
			set {
				_interiorDistance = value;
				if (Interior && _arm != null) {
					_arm.SpringLength = _interiorDistance;
				}
			}
		}
		[Export(PropertyHint.Range, "1.0f,179.0f"), ExportGroup("Interior")]
		public float FieldOfViewInterior {
			get { return _interiorFOV; }
			set {
				_interiorFOV = value;
				if (Interior) {
					_camera.Fov = _interiorFOV;
				}
			}
		}
		[Export(PropertyHint.Range, "0.0f, 90.0f"), ExportGroup("Interior")]
		public float UpperLimitInterior {
			get { return _interiorUpperLimit; }
			set {
				_interiorUpperLimit = value;
				if (Interior) {
					_upperLimit = _interiorUpperLimit;
				}
			}
		}
		[Export(PropertyHint.Range, "0.0f, 90.0f"), ExportGroup("Interior")]
		public float LowerLimitInterior {
			get { return _interiorLowerLimit; }
			set {
				_interiorLowerLimit = value;
				if (Interior) {
					_lowerLimit = _interiorLowerLimit;
				}
			}
		}
		[Export(PropertyHint.Range, "0.0f, 360.0f"), ExportGroup("Interior")]
		public float RotationLimitInterior {
			get { return _interiorRotationLimit; }
			set {
				_interiorRotationLimit = value;
				if (Interior) {
					_rotationLimit = _interiorRotationLimit;
				}
			}
		}

		public override void _EnterTree() {
			_arm = GetNode<SpringArm3D>("%Arm");
			_camera = GetNode<Camera3D>("%Camera");
		}

		public override void _Ready() {
			base._Ready();
			Interior = Interior;
		}

		public void Rotate(Vector2 input, double delta) {
			// Rotate camera
			RotateObjectLocal(new Vector3(0.0f, 1.0f, 0.0f), -input.X * HorizontalSpeed * (float)delta);
			_arm.RotateObjectLocal(new Vector3(1.0f, 0.0f, 0.0f), -input.Y * VerticalSpeed * (float)delta);
			float cameraY = Mathf.Clamp(Rotation.Y, Mathf.DegToRad(-_rotationLimit), Mathf.DegToRad(_rotationLimit));
			float cameraX = Mathf.Clamp(_arm.Rotation.X, Mathf.DegToRad(-_upperLimit), Mathf.DegToRad(-_lowerLimit));
			Rotation = new Vector3(0.0f, cameraY, 0.0f);
			_arm.Rotation = new Vector3(cameraX, 0.0f, 0.0f);
		}

		public Basis GetCameraBasis() {
			return _camera.GlobalBasis;
		}
	}
}