#version 330 core
#extension GL_ARB_separate_shader_objects : enable

in vec3 TexCoords;
out vec4 FragColor;

uniform samplerCube skybox;

void main()
{
    FragColor = texture(skybox, TexCoords);
}
