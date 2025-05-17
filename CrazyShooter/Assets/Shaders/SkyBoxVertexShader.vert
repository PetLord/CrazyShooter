#version 330 core
#extension GL_ARB_separate_shader_objects : enable

layout(location = 0) in vec3 aPos;
layout(location = 2) in vec2 aTexCoord; // NOT location 1 (that’s normal)

uniform mat4 uView;
uniform mat4 uProjection;
uniform mat4 uModel;

out vec2 TexCoords;

void main()
{
    TexCoords = aTexCoord;
    gl_Position = uProjection * uView * uModel * vec4(aPos, 1.0);
}
