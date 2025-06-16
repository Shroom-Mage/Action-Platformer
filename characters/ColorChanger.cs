using Godot;
using Godot.Collections;
using System;

namespace ActionPlatformer {
	[GlobalClass, Tool]
	public partial class ColorChanger : Node3D {
        private Dictionary<MeshInstance3D, Gradient> _gradients;

        [ExportToolButton("Update all meshes with color gradients")]
		public Callable Update => Callable.From(UpdateColors);
		[Export]
		public Dictionary<MeshInstance3D, Gradient> Gradients {
			get { return _gradients; }
			set {
				_gradients = value;
				UpdateColors();
			}
		}

		private void UpdateColors() {
			GD.Print(Gradients);
			foreach ((MeshInstance3D mesh, Gradient gradient) in Gradients) {
				GradientTexture1D gradientTexture = new GradientTexture1D();
				gradientTexture.Gradient = gradient;
                mesh.GetActiveMaterial(0).Set("shader_parameter/albedo_gradient", gradientTexture);
            }
		}
	}
}