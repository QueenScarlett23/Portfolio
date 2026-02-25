#include <string>
#include <iostream>
#include <filesystem>


#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <fstream>
#include <fstream>
#include <vector>
#include "Camera.h"

bool stob(std::string s, bool throw_on_error);


struct Light {
	glm::vec3 lightPosition;
	glm::vec3 lightColour;
	float strength = 0.6;

	Light() {}

	Light(glm::vec3 lightPositi,
		glm::vec3 lightColo) {
		lightPosition = lightPositi;
		lightColour = lightColo;
	}

	Light(glm::vec3 lightPositi,
		glm::vec3 lightColo,
		float stre) {
		lightPosition = lightPositi;
		lightColour = lightColo;
		strength = stre;
	}
};

struct DirLight {
	glm::vec3 direction;

	glm::vec3 ambient;
	glm::vec3 diffuse;
	glm::vec3 specular;
};

struct PointLight {
	glm::vec3 position;

	float constant;
	float linear;
	float quadratic;

	glm::vec3 ambient;
	glm::vec3 diffuse;
	glm::vec3 specular;
};

struct SpotLight {
	glm::vec3 position;
	glm::vec3 direction;
	float cutOff;
	float outerCutOff;

	float constant;
	float linear;
	float quadratic;

	glm::vec3 ambient;
	glm::vec3 diffuse;
	glm::vec3 specular;
};

// settings
DirLight dirLight;
boolean spotLighActive = false;
PointLight pointLights[4];


glm::vec3 cubePositions[] = {
			glm::vec3(0.0f,  0.0f,  0.0f),
			glm::vec3(2.0f,  5.0f, -15.0f),
			glm::vec3(-1.5f, -2.2f, -2.5f),
			glm::vec3(-3.8f, -2.0f, -12.3f),
			glm::vec3(2.4f, -0.4f, -3.5f),
			glm::vec3(-1.7f,  3.0f, -7.5f),
			glm::vec3(1.3f, -2.0f, -2.5f),
			glm::vec3(1.5f,  2.0f, -2.5f),
			glm::vec3(1.5f,  0.2f, -1.5f),
			glm::vec3(-1.3f,  1.0f, -1.5f)
};

glm::vec3 pointLightPositions[] = {
		glm::vec3(-5.7f,  7.5f,  2.5f),
		glm::vec3(-3.9f,  7.5f, 8.05f),
		glm::vec3(-10.1f, 7.5f, 9.45f),
		glm::vec3(-7.24f, 7.5f, 16.0f)
};

float vertex_door[] = {
	// part 1
	0.00, 0.00, -0.13,      0.0f, 0.0f,     0.0f,  0.0f, -1.0f,
	2.00, 0.00, -0.13,      1.0f, 0.0f,     0.0f,  0.0f, -1.0f,
	0.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  0.0f, -1.0f,
	0.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  0.0f, -1.0f,
	2.00, 0.00, -0.13,      1.0f, 0.0f,     0.0f,  0.0f, -1.0f,
	2.00, 2.20, -0.13,      1.0f, 1.0f,     0.0f,  0.0f, -1.0f,

	0.00, 0.00, 0.13,       0.0f, 0.0f,     0.0f,  0.0f, 1.0f,
	2.00, 0.00, 0.13,       1.0f, 0.0f,     0.0f,  0.0f, 1.0f,
	0.00, 2.20, 0.13,       0.0f, 1.0f,     0.0f,  0.0f, 1.0f,
	0.00, 2.20, 0.13,       0.0f, 1.0f,     0.0f,  0.0f, 1.0f,
	2.00, 0.00, 0.13,       1.0f, 0.0f,     0.0f,  0.0f, 1.0f,
	2.00, 2.20, 0.13,       1.0f, 1.0f,     0.0f,  0.0f, 1.0f,

	0.00, 0.00, 0.13,       0.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
	0.00, 0.00, -0.13,      1.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
	0.00, 2.20, 0.13,       0.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
	0.00, 2.20, 0.13,       0.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
	0.00, 0.00, -0.13,      1.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
	0.00, 2.20, -0.13,      1.0f, 1.0f,     -1.0f,  0.0f,  0.0f,

	2.00, 0.00, 0.13,       0.0f, 0.0f,     1.0f,  0.0f,  0.0f,
	2.00, 0.00, -0.13,      1.0f, 0.0f,     1.0f,  0.0f,  0.0f,
	2.00, 2.20, 0.13,       0.0f, 1.0f,     1.0f,  0.0f,  0.0f,
	2.00, 2.20, 0.13,       0.0f, 1.0f,     1.0f,  0.0f,  0.0f,
	2.00, 0.00, -0.13,      1.0f, 0.0f,     1.0f,  0.0f,  0.0f,
	2.00, 2.20, -0.13,      1.0f, 1.0f,     1.0f,  0.0f,  0.0f,

	0.00, 2.20, 0.13,       0.0f, 0.0f,     0.0f,  1.0f,  0.0f,
	2.00, 2.20, 0.13,       1.0f, 0.0f,     0.0f,  1.0f,  0.0f,
	0.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  1.0f,  0.0f,
	0.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  1.0f,  0.0f,
	2.00, 2.20, 0.13,       1.0f, 0.0f,     0.0f,  1.0f,  0.0f,
	2.00, 2.20, -0.13,      1.0f, 1.0f,     0.0f,  1.0f,  0.0f,

	0.00, 0.00, 0.13,       0.0f, 0.0f,     0.0f, -1.0f,  0.0f,
	2.00, 0.00, 0.13,       1.0f, 0.0f,     0.0f, -1.0f,  0.0f,
	0.00, 0.00, -0.13,      0.0f, 1.0f,     0.0f, -1.0f,  0.0f,
	0.00, 0.00, -0.13,      0.0f, 1.0f,     0.0f, -1.0f,  0.0f,
	2.00, 0.00, 0.13,       1.0f, 0.0f,     0.0f, -1.0f,  0.0f,
	2.00, 0.00, -0.13,      1.0f, 1.0f,     0.0f, -1.0f,  0.0f,

	// part 2
	2.00, 1.90, -0.13,      0.0f, 0.0f,     0.0f,  0.0f, -1.0f,
	3.00, 1.90, -0.13,      1.0f, 0.0f,     0.0f,  0.0f, -1.0f,
	2.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  0.0f, -1.0f,
	2.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  0.0f, -1.0f,
	3.00, 1.90, -0.13,      1.0f, 0.0f,     0.0f,  0.0f, -1.0f,
	3.00, 2.20, -0.13,      1.0f, 1.0f,     0.0f,  0.0f, -1.0f,

	2.00, 1.90, 0.13,       0.0f, 0.0f,     0.0f,  0.0f, 1.0f,
	3.00, 1.90, 0.13,       1.0f, 0.0f,     0.0f,  0.0f, 1.0f,
	2.00, 2.20, 0.13,       0.0f, 1.0f,     0.0f,  0.0f, 1.0f,
	2.00, 2.20, 0.13,       0.0f, 1.0f,     0.0f,  0.0f, 1.0f,
	3.00, 1.90, 0.13,       1.0f, 0.0f,     0.0f,  0.0f, 1.0f,
	3.00, 2.20, 0.13,       1.0f, 1.0f,     0.0f,  0.0f, 1.0f,

	2.00, 1.90, 0.13,       0.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
	2.00, 1.90, -0.13,      1.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
	2.00, 2.20, 0.13,       0.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
	2.00, 2.20, 0.13,       0.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
	2.00, 1.90, -0.13,      1.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
	2.00, 2.20, -0.13,      1.0f, 1.0f,     -1.0f,  0.0f,  0.0f,

	3.00, 1.90, 0.13,       0.0f, 0.0f,     1.0f,  0.0f,  0.0f,
	3.00, 1.90, -0.13,      1.0f, 0.0f,     1.0f,  0.0f,  0.0f,
	3.00, 2.20, 0.13,       0.0f, 1.0f,     1.0f,  0.0f,  0.0f,
	3.00, 2.20, 0.13,       0.0f, 1.0f,     1.0f,  0.0f,  0.0f,
	3.00, 1.90, -0.13,      1.0f, 0.0f,     1.0f,  0.0f,  0.0f,
	3.00, 2.20, -0.13,      1.0f, 1.0f,     1.0f,  0.0f,  0.0f,

	2.00, 2.20, 0.13,       0.0f, 0.0f,     0.0f,  1.0f,  0.0f,
	3.00, 2.20, 0.13,       1.0f, 0.0f,     0.0f,  1.0f,  0.0f,
	2.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  1.0f,  0.0f,
	2.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  1.0f,  0.0f,
	3.00, 2.20, 0.13,       1.0f, 0.0f,     0.0f,  1.0f,  0.0f,
	3.00, 2.20, -0.13,      1.0f, 1.0f,     0.0f,  1.0f,  0.0f,

	2.00, 1.90, 0.13,       0.0f, 0.0f,     0.0f, -1.0f,  0.0f,
	3.00, 1.90, 0.13,       1.0f, 0.0f,     0.0f, -1.0f,  0.0f,
	2.00, 1.90, -0.13,      0.0f, 1.0f,     0.0f, -1.0f,  0.0f,
	2.00, 1.90, -0.13,      0.0f, 1.0f,     0.0f, -1.0f,  0.0f,
	3.00, 1.90, 0.13,       1.0f, 0.0f,     0.0f, -1.0f,  0.0f,
	3.00, 1.90, -0.13,      1.0f, 1.0f,     0.0f, -1.0f,  0.0f,

	// part 3
	3.00, 0.00, -0.13,      0.0f, 0.0f,     0.0f,  0.0f, -1.0f,
	5.00, 0.00, -0.13,      1.0f, 0.0f,     0.0f,  0.0f, -1.0f,
	3.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  0.0f, -1.0f,
	3.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  0.0f, -1.0f,
	5.00, 0.00, -0.13,      1.0f, 0.0f,     0.0f,  0.0f, -1.0f,
	5.00, 2.20, -0.13,      1.0f, 1.0f,     0.0f,  0.0f, -1.0f,

	3.00, 0.00, 0.13,       0.0f, 0.0f,     0.0f,  0.0f, 1.0f,
	5.00, 0.00, 0.13,       1.0f, 0.0f,     0.0f,  0.0f, 1.0f,
	3.00, 2.20, 0.13,       0.0f, 1.0f,     0.0f,  0.0f, 1.0f,
	3.00, 2.20, 0.13,       0.0f, 1.0f,     0.0f,  0.0f, 1.0f,
	5.00, 0.00, 0.13,       1.0f, 0.0f,     0.0f,  0.0f, 1.0f,
	5.00, 2.20, 0.13,       1.0f, 1.0f,     0.0f,  0.0f, 1.0f,

	3.00, 0.00, 0.13,       0.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
	3.00, 0.00, -0.13,      1.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
	3.00, 2.20, 0.13,       0.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
	3.00, 2.20, 0.13,       0.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
	3.00, 0.00, -0.13,      1.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
	3.00, 2.20, -0.13,      1.0f, 1.0f,     -1.0f,  0.0f,  0.0f,

	5.00, 0.00, 0.13,       0.0f, 0.0f,     1.0f,  0.0f,  0.0f,
	5.00, 0.00, -0.13,      1.0f, 0.0f,     1.0f,  0.0f,  0.0f,
	5.00, 2.20, 0.13,       0.0f, 1.0f,     1.0f,  0.0f,  0.0f,
	5.00, 2.20, 0.13,       0.0f, 1.0f,     1.0f,  0.0f,  0.0f,
	5.00, 0.00, -0.13,      1.0f, 0.0f,     1.0f,  0.0f,  0.0f,
	5.00, 2.20, -0.13,      1.0f, 1.0f,     1.0f,  0.0f,  0.0f,

	3.00, 2.20, 0.13,       0.0f, 0.0f,     0.0f,  1.0f,  0.0f,
	5.00, 2.20, 0.13,       1.0f, 0.0f,     0.0f,  1.0f,  0.0f,
	3.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  1.0f,  0.0f,
	3.00, 2.20, -0.13,      0.0f, 1.0f,     0.0f,  1.0f,  0.0f,
	5.00, 2.20, 0.13,       1.0f, 0.0f,     0.0f,  1.0f,  0.0f,
	5.00, 2.20, -0.13,      1.0f, 1.0f,     0.0f,  1.0f,  0.0f,

	3.00, 0.00, 0.13,       0.0f, 0.0f,     0.0f, -1.0f,  0.0f,
	5.00, 0.00, 0.13,       1.0f, 0.0f,     0.0f, -1.0f,  0.0f,
	3.00, 0.00, -0.13,      0.0f, 1.0f,     0.0f, -1.0f,  0.0f,
	3.00, 0.00, -0.13,      0.0f, 1.0f,     0.0f, -1.0f,  0.0f,
	5.00, 0.00, 0.13,       1.0f, 0.0f,     0.0f, -1.0f,  0.0f,
	5.00, 0.00, -0.13,      1.0f, 1.0f,     0.0f, -1.0f,  0.0f };

float vertex_cube[] = {
-0.50, -0.50, -0.50,    0.0f, 0.0f,     0.0f,  0.0f, -1.0f,
0.50, -0.50, -0.50,     1.0f, 0.0f,     0.0f,  0.0f, -1.0f,
-0.50, 0.50, -0.50,     0.0f, 1.0f,     0.0f,  0.0f, -1.0f,
-0.50, 0.50, -0.50,     0.0f, 1.0f,     0.0f,  0.0f, -1.0f,
0.50, -0.50, -0.50,     1.0f, 0.0f,     0.0f,  0.0f, -1.0f,
0.50, 0.50, -0.50,      1.0f, 1.0f,     0.0f,  0.0f, -1.0f,
-0.50, -0.50, 0.50,     0.0f, 0.0f,     0.0f,  0.0f, 1.0f,
0.50, -0.50, 0.50,      1.0f, 0.0f,     0.0f,  0.0f, 1.0f,
-0.50, 0.50, 0.50,      0.0f, 1.0f,     0.0f,  0.0f, 1.0f,
-0.50, 0.50, 0.50,      0.0f, 1.0f,     0.0f,  0.0f, 1.0f,
0.50, -0.50, 0.50,      1.0f, 0.0f,     0.0f,  0.0f, 1.0f,
0.50, 0.50, 0.50,       1.0f, 1.0f,     0.0f,  0.0f, 1.0f,
-0.50, -0.50, 0.50,     0.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
-0.50, -0.50, -0.50,    1.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
-0.50, 0.50, 0.50,      0.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
-0.50, 0.50, 0.50,      0.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
-0.50, -0.50, -0.50,    1.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
-0.50, 0.50, -0.50,     1.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
0.50, -0.50, 0.50,      0.0f, 0.0f,     1.0f,  0.0f,  0.0f,
0.50, -0.50, -0.50,     1.0f, 0.0f,     1.0f,  0.0f,  0.0f,
0.50, 0.50, 0.50,       0.0f, 1.0f,     1.0f,  0.0f,  0.0f,
0.50, 0.50, 0.50,       0.0f, 1.0f,     1.0f,  0.0f,  0.0f,
0.50, -0.50, -0.50,     1.0f, 0.0f,     1.0f,  0.0f,  0.0f,
0.50, 0.50, -0.50,      1.0f, 1.0f,     1.0f,  0.0f,  0.0f,
-0.50, 0.50, 0.50,      0.0f, 0.0f,     0.0f,  1.0f,  0.0f,
0.50, 0.50, 0.50,       1.0f, 0.0f,     0.0f,  1.0f,  0.0f,
-0.50, 0.50, -0.50,     0.0f, 1.0f,     0.0f,  1.0f,  0.0f,
-0.50, 0.50, -0.50,     0.0f, 1.0f,     0.0f,  1.0f,  0.0f,
0.50, 0.50, 0.50,       1.0f, 0.0f,     0.0f,  1.0f,  0.0f,
0.50, 0.50, -0.50,      1.0f, 1.0f,     0.0f,  1.0f,  0.0f,
-0.50, -0.50, 0.50,     0.0f, 0.0f,     0.0f, -1.0f,  0.0f,
0.50, -0.50, 0.50,      1.0f, 0.0f,     0.0f, -1.0f,  0.0f,
-0.50, -0.50, -0.50,    0.0f, 1.0f,     0.0f, -1.0f,  0.0f,
-0.50, -0.50, -0.50,    0.0f, 1.0f,     0.0f, -1.0f,  0.0f,
0.50, -0.50, 0.50,      1.0f, 0.0f,     0.0f, -1.0f,  0.0f,
0.50, -0.50, -0.50,     1.0f, 1.0f,     0.0f, -1.0f,  0.0f
/*
	// vertex				// texture		// normals
	// back
	-0.5f, -0.5f, -0.5f,	0.0f, 0.0f,		0.0f,  0.0f, -1.0f,
	0.5f, -0.5f, -0.5f,		1.0f, 0.0f,		0.0f,  0.0f, -1.0f,
	0.5f, 0.5f, -0.5f,		1.0f, 1.0f,		0.0f,  0.0f, -1.0f,
	0.5f, 0.5f, -0.5f,		1.0f, 1.0f,		0.0f,  0.0f, -1.0f,
	-0.5f, 0.5f, -0.5f,		0.0f, 1.0f,		0.0f,  0.0f, -1.0f,
	-0.5f, -0.5f, -0.5f,	0.0f, 0.0f,		0.0f,  0.0f, -1.0f,

	// front
	-0.5f, -0.5f, 0.5f,		0.0f, 0.0f,		0.0f,  0.0f, 1.0f,
	0.5f, -0.5f, 0.5f,		1.0f, 0.0f,		0.0f,  0.0f, 1.0f,
	0.5f, 0.5f, 0.5f,		1.0f, 1.0f,		0.0f,  0.0f, 1.0f,
	0.5f, 0.5f, 0.5f,		1.0f, 1.0f,		0.0f,  0.0f, 1.0f,
	-0.5f, 0.5f, 0.5f,		0.0f, 1.0f,		0.0f,  0.0f, 1.0f,
	-0.5f, -0.5f, 0.5f,		0.0f, 0.0f,		0.0f,  0.0f, 1.0f,

	// left
	-0.5f, 0.5f, 0.5f,		1.0f, 0.0f,		-1.0f,  0.0f,  0.0f,
	-0.5f, 0.5f, -0.5f,		1.0f, 1.0f,		-1.0f,  0.0f,  0.0f,
	-0.5f, -0.5f, -0.5f,	0.0f, 1.0f,		-1.0f,  0.0f,  0.0f,
	-0.5f, -0.5f, -0.5f,	0.0f, 1.0f,		-1.0f,  0.0f,  0.0f,
	-0.5f, -0.5f, 0.5f,		0.0f, 0.0f,		-1.0f,  0.0f,  0.0f,
	-0.5f, 0.5f, 0.5f,		1.0f, 0.0f,		-1.0f,  0.0f,  0.0f,

	// right
	0.5f, 0.5f, 0.5f,		1.0f, 0.0f,		1.0f,  0.0f,  0.0f,
	0.5f, 0.5f, -0.5f,		1.0f, 1.0f,		1.0f,  0.0f,  0.0f,
	0.5f, -0.5f, -0.5f,		0.0f, 1.0f,		1.0f,  0.0f,  0.0f,
	0.5f, -0.5f, -0.5f,		0.0f, 1.0f,		1.0f,  0.0f,  0.0f,
	0.5f, -0.5f, 0.5f,		0.0f, 0.0f,		1.0f,  0.0f,  0.0f,
	0.5f, 0.5f, 0.5f,		1.0f, 0.0f,		1.0f,  0.0f,  0.0f,

	// bottom
	-0.5f, -0.5f, -0.5f,	0.0f, 1.0f,		0.0f, -1.0f,  0.0f,
	0.5f, -0.5f, -0.5f,		1.0f, 1.0f,		0.0f, -1.0f,  0.0f,
	0.5f, -0.5f, 0.5f,		1.0f, 0.0f,		0.0f, -1.0f,  0.0f,
	0.5f, -0.5f, 0.5f,		1.0f, 0.0f,		0.0f, -1.0f,  0.0f,
	-0.5f, -0.5f, 0.5f,		0.0f, 0.0f,		0.0f, -1.0f,  0.0f,
	-0.5f, -0.5f, -0.5f,	0.0f, 1.0f,		0.0f, -1.0f,  0.0f,

	// top
	-0.5f, 0.5f, -0.5f,		0.0f, 1.0f,		0.0f,  1.0f,  0.0f,
	0.5f, 0.5f, -0.5f,		1.0f, 1.0f,		0.0f,  1.0f,  0.0f,
	0.5f, 0.5f, 0.5f,		1.0f, 0.0f,		0.0f,  1.0f,  0.0f,
	0.5f, 0.5f, 0.5f,		1.0f, 0.0f,		0.0f,  1.0f,  0.0f,
	-0.5f, 0.5f, 0.5f,		0.0f, 0.0f,		0.0f,  1.0f,  0.0f,
	-0.5f, 0.5f, -0.5f,		0.0f, 1.0f,		0.0f,  1.0f,  0.0f
	*/
};

float* level_vertex_data;
int level_size = 0;

boolean fileRead = false;
boolean levelGenerated = false;

struct Position {
	Position(float x, float z, char t) {
		floor_x = x;
		floor_z = z;
		type = t;
	}
	char type;

	float floor_x;
	float floor_z;

	float wall_top;
	float wall_bottom;
	float wall_front;
	float wall_back;
	float wall_left;
	float wall_right;
};

class Level {
private:
	std::vector<std::vector<char>> simple_level_data;
	//int str_level_size;

	char wall = 'w';
	char floor = '*';
	char door = 'd';
	char empty = '_';

	float floor_size = 0.25;
	float wall_height = 0.28f;
	float door_height = 0.04f;
	float door_elevation = 0.24f;

public:
	
	std::string texturePath;
	float specularLevel;


	Level() {
		dirLight.direction = { -0.2f, -1.0f, -0.3f };
		dirLight.ambient = { 0.05f, 0.05f, 0.05f };
		dirLight.diffuse = { 0.2f, 0.2f, 0.2f };
		dirLight.specular = { 0.01f, 0.01f, 0.01f };
	}

	void SetUp() {
		/*cubePositions = new glm::vec3[]{
			glm::vec3(0.0f,  0.0f,  0.0f),
			glm::vec3(2.0f,  5.0f, -15.0f),
			glm::vec3(-1.5f, -2.2f, -2.5f),
			glm::vec3(-3.8f, -2.0f, -12.3f),
			glm::vec3(2.4f, -0.4f, -3.5f),
			glm::vec3(-1.7f,  3.0f, -7.5f),
			glm::vec3(1.3f, -2.0f, -2.5f),
			glm::vec3(1.5f,  2.0f, -2.5f),
			glm::vec3(1.5f,  0.2f, -1.5f),
			glm::vec3(-1.3f,  1.0f, -1.5f)
		};
		vertices = new float[] {
			-0.5f, -0.5f, -0.5f, 0.0f, 0.0f,
				0.5f, -0.5f, -0.5f, 1.0f, 0.0f,
				0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
				0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
				-0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
				-0.5f, -0.5f, -0.5f, 0.0f, 0.0f,

				-0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
				0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
				0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
				0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
				-0.5f, 0.5f, 0.5f, 0.0f, 1.0f,
				-0.5f, -0.5f, 0.5f, 0.0f, 0.0f,

				-0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
				-0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
				-0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
				-0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
				-0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
				-0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

				0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
				0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
				0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
				0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
				0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
				0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

				-0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
				0.5f, -0.5f, -0.5f, 1.0f, 1.0f,
				0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
				0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
				-0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
				-0.5f, -0.5f, -0.5f, 0.0f, 1.0f,

				-0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
				0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
				0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
				0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
				-0.5f, 0.5f, 0.5f, 0.0f, 0.0f,
				-0.5f, 0.5f, -0.5f, 0.0f, 1.0f
		};*/
	}

	// generates level from simple map
	void generateLevel() {
		std::vector<std::vector<Position>> peliminary_positions = std::vector<std::vector<Position>>();
		std::vector<float> tempVertex_data = std::vector<float>();

		// loading preliminary data
		for (size_t bb = 0; bb < 4; bb++) // needed because I dont know why. - spent 8 hours figuring this out and I am half dead right now. I still need to do the walls and doors.................pain
			for (int i = 0; i < simple_level_data.size(); i++)
			{
				std::vector<Position> tempPosVector = std::vector<Position>();

				//std::cout << simple_level_data[i].size() << std::endl;

				for (int ii = 0; ii < simple_level_data[i].size(); ii++)
				{
					//std::cout << "x - " << i << "; z - " << ii << std::endl;

					if (simple_level_data[i][ii] != empty) {
						// set floor
						tempPosVector.push_back(Position(i * -floor_size, ii * floor_size, simple_level_data[i][ii]));
						// check wall or door
					}
					else {
						tempPosVector.push_back(Position(i * floor_size, ii * floor_size, simple_level_data[i][ii]));
					}
				}
				peliminary_positions.push_back(tempPosVector);

				//std::cout << std::endl;
			}

		// setting vertex data
		for (int i = 0; i < peliminary_positions.size(); i++)
		{
			for (int ii = 0; ii < peliminary_positions[i].size(); ii++)
			{
				// floor
				if (peliminary_positions[i][ii].type != empty) {
					//std::cout << "x - " << i << " | " << peliminary_positions[i][ii].floor_x << "; z - " << ii << " | " << peliminary_positions[i][ii].floor_z << std::endl;
					// floor
#pragma region Setting Floor vertex data
					// point 1
					// make triangle
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
					// set texture coordinates
					tempVertex_data.push_back(1);
					tempVertex_data.push_back(1);

					// set normal positions
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(1);
					tempVertex_data.push_back(0);

					// point 2
					// make triangle
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_x + floor_size);
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_z + floor_size);
					// set texture coordinates
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(0);

					// set normal positions
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(1);
					tempVertex_data.push_back(0);

					// point 3
					// make triangle
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_x + floor_size);
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
					// set texture coordinates
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(1);

					// set normal positions
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(1);
					tempVertex_data.push_back(0);

					// size 2
					// point 1
					// make triangle
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
					// set texture coordinates
					tempVertex_data.push_back(1);
					tempVertex_data.push_back(1);

					// set normal positions
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(1);
					tempVertex_data.push_back(0);

					// point 2
					// make triangle
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_x + floor_size);
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_z + floor_size);
					// set texture coordinates
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(0);

					// set normal positions
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(1);
					tempVertex_data.push_back(0);

					// point 3
					// make triangle
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(peliminary_positions[i][ii].floor_z + floor_size);
					// set texture coordinates
					tempVertex_data.push_back(1);
					tempVertex_data.push_back(0);

					// set normal positions
					tempVertex_data.push_back(0);
					tempVertex_data.push_back(1);
					tempVertex_data.push_back(0);
#pragma endregion

					if (peliminary_positions[i][ii].type == wall || peliminary_positions[i][ii].type == door)
					{
						bool l = false, r = false, f = false, b = false;

						if (i >= 1 && peliminary_positions[i - 1].size() > ii) {
							// check front
							if (peliminary_positions[i - 1][ii].type == wall || peliminary_positions[i - 1][ii].type == door)
								f = true;
						}
						//std::cout << i << " - " << ii << std::endl;
						if (i < peliminary_positions.size() && peliminary_positions[i + 1].size() > ii) {
							// check back
							if (peliminary_positions[i + 1][ii].type == wall || peliminary_positions[i + 1][ii].type == door)
								b = true;
						}

						if (ii > 0) {
							// check left
							if (peliminary_positions[i][ii - 1].type == wall || peliminary_positions[i][ii - 1].type == door)
								l = true;
						}

						if (ii < peliminary_positions[i].size())
						{
							// check right
							if (peliminary_positions[i][ii + 1].type == wall || peliminary_positions[i][ii + 1].type == door)
								r = true;
						}

						if (f) {
							// point 1
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
							if (peliminary_positions[i][ii].type == door)
								tempVertex_data.push_back(door_elevation);
							else
								tempVertex_data.push_back(0);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
							// set texture coordinates
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// point 2
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x + floor_size);
							tempVertex_data.push_back(wall_height);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
							// set texture coordinates
							tempVertex_data.push_back(1);
							tempVertex_data.push_back(1);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// point 3
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
							tempVertex_data.push_back(wall_height);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
							// set texture coordinates
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// side 2
							// point 1
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
							if (peliminary_positions[i][ii].type == door)
								tempVertex_data.push_back(door_elevation);
							else
								tempVertex_data.push_back(0);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
							// set texture coordinates
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// point 2
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x + floor_size);
							tempVertex_data.push_back(wall_height);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
							// set texture coordinates
							tempVertex_data.push_back(1);
							tempVertex_data.push_back(1);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// point 3
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x + floor_size);
							if (peliminary_positions[i][ii].type == door)
								tempVertex_data.push_back(door_elevation);
							else
								tempVertex_data.push_back(0);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
							// set texture coordinates
							tempVertex_data.push_back(1);
							tempVertex_data.push_back(0);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);
						}

						if (r) {
							// point 1
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
							if (peliminary_positions[i][ii].type == door)
								tempVertex_data.push_back(door_elevation);
							else
								tempVertex_data.push_back(0);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
							// set texture coordinates
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// point 2
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
							tempVertex_data.push_back(wall_height);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z + floor_size);
							// set texture coordinates
							tempVertex_data.push_back(1);
							tempVertex_data.push_back(1);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// point 3
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
							tempVertex_data.push_back(wall_height);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
							// set texture coordinates
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// side 2
							// point 1
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
							if (peliminary_positions[i][ii].type == door)
								tempVertex_data.push_back(door_elevation);
							else
								tempVertex_data.push_back(0);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z);
							// set texture coordinates
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// point 2
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
							tempVertex_data.push_back(wall_height);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z + floor_size);
							// set texture coordinates
							tempVertex_data.push_back(1);
							tempVertex_data.push_back(1);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);

							// point 3
							// make triangle
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_x);
							if (peliminary_positions[i][ii].type == door)
								tempVertex_data.push_back(door_elevation);
							else
								tempVertex_data.push_back(0);
							tempVertex_data.push_back(peliminary_positions[i][ii].floor_z + floor_size);
							// set texture coordinates
							tempVertex_data.push_back(1);
							tempVertex_data.push_back(0);

							// set normal positions
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(0);
							tempVertex_data.push_back(1);
						}
					}
				}
			}
			//std::cout << std::endl;
		}


		// setting pointer
		level_vertex_data = new float[tempVertex_data.size()];
		//std::cout << "size of vertex: " << tempVertex_data.size() << " | " << p << std::endl;

		// loading texture


		//level_vertex_data = ;


		for (int i = 0; i < tempVertex_data.size(); i++)
		{
			level_vertex_data[i] = tempVertex_data[i];

			/*
			if ((i + 1) % 8 == 0 && i != 0)
			{
				//std::cout << i << " - " << level_vertex_data[i] << std::endl;
			}
			else
			{
				//std::cout << i << " - " << level_vertex_data[i] << "; ";
			}*/
			level_size++;
		}
		//level_size ;

		//std::cout << "level_size: " << level_size;
		/*level_vertex_data = new float[]{
				0, 0, 0,	1, 1,	0, 1, 0,
				-2, 0, -2,	0, 0,	0, 1, 0,
				0, 0, -2,	0, 1,	0, 1, 0,
		};*/

		levelGenerated = true;
	}

	/*
	float GetPos(char c, int pos) {
		switch (c)
		{
		case 'x':
			return lights[pos].lightPosition.x;
		case 'y':
			return lights[pos].lightPosition.y;
		case 'z':
			return lights[pos].lightPosition.z;
		default:
			break;
		}
	}

	float GetCol(char c, int pos) {
		switch (c)
		{
		case 'r':
			return lights[pos].lightColour.r;
		case 'g':
			return lights[pos].lightColour.g;
		case 'b':
			return lights[pos].lightColour.b;
		default:
			break;
		}
	}
	*/

	void ReadData(std::string dir = "resources/Wolfenstein/level.txt") {
		std::string line;
		std::ifstream myfile(dir);
		int lastSlash = -1;
		for (int i = dir.length(); i > 0; i--)
		{
			if (dir.substr(i, 1) == "/") {
				lastSlash = i;
				break;
			}
		}
		std::string path = dir.substr(0, lastSlash + 1);

		if (myfile.is_open())
		{
			int phaseCounter = 0;
			//char* level_data;

			//str_level_size = 0;
			int lightCounter = -1;

			Light tmpLights[3];

			while (getline(myfile, line))
			{
				if (line.find("#") == std::string::npos)
				{
					switch (phaseCounter)
					{
					case 0:
						if (line.substr(0, 2) == "::") {
							std::cout << "moving to next phase: " << phaseCounter << std::endl;
							//str_data += line;
							phaseCounter++;
							break;
						}
						else
							std::cout << "wating for lines" << std::endl;
						break;
					case 1:
						// data + + + + + + + + + + + + + + + + + + + + + + 
						if (line.substr(0, 2) == "::") {
							std::cout << "moving to next phase: " << phaseCounter << std::endl;

							
							
							phaseCounter++;
							break;
						}
						else
						{
							std::cout << "in data" << std::endl;

							if (line.find("texturePath") != std::string::npos) { // texture
								std::string temp = line.substr(line.find(":") + 1, line.size());
								texturePath = path + temp;


								//std::cout << "texture: " << temp << " | " << textureIndex << std::endl;
							}
							else if (line.find("specularLevel") != std::string::npos) { // specular level
								std::string temp = line.substr(line.find(":") + 1, line.size());
								specularLevel = std::stof(temp);

								//std::cout << "specular level: " << temp << " | " << specularLevel << std::endl;
							}
							else if (line.find("spotLight") != std::string::npos) { // specular level
								std::string temp = line.substr(line.find(":") + 1, line.size());
								spotLighActive =  stob(temp, false);

								//std::cout << "specular level: " << temp << " | " << specularLevel << std::endl;
							}
						}
						break;
					case 2:
						// lights   + + + + + + + + + + + + + + + + + + + + 
						if (line.substr(0, 2) == "::") {
							std::cout << "moving to next phase: " << phaseCounter << std::endl;

							//std::cout << lights[1].lightPosition.x << " - " << lights[1].lightPosition.y << " - " << lights[1].lightPosition.z;

							phaseCounter++;
							break;
						}
						else {
							std::cout << "in lights" << std::endl;

							std::string type = line.substr(0, line.find(";"));

							int _start = line.find("(");
							int _end = line.find(")");

							//std::cout << "start pos: " << _start  << " - " << _end<< std::endl;

							std::string tmpPos = line.substr(_start + 1, (_end - _start) - 1);
							line = line.substr(_end + 1, line.size());

							_start = line.find("(");
							_end = line.find(")");
							std::string tmpColour = line.substr(_start + 1, (_end - _start) - 1);
							line = line.substr(_end + 1, line.size());

							_start = line.find("(");
							_end = line.find(")");
							std::string tmpStrength = line.substr(_start + 1, (_end - _start) - 1);
							line = line.substr(_end + 1, line.size());

							_start = line.find("(");
							_end = line.find(")");
							std::string tmpAmbientStength = line.substr(_start + 1, (_end - _start) - 1);
							line = line.substr(_end + 1, line.size());

							_start = line.find("(");
							_end = line.find(")");
							std::string tmpRadius = line.substr(_start + 1, (_end - _start) - 1);
							
							//std::cout << "AHHHHHHHHHHHH  : " << type.find("directionalLight") << std::endl;

							// setting temp position
							float tmpPosX, tmpPosY, tmpPosZ;
							tmpPosX = std::stof(tmpPos.substr(0, tmpPos.find(",")));
							tmpPos = tmpPos.substr(tmpPos.find(",") + 1, tmpPos.size());

							tmpPosY = std::stof(tmpPos.substr(0, tmpPos.find(",")));
							tmpPos = tmpPos.substr(tmpPos.find(",") + 1, tmpPos.size());

							tmpPosZ = std::stof(tmpPos.substr(0, tmpPos.find(",")));

							//std::cout << "temp pos: " << tmpPosX << std::endl;
							//std::cout << "temp pos: " << tmpPosY << std::endl;
							//std::cout << "temp pos: " << tmpPosZ << std::endl;

							// setting temp colour
							float tmpColR, tmpColG, tmpColB;
							tmpColR = std::stof(tmpColour.substr(0, tmpColour.find(",")));
							tmpColour = tmpColour.substr(tmpColour.find(",") + 1, tmpColour.size());

							tmpColG = std::stof(tmpColour.substr(0, tmpColour.find(",")));
							tmpColour = tmpColour.substr(tmpColour.find(",") + 1, tmpColour.size());

							tmpColB = std::stof(tmpColour.substr(0, tmpColour.find(",")));

							//std::cout << "temp col: " << tmpColR << std::endl;
							//std::cout << "temp col: " << tmpColG << std::endl;
							//std::cout << "temp col: " << tmpColB << std::endl;

							// setting temp strength
							float tmpStr;
							tmpStr = std::stof(tmpStrength);


							float tmpAmbientStr;
							tmpAmbientStr = std::stof(tmpAmbientStength);

							float tmpRadMax;
							tmpRadMax = std::stof(tmpRadius);


							if (type.find("directionalLight") >= 0 && type.find("directionalLight") < 100) {
								std::cout << "directional light" << std::endl;

								dirLight.direction = glm::vec3(tmpPosX, tmpPosY, tmpPosZ);
								dirLight.diffuse = glm::vec3(tmpColR, tmpColG, tmpColB) * tmpStr;
								dirLight.ambient = glm::vec3(tmpColR, tmpColG, tmpColB) * tmpAmbientStr;
								dirLight.specular = glm::vec3(tmpColR, tmpColG, tmpColB) * 0.01f;

							}
							else {
								std::cout << "standard light " << std::endl;

								lightCounter++;

								pointLights[lightCounter].position = glm::vec3(tmpPosX, tmpPosY, tmpPosZ);
								pointLights[lightCounter].diffuse = glm::vec3(tmpColR, tmpColG, tmpColB) * tmpStr;
								pointLights[lightCounter].ambient = glm::vec3(tmpColR, tmpColG, tmpColB) * tmpAmbientStr;
								pointLights[lightCounter].specular = glm::vec3(tmpColR, tmpColG, tmpColB) * specularLevel;
								pointLights[lightCounter].constant = tmpRadMax;

							}
							std::cout << std::endl;						}
						break;
					case 3:
						// level  + + + + + + + + + + + + + + + + + + + + + 

						if (line.substr(0, 2) == "::") {

							std::cout << "File ended" << std::endl;
							phaseCounter++;
							break;
						}
						else {
							std::cout << "in level" << std::endl;

							// interpreting level data
							std::vector<char> tmp = std::vector<char>();
							for (size_t i = 0; i < line.size(); i++)
							{
								tmp.push_back(line.c_str()[i]);

								//std::cout << "This has to mean something: " << tmp[i] << std::endl;
								//std::cout << tmp[i];
							}
							//std::cout << std::endl;

							simple_level_data.push_back(tmp);

						}
						break;
					default:
						std::cout << "End of data" << std::endl;
						break;
					}
				}
				else {
					//std::cout << "comment: " << line << std::endl;
				}
			}
			myfile.close();

			fileRead = true;
		}
		else std::cout << "Unable to open file";

	}

	void setupLights(Shader shader, Camera cam) {
		shader.use();
		// directional light
		shader.setVec3("dirLight.direction", dirLight.direction);
		shader.setVec3("dirLight.ambient", dirLight.ambient);
		shader.setVec3("dirLight.diffuse", dirLight.diffuse);
		shader.setVec3("dirLight.specular", dirLight.specular);
		// point light 1
		shader.setVec3("pointLights[0].position", pointLights[0].position);
		shader.setVec3("pointLights[0].ambient", pointLights[0].ambient);
		shader.setVec3("pointLights[0].diffuse", pointLights[0].diffuse);
		shader.setVec3("pointLights[0].specular", pointLights[0].specular);
		shader.setFloat("pointLights[0].constant", pointLights[0].constant);
		shader.setFloat("pointLights[0].linear", 0.09);
		shader.setFloat("pointLights[0].quadratic", 0.032);
		// point light 2
		shader.setVec3("pointLights[1].position", pointLights[1].position);
		shader.setVec3("pointLights[1].ambient", pointLights[1].ambient);
		shader.setVec3("pointLights[1].diffuse", pointLights[1].diffuse);
		shader.setVec3("pointLights[1].specular", pointLights[1].specular);
		shader.setFloat("pointLights[1].constant", pointLights[1].constant);
		shader.setFloat("pointLights[1].linear", 0.09);
		shader.setFloat("pointLights[1].quadratic", 0.032);
		// point light 3
		shader.setVec3("pointLights[2].position", pointLights[2].position);
		shader.setVec3("pointLights[2].ambient", pointLights[2].ambient);
		shader.setVec3("pointLights[2].diffuse", pointLights[2].diffuse);
		shader.setVec3("pointLights[2].specular", pointLights[2].specular);
		shader.setFloat("pointLights[2].constant", pointLights[2].constant);
		shader.setFloat("pointLights[2].linear", 0.09);
		shader.setFloat("pointLights[2].quadratic", 0.032);
		// point light 4
		shader.setVec3("pointLights[3].position", pointLights[3].position);
		shader.setVec3("pointLights[3].ambient", pointLights[3].ambient);
		shader.setVec3("pointLights[3].diffuse", pointLights[3].diffuse);
		shader.setVec3("pointLights[3].specular", pointLights[3].specular);
		shader.setFloat("pointLights[3].constant", pointLights[3].constant);
		shader.setFloat("pointLights[3].linear", 1);
		shader.setFloat("pointLights[3].quadratic", 0.032);
		// spotLight
		//shader.setVec3("spotLight.position", { 0.05f, 0.05f, 0.05f });
		//shader.setVec3("spotLight.direction", { 0.05f, 0.05f, 0.05f });
		if (spotLighActive) {

		shader.setVec3("spotLight.position", cam.Position);
		shader.setVec3("spotLight.direction", cam.Front);
		shader.setVec3("spotLight.ambient", { 0.1f, 0.1f, 0.0f });
		shader.setVec3("spotLight.diffuse", { 1.0f, 1.0f, 1.0f });
		//shader.setVec3("spotLight.diffuse", { 0.3f, 0.3f, 0.0f });
		//shader.setVec3("spotLight.specular", { 0.3f, 0.3f, 0.3f });
		shader.setVec3("spotLight.specular", { 0.0f, 0.0f, 0.0f });
		shader.setFloat("spotLight.constant", 1.0f);
		shader.setFloat("spotLight.linear", 0.09);
		shader.setFloat("spotLight.quadratic", 0.032);
		shader.setFloat("spotLight.cutOff", glm::cos(glm::radians(12.5f)));
		shader.setFloat("spotLight.outerCutOff", glm::cos(glm::radians(35.0f)));
		}
		else {
			shader.setVec3("spotLight.position", cam.Position);
			shader.setVec3("spotLight.direction", cam.Front);
			shader.setVec3("spotLight.ambient", { 0.0f, 0.0f, 0.0f });
			shader.setVec3("spotLight.diffuse", { 0.0f, 0.0f, 0.0f });
			//shader.setVec3("spotLight.diffuse", { 0.3f, 0.3f, 0.0f });
			//shader.setVec3("spotLight.specular", { 0.3f, 0.3f, 0.3f });
			shader.setVec3("spotLight.specular", { 0.0f, 0.0f, 0.0f });
			shader.setFloat("spotLight.constant", 1.0f);
			shader.setFloat("spotLight.linear", 0.09);
			shader.setFloat("spotLight.quadratic", 0.032);
			shader.setFloat("spotLight.cutOff", glm::cos(glm::radians(12.5f)));
			shader.setFloat("spotLight.outerCutOff", glm::cos(glm::radians(30.0f)));
		}
	}
	bool stob(std::string s, bool throw_on_error)
	{
		auto result = false;    // failure to assert is false

		std::istringstream is(s);
		// first try simple integer conversion
		is >> result;

		if (is.fail())
		{
			// simple integer failed; try boolean
			is.clear();
			is >> std::boolalpha >> result;
		}

		if (is.fail())
		{
			s.append(" is not convertable to bool");
			std::cout << s;
			if (throw_on_error)
				throw std::invalid_argument(s);
		}

		return result;
	}

};
