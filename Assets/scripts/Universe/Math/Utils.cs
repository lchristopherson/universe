using System;
using UnityEngine;

namespace Universe.Math {

	public class Utils {

		public static Vector3 ToVector3(Vec3f vec) {
			return new Vector3 (vec.X, vec.Y, vec.Z);
		}

	}

}