using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;

namespace VectorsGraf
{
    public class Camera : Vec3, IControlable
    {

        private bool keyPressed = false;
        private double AndgleX = 0, AndgleY = 0, AndgleZ = 0;
        public Matrix4x4 rotationMatrix = Matrix4x4.CreateRotationY(0);
        public Camera(float x, float y, float z) : base(x, y, z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Camera(Vec3 v) : base(v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }
        public void Update()
        {
            if (keyPressed)
            {
                keyPressed = false;
            }
        }

        public void KeyControl(ConsoleKey key)
        {
            /*if (key == ConsoleKey.Spacebar && !keyPressed)
                keyPressed = true;*/
            switch (key)
            {
                case ConsoleKey.A:
                    X += -0.25f;
                    keyPressed = true;
                    break;
                case ConsoleKey.D:
                    X += 0.25f;
                    keyPressed = true;
                    break;
                case ConsoleKey.W:
                    Y = Y - 0.25f;
                    /* X = X - 0.25f;
                     X = */
                    keyPressed = true;
                    break;
                case ConsoleKey.S:
                    Y += 0.25f;
                    keyPressed = true;
                    break;
                case ConsoleKey.PageUp:
                    Z += 0.25f;
                    keyPressed = true;
                    break;
                case ConsoleKey.PageDown:
                    Z += -0.25f;
                    keyPressed = true;
                    break;
                case ConsoleKey.Q:
                    AndgleZ -= 0.05;
                    rotationMatrix = Matrix4x4.CreateRotationZ((float)AndgleZ);
                    keyPressed = true;
                    break;
                case ConsoleKey.E:
                    AndgleZ += 0.05;
                    rotationMatrix = Matrix4x4.CreateRotationZ((float)AndgleZ);
                    keyPressed = true;
                    break;
                case ConsoleKey.LeftArrow:
                    AndgleY += 0.05;
                    rotationMatrix = Matrix4x4.CreateRotationY((float)AndgleY);
                    keyPressed = true;
                    break;
                case ConsoleKey.RightArrow:
                    AndgleY -= 0.05;
                    rotationMatrix = Matrix4x4.CreateRotationY((float)AndgleY);
                    keyPressed = true;
                    break;
                case ConsoleKey.UpArrow:
                    AndgleX += 0.05;
                    rotationMatrix = Matrix4x4.CreateRotationX((float)AndgleX);
                    keyPressed = true;
                    break;
                case ConsoleKey.DownArrow:
                    AndgleX -= 0.05;
                    rotationMatrix = Matrix4x4.CreateRotationX((float)AndgleX);
                    keyPressed = true;
                    break;

            }

        }


    }
}