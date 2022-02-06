#version 330

layout(location = 0) in vec4 a_Position;

uniform mat4 projection;


void main()
{
    gl_Position = projection * a_Position;

}
