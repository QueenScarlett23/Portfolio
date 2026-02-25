#version 330 core
layout (location = 0) in vec3 aPos;   // the position variable has attribute position 0
layout (location = 1) in vec2 aTexCoord; // texture coordinates
layout (location = 2) in vec3 aNormal; // normals

out vec2 TexCoords;  // outpute the texture co-ordinates
out vec3 Normal;    // output normals
out vec3 FragPos;   // output Fragment position

uniform mat4 transform;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = projection * view * model * vec4(aPos, 1.0);
    TexCoords = aTexCoord;
    FragPos = vec3(model * vec4(aPos, 1.0));

    Normal = mat3(transpose(inverse(model))) * aNormal; 
}       