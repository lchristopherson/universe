using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Displacement;

using Universe.Math;

public class Planet : MonoBehaviour {

	public GameObject target;
	public int minTessLevel;
	public int maxTessLevel;
	public float nearTessDist;
	public float farTessDist;
	public float scale = 1;
	public Material mat;
	public GameObject[] faces;
	public int seed;

	[Range(-1, 1)]
	public float cutoff;
	public int width;
	public int height;
	public float noiseScale;

	private float tessDistStep;
	private LODFace[] lodFaces = new LODFace[6];
	private Generator noise;


	static Vec3f[] positions = new Vec3f[] {
		new Vec3f(1, 1, 1).Normalized(),
		new Vec3f(1, 1, -1).Normalized(),
		new Vec3f(-1, 1, -1).Normalized(),
		new Vec3f(-1, 1, 1).Normalized(),
		new Vec3f(1, -1, 1).Normalized(),
		new Vec3f(1, -1, -1).Normalized(),
		new Vec3f(-1, -1, -1).Normalized(),
		new Vec3f(-1, -1, 1).Normalized()
	};

	static Vector2[] uvs = new Vector2[] {
		new Vector2 (0, 0),
		new Vector2 (0, 1),
		new Vector2 (1, 0),
		new Vector2 (1, 1)
	};

	static Vertex[] vertices = new Vertex[] {
		// Face 0 : +Y
		new Vertex(positions[0], uvs[0], positions[0]),
		new Vertex(positions[1], uvs[2], positions[1]),
		new Vertex(positions[2], uvs[3], positions[2]),
		new Vertex(positions[2], uvs[3], positions[2]),
		new Vertex(positions[3], uvs[1], positions[3]),
		new Vertex(positions[0], uvs[0], positions[0]),

		// Face 1 : +X
		new Vertex(positions[0], uvs[0], positions[0]),
		new Vertex(positions[4], uvs[2], positions[4]),
		new Vertex(positions[5], uvs[3], positions[5]),
		new Vertex(positions[5], uvs[3], positions[5]),
		new Vertex(positions[1], uvs[1], positions[1]),
		new Vertex(positions[0], uvs[0], positions[0]),

		// Face 2 : -Z
		new Vertex(positions[1], uvs[0], positions[1]),
		new Vertex(positions[5], uvs[2], positions[5]),
		new Vertex(positions[6], uvs[3], positions[6]),
		new Vertex(positions[6], uvs[3], positions[6]),
		new Vertex(positions[2], uvs[1], positions[2]),
		new Vertex(positions[1], uvs[0], positions[1]),

		// Face 3 : -X
		new Vertex(positions[2], uvs[0], positions[2]),
		new Vertex(positions[6], uvs[2], positions[6]),
		new Vertex(positions[7], uvs[3], positions[7]),
		new Vertex(positions[7], uvs[3], positions[7]),
		new Vertex(positions[3], uvs[1], positions[3]),
		new Vertex(positions[2], uvs[0], positions[2]),

		// Face 4 : +Z
		new Vertex(positions[3], uvs[0], positions[3]),
		new Vertex(positions[7], uvs[2], positions[7]),
		new Vertex(positions[4], uvs[3], positions[4]),
		new Vertex(positions[4], uvs[3], positions[4]),
		new Vertex(positions[0], uvs[1], positions[0]),
		new Vertex(positions[3], uvs[0], positions[3]),

		// Face 5 : -Y
		new Vertex(positions[4], uvs[0], positions[4]),
		new Vertex(positions[7], uvs[2], positions[7]),
		new Vertex(positions[6], uvs[3], positions[6]),
		new Vertex(positions[6], uvs[3], positions[6]),
		new Vertex(positions[5], uvs[1], positions[5]),
		new Vertex(positions[4], uvs[0], positions[4])
	};

	static int[] indices = new int[] {
		0, 1, 2, // triangle 0 
		0, 2, 3, // triangle 1
		0, 4, 5,
		0, 5, 1,
		1, 5, 6,
		1, 6, 2,
		2, 6, 7,
		2, 7, 3,
		3, 7, 4,
		3, 4, 0,
		4, 6, 5,
		4, 7, 6
	};

	static Triangle[] triangles = new Triangle[] {
		new Triangle(vertices[0], vertices[1], vertices[2]),
		new Triangle(vertices[3], vertices[4], vertices[5]),
		new Triangle(vertices[6], vertices[7], vertices[8]),
		new Triangle(vertices[9], vertices[10], vertices[11]),
		new Triangle(vertices[12], vertices[13], vertices[14]),
		new Triangle(vertices[15], vertices[16], vertices[17]),
		new Triangle(vertices[18], vertices[19], vertices[20]),
		new Triangle(vertices[21], vertices[22], vertices[23]),
		new Triangle(vertices[24], vertices[25], vertices[26]),
		new Triangle(vertices[27], vertices[28], vertices[29]),
		new Triangle(vertices[30], vertices[31], vertices[32]),
		new Triangle(vertices[33], vertices[34], vertices[35])
	};

	static Triangle[] side0 = {
		triangles[0],
		triangles[1]
	};

	static Triangle[] side1 = {
		triangles[2],
		triangles[3]
	};

	static Triangle[] side2 = {
		triangles[4],
		triangles[5]
	};

	static Triangle[] side3 = {
		triangles[6],
		triangles[7]
	};

	static Triangle[] side4 = {
		triangles[8],
		triangles[9]
	};

	static Triangle[] side5 = {
		triangles[10],
		triangles[11]
	};

	static Triangle[][] sides = {
		side0,
		side1,
		side2,
		side3,
		side4,
		side5
	};

	private Geometry geom;
	private int counter = 0;

	[ContextMenu("Reload")]
	void Reload() 
	{
		Debug.Log("Loading Geometry");
		Start();
	}

	// Use this for initialization
	void Start() {
		noise = new Scale (new GradientNoise (seed), noiseScale, noiseScale, noiseScale);

		tessDistStep = (farTessDist - nearTessDist) / (maxTessLevel - minTessLevel);

		for (int i = 0; i < 6; i++) {
			lodFaces[i] = new LODFace(sides[i], cutoff);
			faces [i].GetComponent<Renderer> ().material = mat;

			lodFaces[i].UpdateTexture(width, height, noise, faces[i].GetComponent<Renderer>().material);
		}

		//faces [0].GetComponent<Renderer> ().material = new Material (Shader.Find ("Diffuse"));
		//faces [0].GetComponent<Renderer> ().material.SetTexture ("_MainTex", albedo);
		//faces [0].GetComponent<Renderer> ().material.SetTexture ("_BumpMap", bump);
		for (int i = 0; i < 6; i++) {
			UpdateTessLevel (minTessLevel, i);
			AssignMesh (i);
		}

		//faces[5].GetComponent<Renderer>().material.mainTexture = lodFaces[5].UpdateTexture(width, height, noise);
	}
	
	// Update is called once per frame
	void Update() {
		float distance = (transform.position - target.transform.position).magnitude;
		int desiredTessLevel = GetDesiredTessLevel (distance);


		for (int i = 0; i < 6; i++) {
			if (desiredTessLevel > lodFaces[i].GetTessLevel ()) {
				UpdateTessLevel (desiredTessLevel, i);
				AssignMesh (i);
			}
		}
	}

	int GetDesiredTessLevel(float distance) {
		int level = maxTessLevel - (int)((distance - nearTessDist) / tessDistStep);
		if (level < minTessLevel)
			level = minTessLevel;

		return level;
	}


	void UpdateTessLevel(int desiredTessLevel, int index) {
		if (desiredTessLevel > lodFaces[index].GetTessLevel ()) {
			while (desiredTessLevel != lodFaces[index].GetTessLevel()) {
				lodFaces[index].Tessellate ();
			}
		} 
//			else {
//				while (desiredTessLevel != geom.GetTessLevel()) {
//					geom.UnTessellate ();
//				}
//			}

	}


	void AssignMesh(int index) {
		faces [index].GetComponent<MeshFilter> ().mesh = lodFaces [index].ToMesh ();
	}
}
