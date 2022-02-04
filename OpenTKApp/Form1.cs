using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using Utils;



namespace OpenTKApp
{
    public partial class Form1 : Form
    {
        private bool canDraw = false;        
        private Shader shader;

        int vertexArrayObject1;
        int vertexArrayObject2;

        CircuitObject co1 = new CircuitObject();
        CircuitObject co2 = new CircuitObject();

        List<KeyValuePair<string, CircuitObject>> kvplist = new List<KeyValuePair<string, CircuitObject>>();





        public Form1()
        {
            InitializeComponent();
            Logger.Append("===== Appication Started ======");
        }       

        private void renderCanvas_Load(object sender, EventArgs e)
        {
            shader = new Shader("Shaders/VertexShader.glsl", "Shaders/FragmentShader.glsl");

            // Write the positions of vertices to a vertex shader
            bool good = InitVertexBuffers();
            if (!good)
            {
                Logger.Append("Failed to write the positions of vertices to a vertex shader");
                return;
            }

            // Specify the color for clearing
            GL.ClearColor(Color.DarkSlateBlue);


            canDraw = true;

        }

        private bool InitVertexBuffers()
        {
            
            
            co1.SetVertices(new float[]{ 0f, 0.5f, -0.5f, -0.5f, 0.5f, -0.5f });            
            co2.SetVertices(new float[] { -1f, 0.1f, -1.1f, -0.1f, 1.1f, -0.1f });

            GLError.GLClearError();


            //create vertex array object
            GL.GenVertexArrays(1, out vertexArrayObject1);
            GLError.GLCheckError();
            GL.BindVertexArray(vertexArrayObject1);
            GLError.GLCheckError();


            // Create a buffer object
            int vertexBufferObject1;
            GL.GenBuffers(1, out vertexBufferObject1);
            GLError.GLCheckError();

            // Bind the buffer object to target
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject1);
            GLError.GLCheckError();

            // Write data into the buffer object
            GL.BufferData(BufferTarget.ArrayBuffer, co1.vertices.Length * sizeof(float), co1.vertices, BufferUsageHint.StaticDraw);
            GLError.GLCheckError();

                       

            // Get the storage location of a_Position
            int a_Position = shader.GetAttribLocation("a_Position");
            // Assign the buffer object to a_Position variable
            GL.VertexAttribPointer(a_Position, 2, VertexAttribPointerType.Float, false, 0, 0);
            GLError.GLCheckError();

            // Enable the assignment to a_Position variable
            GL.EnableVertexAttribArray(a_Position);
            GLError.GLCheckError();



            //create vertex array object            
            GL.GenVertexArrays(1, out vertexArrayObject2);
            GLError.GLCheckError();
            GL.BindVertexArray(vertexArrayObject2);
            GLError.GLCheckError();

            // Create a buffer object
            int vertexBufferObject2;
            GL.GenBuffers(1, out vertexBufferObject2);
            GLError.GLCheckError();

            // Bind the buffer object to target
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject2);
            GLError.GLCheckError();

            // Write data into the buffer object
            GL.BufferData(BufferTarget.ArrayBuffer, co2.vertices.Length * sizeof(float), co2.vertices, BufferUsageHint.StaticDraw);
            GLError.GLCheckError();

            // Get the storage location of a_Position
            a_Position = shader.GetAttribLocation("a_Position");
            // Assign the buffer object to a_Position variable
            GL.VertexAttribPointer(a_Position, 2, VertexAttribPointerType.Float, false, 0, 0);
            GLError.GLCheckError();

            // Enable the assignment to a_Position variable
            GL.EnableVertexAttribArray(a_Position);
            GLError.GLCheckError();

            return true;
        }

        private void renderCanvas_Paint(object sender, PaintEventArgs e)
        {

            OpenTK.WinForms.GLControl renderCanvas = (OpenTK.WinForms.GLControl)sender;

            if (canDraw)
            {
                renderCanvas.MakeCurrent();
                GLError.GLCheckError();
                GL.Viewport(0, 0, Width, Height);
                GLError.GLCheckError();

                // Clear the render canvas with the current color
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GLError.GLCheckError();

                TabPage tp = (TabPage)renderCanvas.Parent;
                KeyValuePair<string, CircuitObject> co = kvplist.Find(x => x.Key == tp.Name);

                if (tp.Name == "tabPage1")
                    GL.BindVertexArray(vertexArrayObject1);
                else
                    GL.BindVertexArray(vertexArrayObject2);
                
                shader.Use();
                // Draw a triangle
                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);                

                //does this help?
                GL.Flush();
                GLError.GLCheckError();

                renderCanvas.SwapBuffers();
                GLError.GLCheckError();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TabPage tabPage;
            OpenTK.WinForms.GLControl glControl;


            
            tabPage = new System.Windows.Forms.TabPage();
            glControl = new OpenTK.WinForms.GLControl();

            // 
            // tabPage1
            // 
            string title = "TabPage " + (tabControl1.TabCount + 1).ToString();
            tabPage.Location = new System.Drawing.Point(4, 24);
            tabPage.Name = "tabPage" + (tabControl1.TabCount + 1).ToString();
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(192, 72);
            tabPage.TabIndex = 0;
            tabPage.Text = title;
            tabPage.UseVisualStyleBackColor = true;


            //add the glControl to the tab page          

            // 
            // glControl1
            // 
            glControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl.APIVersion = new System.Version(3, 3, 0, 0);
            glControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl.IsEventDriven = true;
            glControl.Location = new System.Drawing.Point(25, 135);
            glControl.Name = "glControl";
            glControl.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            glControl.Size = new System.Drawing.Size(105, 76);
            glControl.TabIndex = 0;
            glControl.Text = "glControl";
            glControl.Dock = DockStyle.Fill;

            glControl.Paint += new System.Windows.Forms.PaintEventHandler(renderCanvas_Paint);
            glControl.Load += new System.EventHandler(renderCanvas_Load);

            //KeyValuePair<string, CircuitObject> kvp = new KeyValuePair<string, CircuitObject>

            //kvplist.Add(new KeyValuePair<string, CircuitObject>(tabPage.Name, co1));

            tabPage.Controls.Add(glControl);
            this.tabControl1.Controls.Add(tabPage);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

       

        }

        private void tabControl1_ControlAdded(object sender, ControlEventArgs e)
        {
            Logger.Append("You Added a control!");
        }
    }
}
