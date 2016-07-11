using System;

namespace Universe.Math {

	public class Vec3f {

		private float[] xyz = new float[3];

		public Vec3f() : this(0, 0, 0) {
		}

		public Vec3f(Vec3f other) {
			xyz [0] = other.xyz [0];
			xyz [1] = other.xyz [1];
			xyz [2] = other.xyz [2];
		}

		public Vec3f(float x, float y, float z) {
			xyz [0] = x;
			xyz [1] = y;
			xyz [2] = z;
		}

		public float this[int key] {
			get { 
				return xyz [key];
			}
			set { 
				xyz [key] = value;
			}
		}

		public float X {
			get { 
				return xyz [0];
			}
			set { 
				xyz [0] = value;
			}
		}

		public float Y {
			get { 
				return xyz [1];
			}
			set { 
				xyz [1] = value;
			}
		}

		public float Z {
			get { 
				return xyz [2];
			}
			set { 
				xyz [2] = value;
			}
		}

		public float Magnitude() {
			return (float)System.Math.Sqrt (X * X + Y * Y + Z * Z);
		}

		public Vec3f Normalized() {
			return this / Magnitude();
		}

		public void Normalize() {
			float magnitude = Magnitude ();
			X = X / magnitude;
			Y = Y / magnitude;
			Z = Z / magnitude;
		}

		public static Vec3f operator*(Vec3f v0, Vec3f v1) {
			return new Vec3f (v0.X * v1.X, v0.Y * v1.Y, v0.Z * v1.Z);
		}

		public static Vec3f operator*(Vec3f v0, float c) {
			return new Vec3f (v0.X * c, v0.Y * c, v0.Z * c);
		}

		public static Vec3f operator/(Vec3f v0, Vec3f v1) {
			return new Vec3f (v0.X / v1.X, v0.Y / v1.Y, v0.Z / v1.Z);
		}

		public static Vec3f operator/(Vec3f v0, float c) {
			return new Vec3f (v0.X / c, v0.Y / c, v0.Z / c);
		}

		public static Vec3f operator+(Vec3f v0, Vec3f v1) {
			return new Vec3f (v0.X + v1.X, v0.Y + v1.Y, v0.Z + v1.Z);
		}

		public static Vec3f operator-(Vec3f v0, Vec3f v1) {
			return new Vec3f (v0.X - v1.X, v0.Y - v1.Y, v0.Z - v1.Z);
		}

		public override String ToString() {
			return "(" + X + ", " + Y + ", " + Z + ")";
		}
	}
}

