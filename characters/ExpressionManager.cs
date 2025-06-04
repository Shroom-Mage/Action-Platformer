using Godot;
using Godot.Collections;
using System;

namespace ActionPlatformer {
	public enum ExpressionType {
		Neutral,
		Determined,
		Fierce,
		Hurt
	}

	[GlobalClass, Tool]
	public partial class ExpressionManager : Node3D {

		private MeshInstance3D _mesh = null;
		private int _surface = 1;
		private ExpressionType _expression = ExpressionType.Neutral;

		[Export]
		public MeshInstance3D Mesh {
			get { return _mesh; }
			set {
				_mesh = value;
				Expression = _expression;
			}
		}
		[Export(PropertyHint.Range, "0,1")]
		public int Surface {
			get { return _surface; }
			set {
				_surface = value;
				Expression = _expression;
			}
		}

		[Export]
		public ExpressionType Expression {
			get { return _expression; }
			set {
				_expression = value;

				if (Mesh == null)
					return;

				Vector3 coordinate;
				if (!ExpressionCoordinates.TryGetValue(_expression, out coordinate)) {
					return;
				}

				switch (_expression) {
				case ExpressionType.Determined:
					Mesh.GetActiveMaterial(_surface).Set("uv1_offset", coordinate);
					break;
				case ExpressionType.Fierce:
					Mesh.GetActiveMaterial(_surface).Set("uv1_offset", coordinate);
					break;
				case ExpressionType.Hurt:
					Mesh.GetActiveMaterial(_surface).Set("uv1_offset", coordinate);
					break;
				case ExpressionType.Neutral:
				default:
					Mesh.GetActiveMaterial(_surface).Set("uv1_offset", coordinate);
					break;
				}
			}
		}

		[Export]
		public Dictionary<ExpressionType,Vector3> ExpressionCoordinates = new Dictionary<ExpressionType, Vector3>();
	}
}
