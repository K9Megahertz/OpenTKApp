using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTKApp
{
    
    public class CircuitObject
    {

        public float[] vertices;
        public int vertexCount;


        public void SetVertices(float[] verts)
        {
            vertices = verts;
            vertexCount = verts.Length / 2;
        }

    }
    
}
