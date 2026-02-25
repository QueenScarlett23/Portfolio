// ClassWork.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <glad/glad.h> 
#include <GLFW/glfw3.h>

//#include <ft2build.h>
//#include FT_FREETYPE_H  

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

// console stuff
#include <windows.h>
#include <filesystem>

#include <chrono>
#include <thread>
#include <vector>
#include <queue>
#include <vector>
#include <map>

#include <iostream>
#include <filesystem>


#include "shader.h"
#define STB_IMAGE_IMPLEMENTATION
//#include "stb_image.h"
#include "Level.h"
#include "Camera.h"
#include "Model.h"

using namespace std;

// delcaration of methods -------------------------------------------------------------------------------------------
int main();
void framebuffer_size_callback(GLFWwindow* window, int width, int height);
void mouse_callback(GLFWwindow* window, double xpos, double ypos);
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);

unsigned int loadCubemap(vector<std::string> faces);

void key_callback(GLFWwindow* window, int key, int scancode, int action, int mods);
void character_callback(GLFWwindow* window, unsigned int codepoint);
void RenderText(Shader& shader, std::string text, float x, float y, float scale, glm::vec3 color);
string ExecuteCommand();
vector<string> split(string x, char delim = ' ');
glm::vec3 toPosition(string s);
bool stob(std::string s, bool throw_on_error = true);
void UpdateTextDisplay();
boolean loadSkyBox(std::string strSkybox);

void processInput(GLFWwindow* window);
unsigned int loadTexture(char const* path);

void changeColour(int i = -1);
void ChangeBlendColour();
void SetUseTime();
void Update();

boolean levelGenerating = false;


// global varibles
GLFWwindow* window;
Shader shader;
Shader lightShader;
Shader skyboxShader;
const char Console_key = GLFW_KEY_GRAVE_ACCENT;

const string Console_name = "Engine_Console";

// DATA -------------------------------------------------------------------------------------

#pragma region Console
boolean skyBoxLoaded = false;
float modelScale = 0.25;
float levelScale = 1;

string previousCommands = "";

bool console_enabled = false;
bool console_typing = false;
bool display_trianglecount = false;
bool display_fps = false;
bool display_updates = false;

string command_line;

long vertices_count;

string display;

vector<Model> modelList;
#pragma endregion

#pragma region FPS
static double limitFPS = 1.0 / 120.0;
const unsigned int SCR_WIDTH = 1200;
const unsigned int SCR_HEIGHT = 800;
#pragma endregion

#pragma region Camera
glm::vec4 current(0.3, 0.5, 0.4, 1.0); // background colour

Camera camera;

bool firstMouse = true;
float lastX;
float lastY;
#pragma endregion

#pragma region Time
double lastTime = glfwGetTime(), timer = lastTime;
double deltaTime = 0, nowTime = 0;
int frames = 0, updates = 0;
#pragma endregion

#pragma region Other
Level level;

string textureDir = "resources/textures/";

struct Texture_str {
	int width, height, nrChannels;
	unsigned char* image_data;
	unsigned int texture;
	int index;
	// , int widt, int heigh, int nrChannel
	Texture_str(string textureName, int ind)
	{
		index = ind;

		/*width = widt;
		height = heigh;

		nrChannels = nrChannel;*/

		image_data = stbi_load((textureDir + textureName).c_str(), &width, &height, &nrChannels, 0);
		// STBI_rgb_alpha.


		// generating textures
		glGenTextures(index, &texture);
		if (image_data)
		{
			glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, image_data);
			glGenerateMipmap(GL_TEXTURE_2D);
		}
		else
		{
			std::cout << "Failed to load texture" << endl;
		}
		stbi_image_free(image_data);
		glUniform1f(glGetUniformLocation(shader.ID, "ourTexture"), texture);
	}
};

/// Holds all state information relevant to a character as loaded using FreeType
struct Character {
	unsigned int TextureID; // ID handle of the glyph texture
	glm::ivec2   Size;      // Size of glyph
	glm::ivec2   Bearing;   // Offset from baseline to left/top of glyph
	unsigned int Advance;   // Horizontal offset to advance to next glyph
};

std::map<GLchar, Character> Characters;
unsigned int VAO_text, VBO_text;

#pragma endregion

// TO BE REMOVED ---------------------------------------------------------------------------------


int main()
{
	float skyboxVertices[] = {
		// positions          
		-1.0f,  1.0f, -1.0f,
		-1.0f, -1.0f, -1.0f,
		 1.0f, -1.0f, -1.0f,
		 1.0f, -1.0f, -1.0f,
		 1.0f,  1.0f, -1.0f,
		-1.0f,  1.0f, -1.0f,

		-1.0f, -1.0f,  1.0f,
		-1.0f, -1.0f, -1.0f,
		-1.0f,  1.0f, -1.0f,
		-1.0f,  1.0f, -1.0f,
		-1.0f,  1.0f,  1.0f,
		-1.0f, -1.0f,  1.0f,

		 1.0f, -1.0f, -1.0f,
		 1.0f, -1.0f,  1.0f,
		 1.0f,  1.0f,  1.0f,
		 1.0f,  1.0f,  1.0f,
		 1.0f,  1.0f, -1.0f,
		 1.0f, -1.0f, -1.0f,

		-1.0f, -1.0f,  1.0f,
		-1.0f,  1.0f,  1.0f,
		 1.0f,  1.0f,  1.0f,
		 1.0f,  1.0f,  1.0f,
		 1.0f, -1.0f,  1.0f,
		-1.0f, -1.0f,  1.0f,

		-1.0f,  1.0f, -1.0f,
		 1.0f,  1.0f, -1.0f,
		 1.0f,  1.0f,  1.0f,
		 1.0f,  1.0f,  1.0f,
		-1.0f,  1.0f,  1.0f,
		-1.0f,  1.0f, -1.0f,

		-1.0f, -1.0f, -1.0f,
		-1.0f, -1.0f,  1.0f,
		 1.0f, -1.0f, -1.0f,
		 1.0f, -1.0f, -1.0f,
		-1.0f, -1.0f,  1.0f,
		 1.0f, -1.0f,  1.0f
	};

#pragma region Level

	while (!levelGenerated && !fileRead) {
		try
		{
			cout << "\nCHOOSE LEVEL DIRECTRY\nType directry of level or \"default\" \n(for directry make user of / instead of \\ when typing directry - thank you)\n ";
			std::string strLevelPath;
			cin >> strLevelPath;

			if (strLevelPath == "default") {
				cout << "reading level\n resources/Wolfenstein/level.txt\n";
				level.ReadData();
			}
			else
			{
				cout << "reading level\n" + strLevelPath + "\n";
				level.ReadData(strLevelPath);
			}
			if (fileRead) {
				cout << "\ngenerating level\n";
				level.generateLevel();
				cout << "\n LEVEL GENERATED\n" << std::endl << std::endl;
			}
		}
		catch (const std::exception& e)
		{
			std::cout << std::endl << "\nUnable to read file. Try again\n" << std::endl << std::endl;
		}
	}

	cout << "CHOOSE SKYBOX DIRECTRY\nType directry of skybox or \"default\" \n(for directry make user of / instead of \\ when typing directry - thank you)\n ";
	std::string strSkybox;
	cin >> strSkybox;
	if (strSkybox == "default")
		strSkybox = "resources/textures/skybox/";

#pragma endregion
#pragma region creating window
	glfwInit();
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

	// making wondow 
	window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "Boop", NULL, NULL);
	if (window == NULL)
	{
		std::cout << "Failed to create GLFW window" << endl;
		glfwTerminate();
		return -1;
	}

	glfwMakeContextCurrent(window);
	glfwSetFramebufferSizeCallback(window, framebuffer_size_callback); // when screen changes size
	glfwSetCursorPosCallback(window, mouse_callback); // when curser moves
	glfwSetScrollCallback(window, scroll_callback); // when scroll whele is used

	// glad
	if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
	{
		std::cout << "Failed to initialize GLAD" << endl << endl;
		return -1;
	}

	//viewport
	glViewport(0, 0, SCR_WIDTH, SCR_HEIGHT);

	void framebuffer_size_callback(GLFWwindow * window, int width, int height);

#pragma endregion
#pragma region Text
	/*
	// compile and setup the shader
	// ----------------------------
	Shader text_shader("TextShader.vert", "TextShader.frag");
	glm::mat4 text_projection = glm::ortho(0.0f, static_cast<float>(SCR_WIDTH), 0.0f, static_cast<float>(SCR_HEIGHT));
	shader.use();
	glUniformMatrix4fv(glGetUniformLocation(shader.ID, "projection"), 1, GL_FALSE, glm::value_ptr(text_projection));

	// FreeType
	// --------
	FT_Library ft;
	// All functions return a value different than 0 whenever an error occurred
	if (FT_Init_FreeType(&ft))
	{
		std::cout << "ERROR::FREETYPE: Could not init FreeType Library" << std::endl;
		return -1;
	}

	// find path to font
	std::string font_name = "resources/fonts/Antonio-Bold.ttf";
	if (font_name.empty())
	{
		std::cout << "ERROR::FREETYPE: Failed to load font_name" << std::endl;
		return -1;
	}

	// load font as face
	FT_Face face;
	if (FT_New_Face(ft, font_name.c_str(), 0, &face)) {
		std::cout << "ERROR::FREETYPE: Failed to load font" << std::endl;
		return -1;
	}
	else {
		// set size to load glyphs as
		FT_Set_Pixel_Sizes(face, 0, 48);

		// disable byte-alignment restriction
		glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

		// load first 128 characters of ASCII set
		for (unsigned char c = 0; c < 128; c++)
		{
			// Load character glyph
			if (FT_Load_Char(face, c, FT_LOAD_RENDER))
			{
				std::cout << "ERROR::FREETYTPE: Failed to load Glyph" << std::endl;
				continue;
			}
			// generate texture
			unsigned int texture;
			glGenTextures(1, &texture);
			glBindTexture(GL_TEXTURE_2D, texture);
			glTexImage2D(
				GL_TEXTURE_2D,
				0,
				GL_RED,
				face->glyph->bitmap.width,
				face->glyph->bitmap.rows,
				0,
				GL_RED,
				GL_UNSIGNED_BYTE,
				face->glyph->bitmap.buffer
			);
			// set texture options
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
			// now store character for later use
			Character character = {
				texture,
				glm::ivec2(face->glyph->bitmap.width, face->glyph->bitmap.rows),
				glm::ivec2(face->glyph->bitmap_left, face->glyph->bitmap_top),
				static_cast<unsigned int>(face->glyph->advance.x)
			};
			Characters.insert(std::pair<char, Character>(c, character));
		}
		glBindTexture(GL_TEXTURE_2D, 0);
	}
	// destroy FreeType once we're finished
	FT_Done_Face(face);
	FT_Done_FreeType(ft);


	// configure VAO/VBO for texture quads
	// -----------------------------------
	glGenVertexArrays(1, &VAO_text);
	glGenBuffers(1, &VBO_text);
	glBindVertexArray(VAO_text);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_text);
	glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 6 * 4, NULL, GL_DYNAMIC_DRAW);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(0, 4, GL_FLOAT, GL_FALSE, 4 * sizeof(float), 0);
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindVertexArray(0);
*/
#pragma endregion

#pragma region gl buffers
	unsigned int VBO, cubeVAO;
	glGenVertexArrays(1, &cubeVAO);
	glGenBuffers(1, &VBO);

	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertex_cube), vertex_cube, GL_STATIC_DRAW);

	glBindVertexArray(cubeVAO);

	const int vertexsize = 8;
	/*
	// position attribute
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	// texture atributes
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);
	// nomals
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)(5 * sizeof(float)));
	glEnableVertexAttribArray(2);

	// second, configure the light's VAO (VBO stays the same; the vertices are the same for the light object which is also a 3D cube)
	unsigned int lightCubeVAO;
	glGenVertexArrays(1, &lightCubeVAO);
	glBindVertexArray(lightCubeVAO);

	// we only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need (it's already bound, but we do it again for educational purposes)
	glBindBuffer(GL_ARRAY_BUFFER, VBO);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	*/
	/*
	// walllll + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +
	unsigned int VBO_door, VAO_door;
	glGenVertexArrays(1, &VAO_door);
	glGenBuffers(1, &VBO_door);

	glBindBuffer(GL_ARRAY_BUFFER, VBO_door);
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertex_door), vertex_door, GL_STATIC_DRAW);

	glBindVertexArray(VAO_door);

	// position attribute
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	// texture atributes
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);
	// nomals
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)(5 * sizeof(float)));
	glEnableVertexAttribArray(2);*/
	// skybox VAO
	unsigned int skyboxVAO, skyboxVBO;
	glGenVertexArrays(1, &skyboxVAO);
	glGenBuffers(1, &skyboxVBO);
	glBindVertexArray(skyboxVAO);
	glBindBuffer(GL_ARRAY_BUFFER, skyboxVBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(skyboxVertices), &skyboxVertices, GL_STATIC_DRAW);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);

	// Level + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + 
	unsigned int VBO_level, VAO_level;
	glGenVertexArrays(1, &VAO_level);
	glGenBuffers(1, &VBO_level);

	glBindBuffer(GL_ARRAY_BUFFER, VBO_level);
	glBufferData(GL_ARRAY_BUFFER, level_size, level_vertex_data, GL_STATIC_DRAW);

	glBindVertexArray(VAO_level);

	// position attribute
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	// texture atributes
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);
	// nomals
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, vertexsize * sizeof(float), (void*)(5 * sizeof(float)));
	glEnableVertexAttribArray(2);
	// elnd level + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + 

#pragma endregion
#pragma region shader stuff
	shader = Shader("shader.vert", "shader.frag");
	lightShader = Shader("shader_light.vert", "shader_light.frag");
	skyboxShader = Shader("skybox.vert", "skybox.frag");

	// texture
	// texture setup
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_MIRRORED_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_MIRRORED_REPEAT);

	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

	stbi_set_flip_vertically_on_load(true);

	// texture loading
	/*string texNames[] = {
	"bamboo.jpg", // 0
	"checkered.jpg",// 1
	"detailed_stone.jpg",//2
	"fancy.png",// 3
	"grass.jpg",// 4
	"image.jpg",// 5
	"red_brick.jpg",// 6
	"static.png",// 7
	"steel.jpg",// 8
	"stone_brick.jpg",// 9
	"container2.png" // 10
	};

	Texture_str t(texNames[1], 1); // default texture

	if (textureIndex >= 0 && textureIndex < texNames->size()) {
		t = Texture_str(texNames[textureIndex], 1);
	}

	glBindTexture(GL_TEXTURE_2D, t.texture);
	*/
	//shader.setInt("ourTexture", 1);

	// setting texture

	// shader configuration
	// --------------------
	shader.use();
	unsigned int diffuseMap = loadTexture(level.texturePath.c_str());
	shader.setInt("material.diffuse", 0);
	unsigned int specularMap = loadTexture(level.texturePath.c_str());
	shader.setInt("material.specular", 1);

	cout << level.texturePath << endl;

	// sky box []  []  []  []  []  []  []  []  []  []  []  []  []  []  []  []  []  []  []  []  []  []
	stbi_set_flip_vertically_on_load(false);
	vector<std::string> faces
	{
		"right.jpg",
		"left.jpg",
		"top.jpg",
		"bottom.jpg",
		"front.jpg",
		"back.jpg"
	};

	unsigned int cubemapTexture;

	for (int i = 0; i < faces.size(); i++)
	{
		faces[i] = strSkybox + faces[i];
	}
	cubemapTexture = loadCubemap(faces);


	skyboxShader.use();
	skyboxShader.setInt("skybox", 0);

	stbi_set_flip_vertically_on_load(true);

#pragma endregion

	// enabling depth buffers
	glEnable(GL_DEPTH_TEST);
	//glDepthFunc(GL_ALWAYS);
	//glDepthMask(GL_FALSE);

	//glEnable(GL_CULL_FACE);
	//glCullFace(GL_BACK);
	//glFrontFace(GL_CW);
	//glCullFace(GL_FRONT);

	glEnable(GL_BLEND);

	glEnable(GL_MULTISAMPLE);
	glfwWindowHint(GLFW_SAMPLES, 4);

	glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

	// clear console
	//system("cls");
	SetConsoleTitle(TEXT("Engine_Console"));

	TCHAR buffer[MAX_PATH] = { 0 };
	GetModuleFileName(NULL, buffer, MAX_PATH);
	string::size_type pos = wstring(buffer).find_last_of(L"\\/");
	wstring dir = wstring(buffer).substr(0, pos);
	string dir_str(dir.begin(), dir.end());

	std::cout << dir_str << endl;
	std::cout << "Console: \' " << Console_key << " \'" << endl;
	string str_console_command;

	// draws wireframe
	//glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);

	// setting key callback method
	glfwSetKeyCallback(window, key_callback);
	glfwSetCharCallback(window, character_callback);


	glfwSetInputMode(window, GLFW_LOCK_KEY_MODS, GLFW_TRUE);

	//camera = Camera();


#pragma region shaddow maps
	/*unsigned int shaddowMapFBO;
	glGenFramebuffers(1, &shaddowMapFBO);
	const unsigned int SHADOW_WIDTH = 1024, SHADOW_HEIGHT = 1024;

	unsigned int depthMap;
	glGenTextures(1, &depthMap);
	glBindTexture(GL_TEXTURE_2D, depthMap);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_DEPTH_COMPONENT,
		SHADOW_WIDTH, SHADOW_HEIGHT, 0, GL_DEPTH_COMPONENT, GL_FLOAT, NULL);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_BORDER);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_BORDER);

	float clampsColour[] = { 1.0f, 1.0f, 1.0f, 1.0f };
	glTexParameterfv(GL_TEXTURE_2D, GL_TEXTURE_BORDER_COLOR, clampsColour);

	glBindFramebuffer(GL_FRAMEBUFFER, shaddowMapFBO);
	glFramebufferTexture2D(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, depthMap, 0);
	glDrawBuffer(GL_NONE);
	glReadBuffer(GL_NONE);
	glBindFramebuffer(GL_FRAMEBUFFER, 0);


	glm::mat4 orthagonalProgection = glm::ortho(-35.0f, 35.0f, -35.0f, 35.0f, 0.1f, 75.0f);
	glm::mat4 lightView = glm::lookAt(20.0 * );*/
#pragma endregion
	// while window is open
	//----------------------------------------------------------------------------------------------------------------
	while (!glfwWindowShouldClose(window))
	{
		// clearing depth buffers
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		vertices_count = level_size / 8;
#pragma region Text
		//RenderText(text_shader, "This is sample text", 25.0f, 25.0f, 1.0f, glm::vec3(0.5, 0.8f, 0.2f));
		//RenderText(text_shader, "(C) LearnOpenGL.com", 540.0f, 570.0f, 0.5f, glm::vec3(0.3, 0.7f, 0.9f));

#pragma endregion
#pragma region Camera
		glm::mat4 view = camera.GetViewMatrix();
		glm::mat4 projection = glm::perspective(glm::radians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);

		shader.use();
		shader.setMat4("view", view);
		shader.setMat4("projection", projection);
		shader.setVec3("viewPos", camera.Position);

#pragma endregion
#pragma region Time + FPS
		// - Measure time
		nowTime = glfwGetTime();
		deltaTime += (nowTime - lastTime) / limitFPS;
		lastTime = nowTime;
#pragma endregion

		// rendering 
		glClearColor(current.r, current.g, current.b, current.a); // background colour
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

#pragma region lights <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) <) 
		/*glm::vec3 lightColor(1.0);
		lightColor.x = sin(glfwGetTime() * 2.0f);
		lightColor.y = sin(glfwGetTime() * 0.7f);
		lightColor.z = sin(glfwGetTime() * 1.3f);
		glm::vec3 diffuseColor = lightColor * glm::vec3(0.4f);
		glm::vec3 ambientColor = diffuseColor * glm::vec3(0.4f);
		shader.setVec3("light.ambient", ambientColor);
		shader.setVec3("light.diffuse", diffuseColor);
		shader.setVec3("light.specular", glm::vec3(1.0));*/

		// be sure to activate shader when setting uniforms/drawing objects
		level.setupLights(shader, camera);

		/*glm::vec3 position = glm::vec3(0, 0, -4);
		shader.use();
		shader.setVec3("light.position", position);

		lightShader.use();
		lightShader.setMat4("view", camera.GetViewMatrix());
		lightShader.setMat4("projection", projection);


		glm::mat4 model = glm::translate(glm::mat4(1.0f), position);
		model = glm::scale(model, glm::vec3(0.2f));
		lightShader.setMat4("model", model);

		glBindVertexArray(lightCubeVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);*/

		/*// ambient light
		shader.setVec3("ambientLight", ambientLight.lightColour * ambientLight.strength);
		// normal lights
		shader.setVec3("lightPos1", lights[0].lightPosition);
		shader.setVec3("lightColour1", lights[0].lightColour * lights[0].strength);

		shader.setVec3("lightPos2", lights[1].lightPosition);
		shader.setVec3("lightColour2", lights[1].lightColour * lights[1].strength);

		shader.setVec3("lightPos3", lights[2].lightPosition);
		shader.setVec3("lightColour3", lights[2].lightColour * lights[2].strength);
		// setting specular level
		shader.setFloat("specularLevel", specularLevel);
		shader.setVec3("viewPos", camera.Position);

		// drawing lights
		lightShader.use();
		lightShader.setMat4("view", camera.GetViewMatrix());
		lightShader.setMat4("projection", projection);
		for (int i = 0; i < 3; i++)
		{
			glm::mat4 model = glm::translate(glm::mat4(1.0f), lights[i].lightPosition);
			model = glm::scale(model, glm::vec3(0.2f));
			lightShader.setMat4("model", model);

			glBindVertexArray(lightCubeVAO);
			glDrawArrays(GL_TRIANGLES, 0, 36);
		}*/
#pragma endregion
#pragma region drawing level

		// material properties
		shader.use();
		shader.setFloat("material.shininess", level.specularLevel);

		// bind diffuse map
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, diffuseMap);
		// bind specular map
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, specularMap);

		// view/projection transformations
		projection = glm::perspective(glm::radians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
		view = camera.GetViewMatrix();
		shader.setMat4("projection", projection);
		shader.setMat4("view", view);

		// world transformation
		glm::mat4 model = glm::mat4(1.0f);
		shader.setMat4("model", model);

		// bind diffuse map
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, diffuseMap);
		// bind specular map
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, specularMap);

		// render containers
		/*glBindVertexArray(cubeVAO);
		for (unsigned int i = 0; i < 10; i++)
		{
			// calculate the model matrix for each object and pass it to shader before drawing
			glm::mat4 model = glm::mat4(1.0f);
			model = glm::translate(model, cubePositions[i]);
			float angle = 20.0f * i;
			model = glm::rotate(model, glm::radians(angle), glm::vec3(1.0f, 0.3f, 0.5f));
			shader.setMat4("model", model);

			glDrawArrays(GL_TRIANGLES, 0, 36);
			vertices_count += 36;
		}*/

		/*glBindVertexArray(VAO_level);
		model = glm::mat4(1.0f);
		//model = glm::translate(model, glm::vec3(3.0f, 0.0f, 0.0f)); // translate it down so it's at the center of the scene
		//model = glm::scale(model, glm::vec3(0.4));	// it's a bit too big for our scene, so scale it down
		shader.setMat4("model", model);
		glDrawArrays(GL_TRIANGLES, 0, level_size / 8);*/
		//level.Draw(shader);

		/*model = glm::mat4(1.0f);
		model = glm::translate(model, glm::vec3(3.0f, 0.0f, 0.0f)); // translate it down so it's at the center of the scene
		model = glm::scale(model, glm::vec3(0.4));	// it's a bit too big for our scene, so scale it down
		shader.setMat4("model", model);
		*/

		shader.use();

		model = glm::mat4(1.0f);
		model = glm::scale(model, glm::vec3(levelScale));
		shader.setBool("isLight", false);
		shader.setMat4("model", model);
		//shader.setInt("material.diffuse", 0);
		//shader.setInt("material.specular", 1);
		//shader.setInt("material.shininess", 1);

		glBindVertexArray(VAO_level);
		glDrawArrays(GL_TRIANGLES, 0, level_size / 8);

		/*
		// material properties
		shader.use();
		shader.setVec3("material.ambient", { 1.0f, 0.5f, 0.31f });
		shader.setVec3("material.diffuse", { 1.0f, 0.5f, 0.31f });
		shader.setVec3("material.specular", glm::vec3(0.5));
		shader.setFloat("material.shininess", 50.0f);

		// world transformation
		glm::vec3 position = glm::vec3(0);
		model = glm::translate(glm::mat4(1.0), position);
		shader.setMat4("model", model);

		// bind diffuse map
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, diffuseMap);
		// bind specular map
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, specularMap);

		// render the cube
		glBindVertexArray(cubeVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		//std::cout << level_size << endl;*/
		shader.use();

		if (modelList.size() > 0)
			for (int i = 0; i < modelList.size(); i++)
			{
				model = glm::mat4(1.0f);
				model = glm::translate(model, modelList[i].position); // translate it down so it's at the center of the scene
				model = glm::scale(model, modelList[i].scale * modelScale);	// it's a bit too big for our scene, so scale it down
				// rotation
				model = glm::rotate(model, glm::radians(modelList[i].rotation.x), glm::vec3(1.0f, 0.0f, 0.0f));
				model = glm::rotate(model, glm::radians(modelList[i].rotation.y), glm::vec3(0.0f, 1.0f, 0.0f));
				model = glm::rotate(model, glm::radians(modelList[i].rotation.z), glm::vec3(0.0f, 0.0f, 1.0f));

				shader.setMat4("model", model);
				vertices_count += modelList[i].Draw(shader);
			}
		/*
		// drawing all triangles  /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\
		for (unsigned int i = 0; i < 10; i++)
		{
			glm::mat4 model = glm::mat4(1.0f);
			model = glm::translate(model, cubePositions[i] * 2.0f);
			float angle = 20.0f * i;
			model = glm::rotate(model, glm::radians((float)glfwGetTime() * 5.0f), glm::vec3(1.0f, 0.3f, 0.5f));
			model = glm::rotate(model, glm::radians(angle), glm::vec3(1.0f, 0.3f, 0.5f));
			shader.setBool("isLight", false);
			shader.setMat4("model", model);

			glBindVertexArray(cubeVAO);
			int s = sizeof(vertex_cube) / 8;
			glDrawArrays(GL_TRIANGLES, 0, s);
		}

		// WALLLLLLLLLLLLLLLLLLLLLLLLLL	--------------------------
		glm::mat4 model = glm::mat4(1.0f);
		shader.setBool("isLight", false);
		shader.setMat4("model", model);

		glBindVertexArray(VAO_door);
		glDrawArrays(GL_TRIANGLES, 0, sizeof(vertex_door) / 8);
		// -------------------------------------------------------
		*/
#pragma endregion

		glDepthFunc(GL_LEQUAL);  // change depth function so depth test passes when values are equal to depth buffer's content
		skyboxShader.use();
		view = glm::mat4(glm::mat3(camera.GetViewMatrix())); // remove translation from the view matrix
		skyboxShader.setMat4("view", view);
		skyboxShader.setMat4("projection", projection);
		// skybox cube
		glBindVertexArray(skyboxVAO);
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_CUBE_MAP, cubemapTexture);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);
		glDepthFunc(GL_LESS); // set depth function back to default


		Update();   // - Update function

		frames++;
#pragma region Display Info
		// - Reset after one second
		if (glfwGetTime() - timer > 1.0) {
			timer++;
			if (!console_typing) {

				display = "||";
				if (display_fps)
					display += "FPS: " + to_string(frames) + "\t||";
				if (display_updates)
					display += "Updates: " + to_string(updates) + "\t||";
				if (display_trianglecount)
					display += "Traingle count: " + to_string(vertices_count / 3) + "\t||";
			}
			UpdateTextDisplay();
			string tempPos = to_string(camera.Position.x) + ", " + to_string(camera.Position.y) + ", " + to_string(camera.Position.z);
			glfwSetWindowTitle(window, tempPos.c_str());

			updates = 0, frames = 0;
		}
#pragma endregion
		// swap buffers and call all envents
		glfwSwapBuffers(window);
		glfwPollEvents();

		//std::cout << "[END OF FRAME]" << endl << endl;
	// END OF FRAME --------------------------------------------------------------------------------------------------------------
	}
	//----------------------------------------------------------------------------------------------------------------

	// Freeing up memory
	glDeleteVertexArrays(1, &cubeVAO);
	//glDeleteVertexArrays(1, &lightCubeVAO);
	glDeleteBuffers(1, &VBO);

	glDeleteVertexArrays(1, &skyboxVAO);
	glDeleteBuffers(1, &skyboxVBO);

	glfwTerminate();
	return 0;
}


void mouse_callback(GLFWwindow* window, double xpos, double ypos)
{
	if (firstMouse)
	{
		lastX = xpos;
		lastY = ypos;
		firstMouse = false;
	}


	float xoffset = xpos - lastX;
	float yoffset = lastY - ypos;
	lastX = xpos;
	lastY = ypos;

	float sensitivity = 0.1f;
	xoffset *= sensitivity;
	yoffset *= sensitivity;

	camera.ProcessMouseMovement(xoffset, yoffset, true);
}

void framebuffer_size_callback(GLFWwindow* window, int width, int height)
{
	glViewport(0, 0, width, height);
	//projection = glm::perspective(glm::radians(camera.Zoom), (float)width / (float)height, 0.1f, 100.0f);
}

void scroll_callback(GLFWwindow* window, double xoffset, double yoffset)
{
	camera.ProcessMouseScroll(yoffset);
}

vector<int> key_press;
vector<int> key_hold;
vector<int> key_release;

void key_callback(GLFWwindow* window, int key, int scancode, int action, int mods)
{
	if (action == GLFW_PRESS || action == GLFW_RELEASE) {
		UpdateTextDisplay();
	}
	if (key == Console_key) {
		if (action == GLFW_PRESS) {

			if (!console_enabled) {
				console_enabled = true;
			}
			else {
				if (command_line.size() > 0)
					printf("\33[2K\r");
				console_enabled = false;
			}
			/*
			// sets console to front
			HWND console_window = ::FindWindow(NULL, L"Engine_Console");
			if (console_window) {
				// move to foreground
				::SetForegroundWindow(console_window);
			}

			// sets to front
			HWND game_window = ::FindWindow(NULL, L"Boop");
			if (game_window) {
				// move to foreground
				::SetForegroundWindow(game_window);
			}*/
		}
	}
	else if (console_enabled)
	{
		if (action == GLFW_PRESS) {

			if (key == GLFW_KEY_ENTER) {
				console_enabled = false;
				console_typing = false;
				previousCommands += command_line;
				if (command_line.size() > 0)
					previousCommands += "\n-> " + ExecuteCommand() + "\n\n";
				return;
			}
			if (key == GLFW_KEY_BACKSPACE) {
				if (command_line.size() > 0) {
					printf("\33[2K\r");
					command_line.resize(command_line.size() - 1);
				}
				return;
			}
		}
	}
	/*
	if (action == GLFW_PRESS) {
		key_press.push_back(key);
		cout << static_cast<char>(key);
	}
	if (action == GLFW_RELEASE) {
		key_release.push_back(key);
		cout << " relesed:" + static_cast<char>(key);*/
		/*for (auto i = key_press.begin(); i != key_press.end(); ++i) {

			if (key_press[i - key_press.begin()] == key) {
				key_press.erase(i);
				i--;
			}
		}

		for (auto i = key_hold.begin(); i != key_hold.end(); ++i) {

			if (key_hold[i - key_hold.begin()] == key) {
				key_hold.erase(i);
				i--;
			}
		}*/
}

void character_callback(GLFWwindow* window, unsigned int codepoint)
{
	UpdateTextDisplay();
	if (codepoint != Console_key)
		if (console_enabled) {
			console_typing = true;
			char c = static_cast<char>(codepoint);
			command_line += c;
		}
}
void UpdateTextDisplay() {
	// display console stuff
	string dis = string(50, '\n');;
	dis += previousCommands + "\n";
	dis += display + "\n";
	if (console_enabled) {
		dis += "type \"help\" for a list of commands\n(for directry make user of / instead of \\ when typing directry - thank you)\nConsole active : \n";
		dis += command_line;
	}
	else {
		dis += "Console: \'`\'";
	}
	//printf("\33[2K\r");
	//system("cls");

	cout << dis;
}
map<string, string> default_models = {
	{"backpack", "resources/models/backpack/backpack.obj"},
	{"plant", "resources/models/plant/Low-PolyPlant_.obj"}
};
string ExecuteCommand() {
	try
	{
		vector<string> command = split(command_line);
		command_line = "";
		transform(command[0].begin(), command[0].end(), command[0].begin(), ::tolower);

		if (command[0] == "model_load") {
			if (default_models.find(command[1]) != default_models.end()) {
				command[1] = default_models[command[1]];
			}

			if (command.size() == 3) {
				modelList.push_back(Model(command[1], toPosition(command[2])));
			}
			else if (command.size() == 4) {
				modelList.push_back(Model(command[1], toPosition(command[2]), stof(command[3])));
			}
			else if (command.size() == 5) {
				modelList.push_back(Model(command[1], toPosition(command[2]), stof(command[3]), toPosition(command[4])));
			}
			else {
				modelList.push_back(Model(command[1]));
			}

			return "Loaded model executed:" + command[1];
		}

		if (command[0] == "set_model_scale") {
			modelScale = stof(command[1]);
			return "set model scale: " + to_string(modelScale);
		}

		if (command[0] == "set_level_scale") {
			levelScale = stof(command[1]);
			return "set level scale: " + to_string(levelScale);
		}

		if (command[0] == "show_fps") {
			bool b;
			b = stob(command[1]);
			display_fps = b;
			string bstr = b ? "true" : "false";
			return "Show FPS: " + bstr;
		}

		if (command[0] == "show_updates") {
			bool b;
			b = stob(command[1]);
			display_updates = b;
			string bstr = b ? "true" : "false";
			return "Show Updates: " + bstr;
		}

		if (command[0] == "show_trianglecount") {
			bool b;
			b = stob(command[1]);
			display_trianglecount = b;
			string bstr = b ? "true" : "false";
			return "Show Triangle Count: " + bstr;
		}

		if (command[0] == "show_all") {
			bool b;
			b = stob(command[1]);
			display_fps = b;
			display_updates = b;
			display_trianglecount = b;
			string bstr = b ? "true" : "false";
			return "Stat display toggled: " + bstr;
		}

		if (command[0] == "default_models") {
			string modelNames = "";
			for (std::map<string, string>::iterator it = default_models.begin(); it != default_models.end(); ++it) {
				modelNames += "\n" + it->first;
				//std::cout << "Key: " << it->first << std::endl();
				//std::cout << "Value: " << it->second << std::endl();
			}
			return "Default model list: " + modelNames;
		}

		if (command[0] == "toggle_wireframe") {
			if (stob(command[1])) {
				glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
			}
			else {
				glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
			}
		}

		if (command[0] == "help") {
			string s =
				"Command List:\n"
				"help\n"
				"default_models // shows list of default models\n"
				"toggle_wireframe <boolean> // toggles on/off wireframe mode\n"
				"set_level_scale <float> // sets scale of level\n"
				"show_fps <true/false> // toggle fps on/off\n"
				"show_updates <true/false> // toggle updates on/off\n"
				"show_trianglecount <true/false> // toggle triangle count on/off\n"
				"show_all <true/false> // toggle all stats on/off\n"
				"set_model_scale <float> // sets scale for all models in scene and future models\n"
				"model_load <model name> // loads model from defult models at 0,0,0\n"
				"model_load <directry> // loads model from directry at 0,0,0\n"
				"model_load <model name> <float>,<float>,<float> // loads model from defult models at position <x>,<y>,<z> \n"
				"model_load <directry> <float>,<float>,<float> // loads model from directry at poition <x>,<y>,<z>\n"
				"model_load <model name> <float> // loads model from defult models at position (uses float for x, y, and z values)\n"
				"model_load <directry> <float> // loads model from directry at poition (uses float for x, y, and z values)\n"
				"model_load <model name> <float>,<float>,<float> <float> // loads model from defult models at position with scale  \n"
				"model_load <directry> <float>,<float>,<float> <float> // loads model from directry at poition with scale\n"
				"model_load <model name> <float>,<float>,<float> <float> <float>,<float>,<float> // loads model from defult models at position with scale and rotation values\n"
				"model_load <directry> <float>,<float>,<float> <float> <float>,<float>,<float> // loads model from directry at poition with scale and rotation values\n"
				"model_load <model name> <float>,<float>,<float> <float> <float> // loads model from defult models at position with scale and rotation values\n"
				"model_load <directry> <float>,<float>,<float> <float> <float> // loads model from directry at poition with scale and rotation values\n"
				"clear // clears previous commands\n"
				"quit\n";
			return s;
		}

		if (command[0] == "clear") {
			previousCommands = "";
			return "";
		}

		// last of commands
		if (command[0] == "quit" || command[0] == "q") {
			glfwSetWindowShouldClose(window, true);
			return "Executed:" + command[0];
		}

		return "Command Unknown";
	}
	catch (const std::exception&)
	{
		return "Error Running Command";
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
		cout << s;
		if (throw_on_error)
			throw std::invalid_argument(s);
	}

	return result;
}

glm::vec3 toPosition(string s) {
	vector<string> numbers = split(s, ',');
	if (numbers.size() == 1)
		return glm::vec3(stof(numbers[0]));
	else
		return glm::vec3(stof(numbers[0]), stof(numbers[1]), stof(numbers[2]));
}

vector<string> split(string x, char delim)
{
	x += delim; //includes a delimiter at the end so last word is also read
	vector<string> splitted;
	string temp = "";
	for (int i = 0; i < x.length(); i++)
	{
		if (x[i] == delim)
		{
			splitted.push_back(temp); //store words in "splitted" vector
			temp = "";
			i++;
		}
		temp += x[i];
	}
	return splitted;
}

void RenderText(Shader& shader, std::string text, float x, float y, float scale, glm::vec3 color)
{
	// activate corresponding render state	
	shader.use();
	glUniform3f(glGetUniformLocation(shader.ID, "textColor"), color.x, color.y, color.z);
	glActiveTexture(GL_TEXTURE0);
	glBindVertexArray(VAO_text);

	// iterate through all characters
	std::string::const_iterator c;
	for (c = text.begin(); c != text.end(); c++)
	{
		Character ch = Characters[*c];

		float xpos = x + ch.Bearing.x * scale;
		float ypos = y - (ch.Size.y - ch.Bearing.y) * scale;

		float w = ch.Size.x * scale;
		float h = ch.Size.y * scale;
		// update VBO for each character
		float vertices[6][4] = {
			{ xpos,     ypos + h,   0.0f, 0.0f },
			{ xpos,     ypos,       0.0f, 1.0f },
			{ xpos + w, ypos,       1.0f, 1.0f },

			{ xpos,     ypos + h,   0.0f, 0.0f },
			{ xpos + w, ypos,       1.0f, 1.0f },
			{ xpos + w, ypos + h,   1.0f, 0.0f }
		};
		// render glyph texture over quad
		glBindTexture(GL_TEXTURE_2D, ch.TextureID);
		// update content of VBO memory
		glBindBuffer(GL_ARRAY_BUFFER, VBO_text);
		glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(vertices), vertices); // be sure to use glBufferSubData and not glBufferData

		glBindBuffer(GL_ARRAY_BUFFER, 0);
		// render quad
		glDrawArrays(GL_TRIANGLES, 0, 6);
		// now advance cursors for next glyph (note that advance is number of 1/64 pixels)
		x += (ch.Advance >> 6) * scale; // bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))
	}
	glBindVertexArray(0);
	glBindTexture(GL_TEXTURE_2D, 0);
}

void Update() {

	// normal things
	if (!console_enabled) {

		if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
			camera.ProcessKeyboard(FORWARD, deltaTime);
		if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
			camera.ProcessKeyboard(BACKWARD, deltaTime);
		if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
			camera.ProcessKeyboard(LEFT, deltaTime);
		if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
			camera.ProcessKeyboard(RIGHT, deltaTime);
		if (glfwGetKey(window, GLFW_KEY_LEFT_SHIFT) == GLFW_PRESS)
			camera.ProcessKeyboard(DOWN, deltaTime);
		if (glfwGetKey(window, GLFW_KEY_SPACE) == GLFW_PRESS)
			camera.ProcessKeyboard(UP, deltaTime);

		// - Only update at 60 frames / s
		while (deltaTime >= 1.0) {
			// input
			processInput(window);
			updates++;
			deltaTime--;
		}
	}

	/* The goal it to have a lot of methods collected here*/
	//process inputs
}
/*
boolean loadSkyBox(std::string strSkybox) {

	try
	{
		stbi_set_flip_vertically_on_load(false);
		vector<std::string> faces
		{
			"right.jpg",
			"left.jpg",
			"top.jpg",
			"bottom.jpg",
			"front.jpg",
			"back.jpg"
		};

		unsigned int cubemapTexture;

		for (int i = 0; i < faces.size(); i++)
		{
			faces[i] = strSkybox + "/" + faces[i];
		}
		cubemapTexture = loadCubemap(faces);


		skyboxShader.use();
		skyboxShader.setInt("skybox", 0);

		stbi_set_flip_vertically_on_load(true);
	}
	catch (const std::exception&)
	{

		return false;
	}
}
*/
unsigned int loadTexture(char const* path)
{
	unsigned int textureID;
	glGenTextures(1, &textureID);

	int width, height, nrComponents;
	unsigned char* data = stbi_load(path, &width, &height, &nrComponents, 0);
	if (data)
	{
		GLenum format;
		if (nrComponents == 1)
			format = GL_RED;
		else if (nrComponents == 3)
			format = GL_RGB;
		else if (nrComponents == 4)
			format = GL_RGBA;

		glBindTexture(GL_TEXTURE_2D, textureID);
		glTexImage2D(GL_TEXTURE_2D, 0, format, width, height, 0, format, GL_UNSIGNED_BYTE, data);
		glGenerateMipmap(GL_TEXTURE_2D);

		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

		stbi_image_free(data);
	}
	else
	{
		std::cout << "Texture failed to load at path: " << path << std::endl;
		stbi_image_free(data);
	}

	return textureID;
}


/// <summary>
/// 
/// </summary>
typedef void (*fp)();
typedef void (*fpi)(int);
typedef void (*fpf)(float);
typedef void (*fps)(string);
typedef void (*fpb)(bool);

template<typename T, typename U>
struct funcV {
	U f;
	T v;
};

class Function {
public:
	vector<fp> func;

	vector<funcV<int, fpi>> funcI;
	vector<funcV<float, fpf>> funcF;
	vector<funcV<string, fps>> funcS;
	vector<funcV<bool, fpb>> funcB;

	void clear() {
		func.empty();
		funcI.empty();
	}

	void addFunc(fp f) {
		func.push_back(f);
	}

	void addFuncI(fpi pointer, int v) {
		funcV<int, fpi> f;
		f.f = pointer;
		f.v = v;
		funcI.push_back(f);
	}

	void addFuncF(fpf pointer, float v) {
		funcV<float, fpf> f;
		f.f = pointer;
		f.v = v;
		funcF.push_back(f);
	}

	void addFuncS(fps pointer, string v) {
		funcV<string, fps> f;
		f.f = pointer;
		f.v = v;
		funcS.push_back(f);
	}

	void addFuncB(fpb pointer, bool v) {
		funcV<bool, fpb> f;
		f.f = pointer;
		f.v = v;
		funcB.push_back(f);
	}

private:
	bool Comparinator() {

	}
};

Function func;
chrono::steady_clock::time_point update = chrono::steady_clock::now();
unsigned int delay = 10; // time in milliseconds
/// <summary>
/// allows for input to be procesed
/// </summary>
void processInput(GLFWwindow* windo)
{
	func.clear();


	if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
		glfwSetWindowShouldClose(window, true);

	if (glfwGetKey(window, GLFW_KEY_F1) == GLFW_PRESS)
		func.addFuncI(&changeColour, 1);

	if (glfwGetKey(window, GLFW_KEY_1) == GLFW_PRESS)
		func.addFunc(&ChangeBlendColour);

	if (glfwGetKey(window, GLFW_KEY_F2) == GLFW_PRESS)
		func.addFuncI(&changeColour, 2);

	if (glfwGetKey(window, GLFW_KEY_F3) == GLFW_PRESS)
		func.addFuncI(&changeColour, 3);

	if (glfwGetKey(window, GLFW_KEY_R) == GLFW_PRESS)
		func.addFunc(&SetUseTime);

	//std::cout << glfwGetTime();
	/*if (update < chrono::steady_clock::now()) {
		// updating time
		int t = DeltaTime().count() * 1000;
		//std::cout << t << endl << glfwGetTime() << endl << endl;
		shader.setFloat("time", glfwGetTime());
		update = chrono::steady_clock::now() + chrono::milliseconds(delay);

		*/

	reverse(func.func.begin(), func.func.end());
	while (!func.func.empty()) {
		(func.func.back()) ();
		func.func.pop_back();
		//std::cout << "delta time" << deltaTime << endl;
	}

	reverse(func.funcI.begin(), func.funcI.end());
	while (!func.funcI.empty()) {
		(func.funcI.back().f) (func.funcI.back().v);
		func.funcI.pop_back();
		//std::cout << "delta time" << deltaTime << endl;
	}

	//}
}

int b;

void changeColour(int i) {

	switch (i)
	{
	case -1:
		current.r = 1;
		current.g = 1;
		current.b = 1;
		current.a = 1;
		break;
	case 1:
		if (b == 0) {
			current.r = 1;
			current.g = 0;
			current.b = 0;
			current.a = 1;
			b = 1;
		}
		else if (b == 1) {
			current.r = 0;
			current.g = 1;
			current.b = 0;
			current.a = 1;
			b = 2;
		}
		else if (b == 2) {
			current.r = 0;
			current.g = 0;
			current.b = 1;
			current.a = 1;
			b = 0;
		}
		break;
	case 2:
		current.r = 0.8;
		current.g = 0.1;
		current.b = 0.9;
		current.a = 1;
		break;
	default:
		current.r = 1;
		current.g = 1;
		current.b = 1;
		current.a = 1;
		break;
	}
}

float blend = 1.0;
float change = 0.01;
bool dir = true;

void ChangeBlendColour() {
	if (dir) {
		blend += change;
	}
	else {
		blend -= change;
	}
	if (blend <= 0) {
		dir = true;
	}
	else if (blend >= 1) {
		dir = false;
	}

	shader.setFloat("blend", blend);
}

bool timeUse = false;

void SetUseTime() {
	if (timeUse) {
		timeUse = false;
	}
	else {
		timeUse = true;
	}
	shader.setBool("useTime", timeUse);
}

unsigned int loadCubemap(vector<std::string> faces)
{
	unsigned int textureID;
	glGenTextures(1, &textureID);
	glBindTexture(GL_TEXTURE_CUBE_MAP, textureID);

	int width, height, nrChannels;
	for (unsigned int i = 0; i < faces.size(); i++)
	{
		unsigned char* data = stbi_load(faces[i].c_str(), &width, &height, &nrChannels, 0);
		if (data)
		{
			glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i,
				0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, data
			);
			stbi_image_free(data);
			skyBoxLoaded = true;
		}
		else
		{
			std::cout << "Cubemap tex failed to load at path: " << faces[i] << std::endl;
			skyBoxLoaded = false;
			stbi_image_free(data);
		}
	}
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);

	return textureID;
}
