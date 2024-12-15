shader_type canvas_item;

uniform vec3 color : source_color;
uniform float num_cells : hint_range(2.0, 20.0, 1.0) = 10.0;
uniform float speed : hint_range(0.1, 4.0, 0.01) = 1.0;
uniform float smoothness : hint_range(0.5, 2.0, 0.01) = 1.0;
uniform float angle : hint_range(0.0, 360.0) = 45.0;
uniform float ripple_time : hint_range(0, 1.2) = 0.243;

float rectanglef(vec2 uv, float width, float height, float feather) {
    vec2 uv_cartesian = uv * 2.0 - 1.0;
    vec2 uv_reflected = abs(uv_cartesian);
    float dfx = smoothstep(width, width + feather, uv_reflected.x);
    float dfy = smoothstep(height, height + feather, uv_reflected.y);
    return max(dfx, dfy);
}

vec2 rotation(vec2 uv, vec2 center, float ang) {
    mat2 rotation = mat2(
        vec2(cos(ang), -sin(ang)),
        vec2(sin(ang), cos(ang))
    );
    uv -= center;
    uv *= rotation;
    uv += center;
    return uv;
}

void fragment() {
    vec2 igrid = floor(UV * num_cells) / num_cells;

    igrid = rotation(igrid, vec2(0.5), angle * PI / 180.0);
    igrid.x += 2.0 - mod(ripple_time * speed, 4.0);
    vec2 fgrid = fract(UV * num_cells);
    float rect_mask = rectanglef(igrid, 0.1, 2.0, smoothness);
    float grid_mask = 1.0 - rectanglef(fgrid, rect_mask, rect_mask, 0.01);
    float outline_mask = 1.0 - rectanglef(fgrid, rect_mask + 0.1, rect_mask + 0.1, 0.01) - grid_mask;
    vec3 outline = outline_mask * (COLOR.rgb * 3.0); // brighten the underlying color a bit for the outline
    vec3 tex = COLOR.rgb;

    // Sample the alpha channel of the texture
    float alpha = COLOR.a;

    // Blend the output based on the alpha value of the texture
    vec3 output = mix(tex, outline, outline_mask);

    // Apply the transparency based on the alpha channel
    COLOR = vec4(output, alpha * (outline_mask + grid_mask));
}
