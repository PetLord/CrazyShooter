#version 330 core
#extension GL_ARB_separate_shader_objects : enable


layout(location = 0) in vec3 aPosition;

out vec3 TexCoords;

uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    TexCoords = aPosition;
    
    mat4 viewNoTranslation = mat4(mat3(uView));
    vec4 pos = uProjection * viewNoTranslation * vec4(aPosition, 1.0);

    // set w component equal to z to push depth to far plane
    gl_Position = pos.xyww;
}
