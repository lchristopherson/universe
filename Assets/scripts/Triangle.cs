using System;
using UnityEngine;

public class Triangle {

	Vertex[] vertices = new Vertex[3];

	public Triangle(Vertex v0, Vertex v1, Vertex v2) {
		vertices[0] = v0;
		vertices[1] = v1;
		vertices[2] = v2;
	}

	public Vertex GetVertex(int index) {
		return vertices[index];
	}

	public void Normalize() {
		for (int i = 0; i < 3; i++)
			vertices[i].Normalize();
	}
}

