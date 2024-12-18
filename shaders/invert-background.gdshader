shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE: hint_screen_texture;

void fragment() {
	float s = sin(TIME);
	vec4 color = texture(SCREEN_TEXTURE, UV);
	COLOR = vec4(mix(1.0 - color.rgb, COLOR.rgb, 0.1),color.a);
}
