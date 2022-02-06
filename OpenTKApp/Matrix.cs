using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix
{
    class Matrix4f
    {
        float[,] data = new float[4,4];
        
        public Matrix4f()
        {
            Clear(); //not really needed but it gives me warm and fuzzies          
        }

        public Matrix4f(float[,] mat)
        {            
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    data[i, j] = mat[i, j];
                }
            }
        }

        public static Matrix4f Ortho(float left, float right, float top, float bottom, float near, float far)
        {
            Matrix4f res = new Matrix4f();

            res.data[0,0] = 2.0f / (right - left);
            res.data[1,1] = 2.0f / (top - bottom);
            res.data[2,2] = -2.0f / (far - near);
            res.data[3,0] = -(right + left) / (right - left);
            res.data[3,1] = -(top + bottom) / (top - bottom);
            res.data[3,2] = -(far + near) / (far - near);

            return res;
        }

        public void Identity()
        {
            Clear();
            data[0, 0] = 1.0f;
            data[1, 1] = 1.0f;
            data[2, 2] = 1.0f;
            data[3, 3] = 1.0f;
        }

        

        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    data[i, j] = 0;
                }
            }
        }

        public static Matrix4f operator +(Matrix4f A, Matrix4f B)
        {
            Matrix4f res = new Matrix4f();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    res.data[i, j] = A.data[i, j] + B.data[i, j];
                }
            }

            return res;
        }

        public static Matrix4f operator -(Matrix4f A, Matrix4f B)
        {
            Matrix4f res = new Matrix4f();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    res.data[i, j] = A.data[i, j] - B.data[i, j];
                }
            }

            return res;
        }


        public static Matrix4f operator *(Matrix4f A, Matrix4f B)
        {
            Matrix4f res = new Matrix4f();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        res.data[i, j] += A.data[i, k] * B.data[k, j];
                    }
                }
            }

            return res;
        }
    }
}



