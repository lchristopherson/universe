using System;
using UnityEngine;
using System.Collections.Generic;

public class Geometry {
	/*
	private Triangle[] triangles;
	private float scale;
	private int currentTessLevel;


	public Geometry(Triangle[] triangles, float scale) {
		for (int i = 0; i < triangles.Length; i++) {
			triangles[i].Normalize();
		}

		this.triangles = triangles;
		this.scale = scale;
		this.currentTessLevel = 0;
	}

	public Mesh toMesh() {
		Mesh m = new Mesh ();
		int index = 0;
		int vertexCount = triangles.Length * 3;
		Vector3[] vertices = new Vector3[vertexCount];
		Vector2[] uvs = new Vector2[vertexCount];
		int[] indices = new int[vertexCount];

		foreach (Triangle triangle in triangles) {
			for (int i = 0; i < 3; i++) {
				vertices[index] = triangle.GetVertex(i) * scale;
				indices[index] = index++;
			}
		}

		m.vertices = vertices;
		m.uv = uvs;
		m.triangles = indices;

		return m;
	}

	public void Tessellate() {
		int index = 0;
		Triangle[] newTriangles = new Triangle[triangles.Length * 2];
		foreach (Triangle triangle in triangles) {
			Vector3 v0 = triangle.GetVertex(0);
			Vector3 v1 = triangle.GetVertex(1);
			Vector3 v2 = triangle.GetVertex(2);
			Vector3 mid = ((v0 + v2) / 2).normalized;
			Triangle t0 = new Triangle(v1, mid, v0);
			Triangle t1 = new Triangle(v2, mid, v1);
			newTriangles[index++] = t0;
			newTriangles[index++] = t1;
		}
		triangles = newTriangles;
		currentTessLevel++;
	}

	public void UnTessellate() {
		int size = triangles.Length;
		Triangle[] newTriangles = new Triangle[size / 2];
		for (int i = 0; i < size; i+= 2) {
			Triangle t0 = triangles [i];
			Triangle t1 = triangles [i + 1];
			Vector3 v0 = t0.GetVertex(2);
			Vector3 v1 = t0.GetVertex(0);
			Vector3 v2 = t1.GetVertex(0);
			Triangle result = new Triangle (v0, v1, v2);
			newTriangles [i / 2] = result;
		}
		triangles = newTriangles;
		currentTessLevel--;
	}

	public int GetTessLevel() {
		return currentTessLevel;
	}
	*/
}

