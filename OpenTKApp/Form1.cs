using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using Utils;
using Matrix;



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

        OpenTK.Mathematics.Matrix4 projectionMatrix = OpenTK.Mathematics.Matrix4.CreateOrthographic(800, 600, -1.0f, 1.0f);
        float frame = 0;



        //mouse shit for dragpanning
        int xPos = 0;
        int yPos = 0;

        int deltaMouseX = 0;
        int deltaMouseY = 0;

        int savemousex = 0;
        int savemousey = 0;




        public Form1()
        {
            InitializeComponent();
            Logger.Append("===== Appication Started ======");
        }       

        private void renderCanvas_Load(object sender, EventArgs e)
        {

            //create a simple shader
            shader = new Shader("Shaders/VertexShader.glsl", "Shaders/FragmentShader.glsl");

            // Write the positions of vertices to a vertex shader
            bool good = InitVertexBuffers();
            if (!good)
            {
                Logger.Append("Failed to write the positions of vertices to a vertex shader");
                return;
            }

            OpenTK.WinForms.GLControl renderCanvas = (OpenTK.WinForms.GLControl)sender;
            

            // Specify the color for clearing
            GL.Viewport(0, 0, renderCanvas.Width, renderCanvas.Height);
            GL.ClearColor(Color.DarkSlateBlue);


            canDraw = true;

        }

        private bool InitVertexBuffers()
        {
            
            
            co1.SetVertices(new float[] { 0.0f, 0.0f, 300.0f, 0.0f, 300.0f, 200.0f });
            co2.SetVertices(new float[] { 0.0f, 0.0f, 300.0f, 0.0f, 300.0f, -200.0f });

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

            // Enable the assignment to a_Position variable
            GL.EnableVertexAttribArray(a_Position);
            GLError.GLCheckError();

            // Get the storage location of a_Position
            a_Position = shader.GetAttribLocation("a_Position");
            // Assign the buffer object to a_Position variable
            GL.VertexAttribPointer(a_Position, 2, VertexAttribPointerType.Float, false, 0, 0);
            GLError.GLCheckError();

            

            return true;
        }

        private void renderCanvas_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                

                savemousex = e.X;
                savemousey = e.Y;


            }

        }

        private void renderCanvas_MouseUp(object sender, MouseEventArgs e)
        {

            /*if (e.Button == MouseButtons.Right)
            {

                deltaMouseX = e.X - savemousex;
                deltaMouseY = e.Y - savemousey;

                xPos -= deltaMouseX;
                yPos += deltaMouseY;

                savemousex = e.X;
                savemousey = e.Y;

            }*/

        }
        private void renderCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                deltaMouseX = e.X - savemousex;
                deltaMouseY = e.Y - savemousey;

                xPos += deltaMouseX;
                yPos -= deltaMouseY;

                savemousex = e.X;
                savemousey = e.Y;

                OpenTK.WinForms.GLControl renderCanvas = (OpenTK.WinForms.GLControl)sender;
                renderCanvas.Invalidate();

            }
        }

        private void renderCanvas_Paint(object sender, PaintEventArgs e)
        {
            //Cast the sender
            OpenTK.WinForms.GLControl renderCanvas = (OpenTK.WinForms.GLControl)sender;

            if (canDraw)
            {

                // Set the current glControl context to be active
                renderCanvas.MakeCurrent();
                GLError.GLCheckError();

                // Set up our projection matrix based on the canvas size
                projectionMatrix = OpenTK.Mathematics.Matrix4.CreateOrthographic(renderCanvas.Width, renderCanvas.Height, -1.0f, 1.0f);

                OpenTK.Mathematics.Matrix4 translationMatrix = new OpenTK.Mathematics.Matrix4(1, 0, 0, xPos, 
                                                                                              0, 1, 0, yPos, 
                                                                                              0, 0, 1, 0, 
                                                                                              0, 0, 0, 1);
                OpenTK.Mathematics.Matrix4 finalMatrix = projectionMatrix * translationMatrix; 

                //Create a viewport to match
                GL.Viewport(0, 0, renderCanvas.Width, renderCanvas.Height);
                GLError.GLCheckError();

                // Clear the render canvas with the current color
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GLError.GLCheckError();

                //get the tabpage that the glControl is attached to by getting it parent
                TabPage tp = (TabPage)renderCanvas.Parent;

                //TODO: fix this later
                //KeyValuePair<string, CircuitObject> co = kvplist.Find(x => x.Key == tp.Name);


                //TODO: hard coded for now for testing, I'll make this be determined at runtime later
                if (tp.Name == "tabPage1")
                    GL.BindVertexArray(vertexArrayObject1);  //Bind verts for obj1
                else
                    GL.BindVertexArray(vertexArrayObject2);  //Bind verts for obj1

                //Make the shader the active one
                shader.Use();
                
                //get the location of the projection variable inside the shader so we can upload the matrix to it. Neo is waiting.
                int loc = shader.GetUniformLocation("projection");
                GLError.GLCheckError();

                //Upload the projection matrix
                GL.UniformMatrix4(loc, true, ref finalMatrix);
                GLError.GLCheckError();

                // Draw a triangle
                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);                

                //does this help?
                GL.Flush();
                GLError.GLCheckError();

                //present our scene
                renderCanvas.SwapBuffers();
                GLError.GLCheckError();

                frame+= 0.1f;
                renderCanvas.Invalidate();
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
            glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderCanvas_MouseDown);
            glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.renderCanvas_MouseUp);
            glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderCanvas_MouseMove);



            //TODO: fix this up later 
            //KeyValuePair<string, CircuitObject> kvp = new KeyValuePair<string, CircuitObject>;
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

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //This code will render a "x" mark at the end of the Tab caption. 
            e.Graphics.DrawString("x", e.Font, Brushes.Black, e.Bounds.Right - 15, e.Bounds.Top + 4);
            e.Graphics.DrawString(this.tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
            e.DrawFocusRectangle();

        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //Looping through the controls.
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                Rectangle r = tabControl1.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
                if (closeButton.Contains(e.Location))
                {
                    if (MessageBox.Show("Would you like to Close this Tab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.tabControl1.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
