shader_type canvas_item;

uniform sampler2D dissolve_texture : source_color;
uniform float dissolve_value : hint_range(0, 1);
uniform float burn_size : hint_range(0.0, 1.0, 0.01);
uniform vec4 burn_color : source_color;
uniform bool modulate_color; // Use modulate or not

void fragment() {
    vec4 main_texture = texture(TEXTURE, UV);       // Fetch texture
    vec4 noise_texture = texture(dissolve_texture, UV); // Noise texture

    float burn_size_step = burn_size * step(0.001, dissolve_value) * step(dissolve_value, 0.999);
    float threshold = smoothstep(noise_texture.r - burn_size_step, noise_texture.r, dissolve_value);
    float border = smoothstep(noise_texture.r, noise_texture.r + burn_size_step, dissolve_value);

    // Calculate the burn effect
    vec3 burn_effect = mix(burn_color.rgb, main_texture.rgb, border);

    // Conditionally apply modulate alpha
    if (modulate_color) {
        COLOR.rgb = burn_effect * (COLOR.rgb / max(COLOR.rgb.r + COLOR.rgb.g + COLOR.rgb.b, 1.0));
        COLOR.a = main_texture.a * threshold * COLOR.a; // Combine dissolve alpha with modulate alpha
    } else {
        COLOR.rgb = burn_effect;           // Ignore modulation
        COLOR.a = main_texture.a * threshold; // Apply only dissolve alpha
    }
}
