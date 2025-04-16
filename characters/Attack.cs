using Godot;
using Godot.Collections;
using System;

namespace ActionPlatformer {
	public partial class Attack : Area3D {
		private AnimatedSprite3D _sprite = null;
		private GpuParticles3D _particles = null;

		private Array<Node3D> _targets;

		public AnimatedSprite3D Sprite {
			get { return _sprite; }
		}

		public GpuParticles3D Particles {
			get { return _particles; }
		}

		public Attack() {
			_targets = new Array<Node3D>();
		}

		public override void _Ready() {
			_sprite = GetNode<AnimatedSprite3D>("Sprite");
			_particles = GetNode<GpuParticles3D>("Particles");
			Monitoring = true;
		}

		public void Play(StringName name = null) {
			_sprite.Play(name);
		}

		public bool IsPlaying() {
			return _sprite.IsPlaying();
		}
		private void OnBodyEntered(Node3D node) {
			Combatant body = node as Combatant;
			if (body == null) {
				return;
			}

			_targets.Add(node);

			GD.Print(_targets);
		}

		private void OnBodyExited(Node3D node) {
			Combatant body = node as Combatant;
			if (body == null) {
				return;
			}

			_targets.Remove(node);

			GD.Print(_targets);
		}

		private void OnReady() {
			GD.Print("Attack Ready");
		}

		public void DamageTargets(float damage) {
			foreach (Combatant body in _targets) {
				body.TakeDamage(damage);
			}
		}
	}
}
