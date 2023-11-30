// The default "sprite" vertex shader
// A lot of places in DuckGame assume the VS from the previous shader can be used, which is not the case on Switch.
// All of the places discovered so far relied on the sprite shader--as it's indeed the only vertex shader in the game
// Include this in Switch shaders to get the VS you probably want

#include "Macros.fxh"

struct VertexToPixel
{
	vec4 Position;
	vec4 Color   ;
	vec2 UV		 ;
};

@IFDEF_VS
uniform global
{
	mat4 MatrixTransform;
};


layout(location = 0) in vec3 a_position;
layout(location = 1) in vec4 a_color;
layout(location = 2) in vec2 a_texCoord;

layout(location = 0) out gl_PerVertex
{
	vec4 gl_Position;
};
layout(location = 1) out VertexToPixel vs_out;

void SpriteVertexShader()
{
	gl_Position = vec4(a_position, 1.0) * MatrixTransform;
    vs_out.Position = gl_Position;
	vs_out.Color    = a_color;
	vs_out.UV       = a_texCoord;
}
@ENDIF_VS