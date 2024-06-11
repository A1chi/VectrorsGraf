using System.Numerics;

namespace VectorsGraf
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int screenWidth = 160;
            int screenHeight = 40;
            char[] screen = new char[screenWidth * screenHeight];
            Camera camera = new Camera(1, 1, -10);
            Vec3 light = new Vec3(-15, -15, -10);
            float aspect = (float)screenWidth / screenHeight;
            float pixAspect = 11.0f / 24.0f;
            //camera = new Camera(Sphere.RotateY(camera, Math.PI / 4));
            KeyControl controller = new KeyControl();
            controller.usedKey += camera.KeyControl;
            ConsoleKey key = new ConsoleKey();
            Thread listening = new Thread(new ThreadStart(controller.Listen));
            List<IRayTrasable> objects = new List<IRayTrasable>
            {
                 // new Cube (new Vec3(0, 8, 0), new Vec3(8, 16, 8)),
                new Sphere(new Vec3(0, 0, 5), 3),
                new Plane(new Vec3(0, -1, -1), new Vec3(0, 0, 10))
            };
            listening.Start();
            double angle = 0;
            while (true)
            {
                Array.Fill(screen, ' ');
                angle -= 0.005;
                Matrix4x4 rotationMatrix = camera.rotationMatrix;
                //camera.Update();

                for (int y = 0; y < screenHeight; y++)
                {
                    for (int x = 0; x < screenWidth; x++)
                    {
                        float u = ((x / (float)screenWidth) * 2.0f - 1.0f);
                        float v = (y / (float)screenHeight) * 2.0f - 1.0f;
                        Vec3 rayDir = new Vec3(u, v, 1).Normalize();
                        rayDir.X *= aspect * pixAspect;
                        rayDir = Transform(rayDir, camera.rotationMatrix);
                        //camera = new Camera(Transform(camera, camera.rotationMatrix));
                        Ray ray = new Ray(camera, rayDir);

                        Vec3 color = TraceRay(ray, objects, light, 2);
                        screen[y * screenWidth + x] = " .:!/r(l1Z4H9W8$@"[(int)(Math.Min(1.0f, color.Length()) * 16)];
                    }
                }
                Console.SetCursorPosition(0, 0);
                Console.Write(screen);
            }
        }
        public static Vec3 Transform(Vec3 v, Matrix4x4 m)
        {
            return new Vec3(
                v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31 + m.M41,
                v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32 + m.M42,
                v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33 + m.M43
            );
        }

        public static Vec3 TraceRay(Ray ray, List<IRayTrasable> objects, Vec3 light, int depth)
        {
            if (depth == 0 )
                return new Vec3(0, 0, 0);

            IRayTrasable closestObject = null;
            float closestT = float.MaxValue;

            foreach (var obj in objects)
            {
                float t = obj.Intersect(ray);
                if (t > 0 && t < closestT)
                {
                    closestT = t;
                    closestObject = obj;
                }
            }

            if (closestObject == null)
                return new Vec3(0, 0, 0);

            Vec3 hitPoint = ray.Origin + ray.Direction * closestT;
            Vec3 normal = closestObject.GetNormal(hitPoint);

            float intensity = Math.Max(0, Vec3.Dot(normal, (light - hitPoint).Normalize()));
            Vec3 localColor = new Vec3(intensity, intensity, intensity);

            Vec3 reflectDir = Reflect(ray.Direction, normal).Normalize();
            Ray reflectRay = new Ray(hitPoint + reflectDir * 0.001f, reflectDir);
            Vec3 reflectColor = TraceRay(reflectRay, objects, light, depth - 1);

            return localColor * 0.6f + reflectColor * 0.2f;
        }

        static Vec3 Reflect(Vec3 I, Vec3 N)
        {
            return I - N * 2.0f * Vec3.Dot(I, N);
        }
    }

    interface IRayTrasable
    {
        float Intersect(Ray ray);
        Vec3 GetNormal(Vec3 hitPoint);
    }
    class Cube : IRayTrasable, IControlable
    {
        public Vec3 Min { get; }
        public Vec3 Max { get; }

        public Cube(Vec3 min, Vec3 max)
        {
            Min = min;
            Max = max;
        }

        public float Intersect(Ray ray)
        {
            float tmin = (Min.X - ray.Origin.X) / ray.Direction.X;
            float tmax = (Max.X - ray.Origin.X) / ray.Direction.X;

            if (tmin > tmax)
            {
                float temp = tmin;
                tmin = tmax;
                tmax = temp;
            }

            float tymin = (Min.Y - ray.Origin.Y) / ray.Direction.Y;
            float tymax = (Max.Y - ray.Origin.Y) / ray.Direction.Y;

            if (tymin > tymax)
            {
                float temp = tymin;
                tymin = tymax;
                tymax = temp;
            }

            if ((tmin > tymax) || (tymin > tmax))
                return -1;

            if (tymin > tmin)
                tmin = tymin;

            if (tymax < tmax)
                tmax = tymax;

            float tzmin = (Min.Z - ray.Origin.Z) / ray.Direction.Z;
            float tzmax = (Max.Z - ray.Origin.Z) / ray.Direction.Z;

            if (tzmin > tzmax)
            {
                float temp = tzmin;
                tzmin = tzmax;
                tzmax = temp;
            }

            if ((tmin > tzmax) || (tzmin > tmax))
                return -1;

            if (tzmin > tmin)
                tmin = tzmin;

            if (tzmax < tmax)
                tmax = tzmax;

            return tmin > 0 ? tmin : tmax;
        }

        public Vec3 GetNormal(Vec3 hitPoint)
        {
            const float epsilon = 1e-5f;
            Vec3 normal = new Vec3(0, 0, 0);

            if (Math.Abs(hitPoint.X - Min.X) < epsilon)
                normal = new Vec3(-1, 0, 0);
            else if (Math.Abs(hitPoint.X - Max.X) < epsilon)
                normal = new Vec3(1, 0, 0);
            else if (Math.Abs(hitPoint.Y - Min.Y) < epsilon)
                normal = new Vec3(0, -1, 0);
            else if (Math.Abs(hitPoint.Y - Max.Y) < epsilon)
                normal = new Vec3(0, 1, 0);
            else if (Math.Abs(hitPoint.Z - Min.Z) < epsilon)
                normal = new Vec3(0, 0, -1);
            else if (Math.Abs(hitPoint.Z - Max.Z) < epsilon)
                normal = new Vec3(0, 0, 1);

            return normal;
        }

        public void KeyControl(ConsoleKey key)
        {
            throw new NotImplementedException();
        }
    }
    public class Vec3
    {
        public float X, Y, Z;

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vec3(Vec3 v) { X = v.X; Y = v.Y; Z = v.Z; }

        public static Vec3 operator +(Vec3 a, Vec3 b) => new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vec3 operator -(Vec3 a, Vec3 b) => new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vec3 operator *(Vec3 a, float b) => new Vec3(a.X * b, a.Y * b, a.Z * b);
        public static Vec3 operator *(Vec3 a, Vec3 b) => new Vec3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static float Dot(Vec3 a, Vec3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public Vec3 Normalize()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            return new Vec3(X / length, Y / length, Z / length);
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }
    }

    class Ray
    {
        public Vec3 Origin { get; }
        public Vec3 Direction { get; }

        public Ray(Vec3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }

    class Sphere : IRayTrasable
    {
        public Vec3 Center { get; set; }
        public float Radius { get; }

        public Sphere(Vec3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public float Intersect(Ray ray)
        {
            Vec3 oc = ray.Origin - Center;
            float a = Vec3.Dot(ray.Direction, ray.Direction);
            float b = 2.0f * Vec3.Dot(oc, ray.Direction);
            float c = Vec3.Dot(oc, oc) - Radius * Radius;
            float discriminant = b * b - 4 * a * c;
            if (discriminant > 0)
            {
                return (-b - (float)Math.Sqrt(discriminant)) / (2.0f * a);
            }
            return -1.0f;
        }

        public Vec3 GetNormal(Vec3 hitPoint)
        {
            return (hitPoint - Center).Normalize();
        }

        public static Vec3 RotateY(Vec3 vectr, double angle)
        {
            Vec3 newVectr = new Vec3(vectr);
            newVectr.X = (float)(vectr.X * Math.Cos(angle) - vectr.Z * Math.Sin(angle));
            newVectr.Z = (float)(vectr.X * Math.Sin(angle) + vectr.Z * Math.Cos(angle));
            return newVectr;
        }

        public static Vec3 RotateX(Vec3 vectr, double angle)
        {
            Vec3 newVectr = new Vec3(vectr);
            newVectr.Y = (float)(vectr.Y * Math.Cos(angle) - vectr.Z * Math.Sin(angle));
            newVectr.Z = (float)(vectr.Y * Math.Sin(angle) + vectr.Z * Math.Cos(angle));
            return newVectr;
        }

        public static Vec3 RotateZ(Vec3 vectr, double angle)
        {
            Vec3 newVectr = new Vec3(vectr);
            newVectr.X = (float)(vectr.X * Math.Cos(angle) - vectr.Y * Math.Sin(angle));
            newVectr.Y = (float)(vectr.Y * Math.Cos(angle) + vectr.X * Math.Sin(angle));
            return newVectr;
        }
    }

    class Plane : IRayTrasable
    {
        public Vec3 Normal { get; }
        public Vec3 Point { get; }

        public Plane(Vec3 normal, Vec3 point)
        {
            Normal = normal.Normalize();
            Point = point;
        }

        public float Intersect(Ray ray)
        {
            float denom = Vec3.Dot(Normal, ray.Direction);
            if (Math.Abs(denom) > 1e-6) // Avoid division by zero
            {
                Vec3 p0l0 = Point - ray.Origin;
                float t = Vec3.Dot(p0l0, Normal) / denom;
                return (t >= 0) ? t : -1;
            }
            return -1;
        }

        public Vec3 GetNormal(Vec3 hitPoint)
        {
            return Normal;
        }
    }
}
