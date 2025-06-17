// NOTE: Shader automatically converted from Godot Engine 4.4.1.limboai+v1.4.1.mono's StandardMaterial3D.

shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_disabled, diffuse_burley, specular_schlick_ggx;

uniform vec4 albedo_floor : source_color = vec4(1.0f, 1.0f, 1.0f, 1.0f);
uniform vec4 albedo_wall : source_color = vec4(1.0f, 1.0f, 1.0f, 1.0f);
uniform vec4 albedo_ceiling : source_color = vec4(1.0f, 1.0f, 1.0f, 1.0f);
uniform sampler2D texture_albedo_floor : source_color, filter_linear_mipmap, repeat_enable;
uniform sampler2D texture_albedo_wall : source_color, filter_linear_mipmap, repeat_enable;
uniform sampler2D texture_albedo_ceiling : source_color, filter_linear_mipmap, repeat_enable;
uniform float point_size : hint_range(0.1, 128.0, 0.1) = 0.1f;

uniform float roughness : hint_range(0.0, 1.0) = 1.0f;
uniform sampler2D texture_metallic : hint_default_white, filter_linear_mipmap, repeat_enable;
uniform vec4 metallic_texture_channel;
uniform sampler2D texture_roughness : hint_roughness_r, filter_linear_mipmap, repeat_enable;

uniform float specular : hint_range(0.0, 1.0, 0.01) = 0.0f;
uniform float metallic : hint_range(0.0, 1.0, 0.01) = 0.0f;

uniform sampler2D texture_normal_floor : hint_roughness_normal, filter_linear_mipmap, repeat_enable;
uniform sampler2D texture_normal_wall : hint_roughness_normal, filter_linear_mipmap, repeat_enable;
uniform sampler2D texture_normal_ceiling : hint_roughness_normal, filter_linear_mipmap, repeat_enable;
uniform float normal_scale : hint_range(-16.0, 16.0) = 1.0f;
varying vec3 uv1_triplanar_pos;

uniform float uv1_blend_sharpness : hint_range(0.0, 150.0, 0.001) = 10.0f;
varying vec3 uv1_power_normal;

uniform vec3 uv1_scale;
uniform vec3 uv1_offset;
uniform vec3 uv2_scale;
uniform vec3 uv2_offset;

varying vec3 normal;

void vertex() {
	normal = MODEL_NORMAL_MATRIX * NORMAL;
	
	TANGENT = vec3(0.0, 0.0, -1.0) * abs(normal.x);
	TANGENT += vec3(1.0, 0.0, 0.0) * abs(normal.y);
	TANGENT += vec3(1.0, 0.0, 0.0) * abs(normal.z);
	TANGENT = inverse(MODEL_NORMAL_MATRIX) * normalize(TANGENT);
	
	BINORMAL = vec3(0.0, 1.0, 0.0) * abs(normal.x);
	BINORMAL += vec3(0.0, 0.0, -1.0) * abs(normal.y);
	BINORMAL += vec3(0.0, 1.0, 0.0) * abs(normal.z);
	BINORMAL = inverse(MODEL_NORMAL_MATRIX) * normalize(BINORMAL);
	
	// UV1 Triplanar: Enabled (with World Triplanar)
	uv1_power_normal = pow(abs(normal), vec3(uv1_blend_sharpness));
	uv1_triplanar_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz * uv1_scale + uv1_offset;
	uv1_power_normal /= dot(uv1_power_normal, vec3(1.0));
	uv1_triplanar_pos *= vec3(1.0, -1.0, 1.0);
}

vec4 triplanar_texture(sampler2D p_sampler, vec3 p_weights, vec3 p_triplanar_pos) {
	vec4 samp = vec4(0.0);
	samp += texture(p_sampler, p_triplanar_pos.xy) * p_weights.z;
	samp += texture(p_sampler, p_triplanar_pos.xz) * p_weights.y;
	samp += texture(p_sampler, p_triplanar_pos.zy * vec2(-1.0, 1.0)) * p_weights.x;
	return samp;
}

void fragment() {
	if (normal.y > 0.5f) {
		// Floor
		vec4 albedo_tex = triplanar_texture(texture_albedo_floor, uv1_power_normal, uv1_triplanar_pos);
		ALBEDO = albedo_floor.rgb * albedo_tex.rgb;
		NORMAL_MAP = triplanar_texture(texture_normal_floor, uv1_power_normal, uv1_triplanar_pos).rgb;
	} else if (normal.y < -0.5f) {
		// Ceiling
		vec4 albedo_tex = triplanar_texture(texture_albedo_ceiling, uv1_power_normal, uv1_triplanar_pos);
		ALBEDO = albedo_ceiling.rgb * albedo_tex.rgb;
		NORMAL_MAP = triplanar_texture(texture_normal_ceiling, uv1_power_normal, uv1_triplanar_pos).rgb;
	} else {
		// Wall
		vec4 albedo_tex = triplanar_texture(texture_albedo_wall, uv1_power_normal, uv1_triplanar_pos);
		ALBEDO = albedo_wall.rgb * albedo_tex.rgb;
		NORMAL_MAP = triplanar_texture(texture_normal_wall, uv1_power_normal, uv1_triplanar_pos).rgb;
	}
	
	float metallic_tex = dot(triplanar_texture(texture_metallic, uv1_power_normal, uv1_triplanar_pos), metallic_texture_channel);
	METALLIC = metallic_tex * metallic;
	SPECULAR = specular;
	
	vec4 roughness_texture_channel = vec4(1.0, 0.0, 0.0, 0.0);
	float roughness_tex = dot(triplanar_texture(texture_roughness, uv1_power_normal, uv1_triplanar_pos), roughness_texture_channel);
	ROUGHNESS = roughness_tex * roughness;
	
	NORMAL_MAP_DEPTH = normal_scale;
}
