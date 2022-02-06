using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.IO;
using Utils;

namespace OpenTKApp
{
    public class Shader
    {
        private int program;
        public Shader(string vertextShader, string fragmentShader) 
        {

            // Load shaders from files
            string vShaderSource = null;
            string fShaderSource = null;
            LoadFile("Shaders/VertexShader.glsl", out vShaderSource);
            LoadFile("Shaders/FragmentShader.glsl", out fShaderSource);
            if (vShaderSource == null || fShaderSource == null)
            {
                Logger.Append("Failed load shaders from files");
                return;
            }

            // Initialize the shaders
            if (!InitShaders(vShaderSource, fShaderSource, out program))
            {
                Logger.Append("Failed to initialize the shaders");
                return;
            }
        }

        public bool InitShaders(string vShaderSource, string fShaderSource, out int program)
        {
            program = CreateProgram(vShaderSource, fShaderSource);
            if (program == 0)
            {
                Logger.Append("Failed to create program");
                return false;
            }

            return true;
        }

        ///<summary>
        ///Load a shader from a file
        ///</summary>
        ///<param name="errorOutputFileName">a file name for error messages</param>
        ///<param name="fileName">a file name to a shader</param>
        ///<param name="shaderSource">a shader source string</param>
        public void LoadFile(string shaderFileName, out string shaderSource)
        {
            shaderSource = null;

            //clear out existing logfile 
            if (!File.Exists(shaderFileName))
            {
                Logger.Append("Could not find file: " + shaderFileName);
                return;
            }

            using (StreamReader sr = new StreamReader(shaderFileName))
            {
                shaderSource = sr.ReadToEnd();
            }
        }

        private int LoadShader(ShaderType shaderType, string shaderSource)
        {
            // Create shader object
            GLError.GLClearError();
            int shader = GL.CreateShader(shaderType); GLError.GLCheckError();
            if (shader == 0)
            {
                Logger.Append("Unable to create shader");
                return 0;
            }

            // Set the shader program
            GL.ShaderSource(shader, shaderSource); GLError.GLCheckError();

            // Compile the shader
            GL.CompileShader(shader); GLError.GLCheckError();

            // Check the result of compilation
            int status;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status); GLError.GLCheckError();
            if (status == 0)
            {
                string errorString = string.Format("Failed to compile {0} shader: {1}", shaderType.ToString(), GL.GetShaderInfoLog(shader));
                Logger.Append(errorString);
                GL.DeleteShader(shader); GLError.GLCheckError();
                return 0;
            }

            return shader;
        }

        private int CreateProgram(string vShader, string fShader)
        {
            // Create shader object
            int vertexShader = LoadShader(ShaderType.VertexShader, vShader);
            int fragmentShader = LoadShader(ShaderType.FragmentShader, fShader);
            if (vertexShader == 0 || fragmentShader == 0)
            {
                return 0;
            }

            // Create a program object
            int program = GL.CreateProgram();
            GLError.GLCheckError();
            if (program == 0)
            {
                return 0;
            }

            // Attach the shader objects
            GL.AttachShader(program, vertexShader); GLError.GLCheckError();
            GL.AttachShader(program, fragmentShader); GLError.GLCheckError();

            // Link the program object
            GL.LinkProgram(program); GLError.GLCheckError();

            // Check the result of linking
            int status;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status); GLError.GLCheckError();
            if (status == 0)
            {
                string errorString = string.Format("Failed to link program: {0}" + Environment.NewLine, GL.GetProgramInfoLog(program));
                Logger.Append(errorString);
                GL.DeleteProgram(program); GLError.GLCheckError();
                GL.DeleteShader(vertexShader); GLError.GLCheckError();
                GL.DeleteShader(fragmentShader); GLError.GLCheckError();
                return 0;
            }

            return program;
        }

        public void Use()
        {
            GL.UseProgram(program);
        }

        public int GetProgram()
        {
            return program;
        }

        public int GetAttribLocation(string attribute)
        {
            //get the position of the attribute specified in text
            int position = GL.GetAttribLocation(program, attribute);
            if (position < 0)
            {
                Logger.Append("Failed to get the storage location of attribute" + attribute);
                return -1;
            }
            return position;
        }

        public int GetUniformLocation(string name)
        {
            int position = GL.GetUniformLocation(program, name);
            if (position < 0)
            {
                Logger.Append("Failed to get the storage location of uniform" + name);
                return -1;
            }
            return position;

        }

    }
}
