using System;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace Utils
{  

    public class ShaderLoader
    {
        ///<summary>
        ///Create a program object and make current
        ///</summary>
        ///<param name="vShader">a vertex shader program</param>
        ///<param name="fShader">a fragment shader program</param>
        ///<param name="program">created program</param>
        ///<returns>
        ///return true, if the program object was created and successfully made current
        ///</returns>
        public static bool InitShaders(string vShaderSource, string fShaderSource, out int program)
        {
            program = CreateProgram(vShaderSource, fShaderSource);
            if (program == 0)
            {
                Logger.Append("Failed to create program");
                return false;
            }

            GL.UseProgram(program);

            return true;
        }

        ///<summary>
        ///Load a shader from a file
        ///</summary>
        ///<param name="errorOutputFileName">a file name for error messages</param>
        ///<param name="fileName">a file name to a shader</param>
        ///<param name="shaderSource">a shader source string</param>
        public static void LoadShader(string shaderFileName, out string shaderSource)
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

        private static int LoadShader(ShaderType shaderType, string shaderSource)
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

        private static int CreateProgram(string vShader, string fShader)
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

       
    }

    public static class Logger
    {
        public static string logFileName = "info.txt";

        /// <summary>
        /// Default Constructor
        /// </summary>        
        static Logger()
        {
            
            //clear out existing logfile 
            if (File.Exists(Logger.logFileName))
            {
                // Clear File
                File.WriteAllText(Logger.logFileName, "");
            }

            //Let the games begin!
            Append("===== Logger Started =====");            
        }

        /// <summary>
        /// Write a message to a log file
        /// </summary>
        /// <param name="message">a message that will append to a log file</param>
        public static void Append(string message)
        {
            //format date and time into a string
            string dt = DateTime.Now.ToString("[MM\\/dd\\/yyyy HH\\:mm\\:ss.fff] ");

            //write message to log file with date and time preffix
            File.AppendAllText(logFileName, dt + message + Environment.NewLine);
        }
    }

    public class GLError
    {
        public static void GLClearError()
        {
            //continually call GetError until its clean clearing out the junk.
            while (GL.GetError() != ErrorCode.NoError) ;
        }

        public static void GLCheckError()
        {
            ErrorCode error;

            //so log as we keep getting error messsages keep printing them out, if we get NoError we're done
            while ((error = GL.GetError()) != ErrorCode.NoError)            
            {                
                string errstr = "Error " + error;
                Logger.Append(errstr);
            }
        }


    }
}
