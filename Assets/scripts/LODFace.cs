using UnityEngine;

using System.IO;

using Universe.Math;
using CoherentNoise;

public class LODFace {

	Triangle[] triangles;
	private int tessLevel;
	private Vec3f[,] vertexMap; // for texture generation
	private float radius = 1;
	private float cutoff;
	private const TextureMode textureMode = TextureMode.CUBE;

	private const bool NORMALIZE = true;
	private const bool POS_TEST = false;
	private const float EXTRUSION = 10.0f;
	private Color32 flatNormal = new Color32 (128, 128, 255, 255);
	private static int count = 0;


	public enum TextureMode
	{
		CUBE,
		TESSELLATED
	}

	public LODFace(Triangle[] triangles, float cutoff) {
		this.triangles = triangles;
		this.tessLevel = 0;
		this.cutoff = cutoff;
		InitializeVertexMap ();
	} 

	private void InitializeVertexMap() {
		vertexMap = new Vec3f[2, 2];
		vertexMap [0, 0] = triangles [0].GetVertex (0).Position;
		vertexMap [1, 0] = triangles [0].GetVertex (1).Position;
		vertexMap [1, 1] = triangles [0].GetVertex (2).Position;
		vertexMap [0, 1] = triangles [1].GetVertex (1).Position;
	}

	public Mesh ToMesh() {
		Mesh mesh = new Mesh ();
		int numTris = triangles.Length;
		Vector3[] vertices = new Vector3[numTris * 3];
		Vector2[] uvs = new Vector2[numTris * 3];
		int[] indices = new int[numTris * 3];
		Vector3[] normals = new Vector3[numTris * 3];
		int index = 0;

		for (int i = 0; i < triangles.Length; i++) {
			for (int j = 0; j < 3; j++) {
				vertices [index] = Utils.ToVector3 (triangles [i].GetVertex (j).Position);
				uvs [index] = triangles [i].GetVertex (j).UV;
				indices [index] = index;
				normals [index] = Utils.ToVector3 (triangles [i].GetVertex (j).Normal);
				index++;
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = indices;
		mesh.normals = normals;

		return mesh;
	}

	public void Tessellate() {
		TessellateMesh ();
		TessellateVertexMap ();
		tessLevel++;
	}

	private void TessellateMesh() {
		int index = 0;
		Triangle[] newTriangles = new Triangle[triangles.Length * 2];
		foreach (Triangle triangle in triangles) {
			Vertex v0 = triangle.GetVertex(0);
			Vertex v1 = triangle.GetVertex(1);
			Vertex v2 = triangle.GetVertex(2);
			Vec3f midPos = ((v0.Position + v2.Position) / 2).Normalized();
			Vector2 midUV = ((v0.UV + v2.UV) / 2);
			midPos.Normalize ();
			Vertex mid = new Vertex(midPos, midUV, midPos);
			Triangle t0 = new Triangle(v1, mid, v0);
			Triangle t1 = new Triangle(v2, mid, v1);
			newTriangles[index++] = t0;
			newTriangles[index++] = t1;
		}
		triangles = newTriangles;
	}

	private void TessellateVertexMap() {
		if (((tessLevel + 1) % 2) == 0) { // next tess level is even
			int currentSize = vertexMap.GetLength (0);

			// add vert/horiz
			for (int i = 0; i < currentSize; i++) {
				for (int j = 0; j < currentSize; j++) {
					if (vertexMap [i, j] == null) {
						if (i % 2 == 0) {
							vertexMap [i, j] = CalculateMidValue (vertexMap [i, j - 1], vertexMap [i, j + 1]);
						} else {
							vertexMap [i, j] = CalculateMidValue (vertexMap [i - 1, j], vertexMap [i + 1, j]);
						}
					}
				}
			}
		} else { // next tess level is odd
			int currentSize = vertexMap.GetLength (0);
			int nextSize = currentSize * 2 - 1;
			Vec3f[,] newMap = new Vec3f[nextSize, nextSize];

			// transfer values
			for (int i = 0; i < currentSize; i++) {
				for (int j = 0; j < currentSize; j++) {
					newMap [i * 2, j * 2] = vertexMap [i, j];
				}
			}

			// add diagonal
			for (int i = 1; i < nextSize; i += 2) {
				for (int j = 1; j < nextSize; j += 2) {
					if (newMap [i, j] == null) {
						newMap [i, j] = CalculateMidValue (newMap [i - 1, j - 1], newMap [i + 1, j + 1]);
					}
				}
			}

			vertexMap = newMap;
		}
	}

	public void UpdateTexture(int width, int height, Generator gen, Material mat) {
		switch (textureMode) {
		case TextureMode.CUBE:
			UpdateTextureCube (width, height, gen, mat);
			return;

		case TextureMode.TESSELLATED:
			UpdateTextureTessellated (width, height, gen, mat);
			return;
		}
	}

	public void UpdateTextureTessellated(int width, int height, Generator gen, Material mat) {
		int length = tessLevel / 2 + 1;
		int vertices = length + 1;
		bool evenTess = tessLevel % 2 == 0;
		int dLength = evenTess ? 1 : 2;
		int pxPerEdgeWidth = width / length;
		int pxPerEdgeHeight = height / length;
		int finalWidth = pxPerEdgeWidth * length;
		int finalHeight = pxPerEdgeHeight * length;
		int area = finalWidth * finalHeight;
		Color32[] colors = new Color32[area];
		float[,] heightmap = new float[finalWidth, finalHeight];



		for (int y = 0; y < length; y ++) {
			for (int x = 0; x < length; x ++) {
				int xIndex = x * dLength;
				int yIndex = y * dLength;

				Vec3f start = vertexMap [xIndex, yIndex];
				int startIndex = x * pxPerEdgeWidth + y * pxPerEdgeHeight * finalWidth;
				Vec3f vx = vertexMap [xIndex + dLength, yIndex] - start;
				Vec3f vy = vertexMap [xIndex, yIndex + dLength] - start;

				Vec3f dxv = vx / pxPerEdgeWidth;
				Vec3f dyv = vy / pxPerEdgeHeight;

				int startX = x * pxPerEdgeWidth;
				int startY = y * pxPerEdgeHeight;

				// Build mainTex color array and compile heightmap
				for (int yy = 0; yy < pxPerEdgeHeight; yy++) {
					Vec3f subStart = start;
					int subIndex = startIndex;
					for (int xx = 0; xx < pxPerEdgeWidth; xx++) {

						if (POS_TEST) {
							colors [subIndex++] = GetColorFromPos (subStart);
						} else if (NORMALIZE) {
							Vec3f norm = subStart.Normalized ();
							float noiseValue = gen.GetValue (norm.X, norm.Y, norm.Z);
							heightmap [startX + xx, startY + yy] = noiseValue;
							colors [subIndex++] = GetColor (noiseValue);
						} else {
							colors [subIndex++] = GetColor (gen.GetValue (subStart.X, subStart.Y, subStart.Z));
						}
						subStart = subStart + dxv;
					}
					start = start + dyv;
					startIndex += finalWidth;
				}


			}
		}

		Texture2D mainTex = new Texture2D (finalWidth, finalHeight, TextureFormat.RGBA32, false);
		mainTex.wrapMode = TextureWrapMode.Clamp;
		mainTex.SetPixels32 (colors);
		mainTex.Apply ();

		/*
		 * Edges messed up because of sampling off edge
		 */ 
		Texture2D bump = new Texture2D (finalWidth, finalHeight, TextureFormat.ARGB32, false);
		bump.wrapMode = TextureWrapMode.Clamp;
		bump.SetPixels32 (ComputeNormalMap(width, height, heightmap));
		bump.Apply();

		mat.SetTexture ("_MainTex", mainTex);
		mat.SetTexture ("_BumpMap", bump);
	}

	private void UpdateTextureCube(int width, int height, Generator gen, Material mat) {
		float[,] heightmap = CalculateHeightMapXY (width, height, gen);

		int area = width * height;
		Color32[] mainTexColors = new Color32[area];
		Color32[] normalColors = new Color32[area];
		int index = 0;

		for (int y = 1; y < height + 1; y++) {
			for (int x = 1; x < width + 1; x++) {
				mainTexColors [index] = GetColor (heightmap [x, y]);
				normalColors [index] = CalculateNormal (x, y, width + 2, height + 2, heightmap);
				index++;
			}
		}

		Texture2D mainTex = new Texture2D (width, height, TextureFormat.RGBA32, false);
		mainTex.wrapMode = TextureWrapMode.Clamp;
		mainTex.SetPixels32 (mainTexColors);
		mainTex.Apply ();

		Texture2D bump = new Texture2D (width, height, TextureFormat.RGBA32, false);
		bump.wrapMode = TextureWrapMode.Clamp;
		bump.SetPixels32 (normalColors);
		bump.Apply();

		mat.SetTexture ("_MainTex", mainTex);
		mat.SetTexture ("_BumpMap", bump);
	}

	private float[,] CalculateHeightMapRadially(int width, int height, Generator gen) {
		float[,] heightmap = new float[width + 2, height + 2];

		Vector3 start = Utils.ToVector3(vertexMap [0, 0]);
		Vector3 vx = Utils.ToVector3(vertexMap [1, 0]) - start;
		Vector3 vy = Utils.ToVector3(vertexMap [0, 1]) - start;

		Quaternion qStart = Quaternion.LookRotation (start);
		Quaternion qx = Quaternion.LookRotation (vx);
		Quaternion qy = Quaternion.LookRotation (vy);

		float dx = 1.0f / width;
		float dy = 1.0f / height;

		Quaternion rotationX = Quaternion.Slerp (qStart, qx, dx);
		Quaternion rotationY = Quaternion.Slerp (qStart, qy, dy);

		Matrix4x4 rotMatX = Matrix4x4.TRS (Vector3.zero, rotationX, Vector3.one);
		Matrix4x4 rotMatY = Matrix4x4.TRS (Vector3.zero, rotationY, Vector3.one);

		start = rotMatX.inverse.MultiplyVector (start);
		start = rotMatY.inverse.MultiplyVector (start);

		for (int y = 0; y < height + 2; y++) {
			Vector3 sub = start;
			for (int x = 0; x < height + 2; x++) {
				heightmap [x, y] = gen.GetValue (sub.x, sub.y, sub.z);
				sub = rotMatX.MultiplyVector (sub);
			}
			start = rotMatY.MultiplyVector (start);
		}

		return heightmap;
	}

	private float[,] CalculateHeightMapXY(int width, int height, Generator gen) {
		float[,] heightmap = new float[width + 2, height + 2];

		Vec3f start = vertexMap [0, 0];
		Vec3f vx = vertexMap [1, 0] - start;
		Vec3f vy = vertexMap [0, 1] - start;

		Vec3f dxv = vx / width;
		Vec3f dyv = vy / height;

		Vec3f current = start - dxv - dyv;

		for (int y = 0; y < height + 2; y++) {
			Vec3f sub = current;
			for (int x = 0; x < height + 2; x++) {
				Vec3f normalized = sub.Normalized ();
				heightmap [x, y] = gen.GetValue (normalized.X, normalized.Y, normalized.Z);
				sub = sub + dxv;
			}
			current = current + dyv;
		}

		return heightmap;
	}

	private Color32[] ComputeNormalMap(int width, int height, float[,] heightmap) {
		Color32[] colors = new Color32[width * height];
		int index = 0;

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colors [index++] = CalculateNormal (x, y, width, height, heightmap);
			}
		}

		return colors;
	}

	private float GetHeightFromMap(int x, int y, int width, int height, float[,] heightmap) {
		if (x < 0) x = 0;
		if (y < 0) y = 0;
		if (x >= width) x = width - 1;
		if (y >= height) y = height - 1;
		
		return heightmap [x, y];
	}

	private Color32 CalculateNormal(int x, int y, int width, int height, float[,] heightmap) {
		float center = GetHeightFromMap (x, y, width, height, heightmap);
		if (center <= cutoff)
			return flatNormal;

		float up = GetHeightFromMap (x, y - 1, width, height, heightmap);
		float down = GetHeightFromMap (x, y + 1, width, height, heightmap);
		float left = GetHeightFromMap (x - 1, y, width, height, heightmap);
		float right = GetHeightFromMap (x + 1, y, width, height, heightmap);
		float upLeft = GetHeightFromMap (x - 1, y - 1, width, height, heightmap);
		float upRight = GetHeightFromMap (x + 1, y - 1, width, height, heightmap);
		float downLeft = GetHeightFromMap (x - 1, y + 1, width, height, heightmap);
		float downRight = GetHeightFromMap (x + 1, y + 1, width, height, heightmap);

		float vert = (down - up) * 2.0f + downRight + downLeft - upRight - upLeft;
		float horiz = (right - left) * 2.0f + upRight + downRight - upLeft - downLeft;
		float depth = 1.0f / EXTRUSION;
		float scale = 127.0f / Mathf.Sqrt(vert*vert + horiz*horiz + depth*depth);

		byte r = (byte)(128 - horiz * scale);
		byte g = (byte)(128 + vert * scale);
		byte b = (byte)(128 + depth * scale);
		byte a = 255;

		return new Color32(r, g, b, a);
	}

	private Color32 GetColorFromPos(Vec3f pos) {
		if (pos.Z > 0)
			return new Color32 (255, 0, 0, 255);
		return new Color (0, 0, 255, 255);
	}

	private Color32 GetColor(float value) {
		if (value > cutoff) {
			return new Color32 (0, 255, 0, 255);
		}

		return new Color32 (0, 0, 255, 255);
//		byte comp = (byte)(((value + 1) / 2) * 255);
//
//		return new Color32 (comp, comp, comp, 255);
	}

	private Vec3f CalculateMidValue(Vec3f v0, Vec3f v1) {
		return ((v0 + v1) / 2).Normalized() * radius;
	}

	public int GetTessLevel() {
		return tessLevel;
	}
} 

