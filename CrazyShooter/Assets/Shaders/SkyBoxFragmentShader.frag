#version 330 core
#extension GL_ARB_separate_shader_objects : enable

in vec2 TexCoords;
out vec4 FragColor;

uniform sampler2D uTexture;

void main()
{
    vec4 texColor = texture(uTexture, TexCoords);
    FragColor = texColor;
}
