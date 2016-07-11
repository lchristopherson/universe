using System;
using UnityEngine;

using Universe.Math;

public class Vertex {

	Vec3f position;
	Vector2 uv;
	Vec3f normal;

	public Vertex(Vec3f position, Vector2 uv, Vec3f normal) {
		this.position = position;
		this.uv = uv;
		this.normal = normal;
	}

	public Vec3f Position {
		get { 
			return this.position;
		}
	}

	public Vector2 UV {
		get { 
			return this.uv;
		}
	}

	public Vec3f Normal {
		get { 
			return this.normal;
		}
	}

	public void Normalize() {
		position.Normalize();
	}
}

