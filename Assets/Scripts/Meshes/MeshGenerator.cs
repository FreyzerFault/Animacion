using System;
using JetBrains.Annotations;
using UnityEngine;

public static class MeshGenerator
{
	public static MeshData generateMesh(int width, int height)
	{
		MeshData data = new MeshData(width, height);

		// La malla la creamos centrada en 0:
		float initX = (width - 1) / -2f;
		float initY = (height - 1) / -2f;

		int vertIndex = 0;
		for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			data.vertices[vertIndex] = new Vector3(initX + x, 0, initY + y);
			data.uvs[vertIndex] = new Vector2((float)x / width, (float)y / height);

			// Ignorando la ultima fila y columna de vertices, añadimos los triangulos
			if (x < width - 1 && y < height - 1)
			{
				data.AddTriangle(vertIndex, vertIndex + width, vertIndex + width + 1);
				data.AddTriangle(vertIndex + width + 1, vertIndex + 1, vertIndex);
			}

			vertIndex++;
		}

		return data;
	}

	public static MeshData generateSphericalMesh(int radius, int sections)
	{
		MeshData data = new MeshData(sections, sections);
		
		int vertIndex = 0;
		for (int y = 0; y < sections; y++)
		for (int x = 0; x < sections; x++)
		{
			data.vertices[vertIndex] = new Vector3(Mathf.Cos(x) * Mathf.Cos(y), Mathf.Sin(x), Mathf.Sin(y));
			data.uvs[vertIndex] = new Vector2((float)x / sections, (float)y / sections);

			// Ignorando la ultima fila y columna de vertices, añadimos los triangulos
			if (x < sections - 1 && y < sections - 1)
			{
				data.AddTriangle(vertIndex, vertIndex + sections, vertIndex + sections + 1);
				data.AddTriangle(vertIndex + sections + 1, vertIndex + 1, vertIndex);
			}

			vertIndex++;
		}

		return data;
	}
}



public class MeshData
{
	private int width;
	private int height;

	public Vector3[] vertices;
	public int[] triangles;

	// Redundante, para usar o Colores o Textura
	public Vector2[] uvs;

	public MeshData(int meshWidth, int meshHeight)
	{
		width = meshWidth;
		height = meshHeight;

		vertices = new Vector3[width * height];
		uvs = new Vector2[width * height];
		triangles = new int[(width - 1) * (height - 1) * 6];
	}

	public MeshData(MeshData data)
	{
		width = data.width;
		height = data.height;

		vertices = new Vector3[width * height];
		uvs = new Vector2[width * height];
		triangles = new int[(width - 1) * (height - 1) * 6];

		data.vertices.CopyTo(vertices, 0);
		data.triangles.CopyTo(triangles, 0);
		data.uvs.CopyTo(uvs, 0);
	}

	private int triIndex = 0;

	public void AddTriangle(int a, int b, int c)
	{
		if (a >= vertices.Length || b >= vertices.Length || c >= vertices.Length)
		{
			Debug.Log("Triangle out of Bounds!!! " + vertices.Length + " Vertices. Triangle(" + a + ", " + b + ", " +
			          c + ")");
			return;
		}

		triangles[triIndex + 0] = a;
		triangles[triIndex + 1] = b;
		triangles[triIndex + 2] = c;
		triIndex += 3;
	}

	// Pick & Pull dentro de un radio de efecto, con una fuerza de deformacion, y con un factor de escala k
	// La atenuacion de la deformacion se reduce conforme a la distancia del vertice RAIZ, segun k
	public MeshData PickAndPull(Vector3 pos, Vector3 deformation, float effectRadius, float scaleFactor, float max, float min)
	{
		MeshData newData = new MeshData(this);

		// Buscamos el Vertice mas cercano a la posicion, va a ser el VERTICE RAIZ
		float minDist = Mathf.Infinity;
		int rootVertIndex = getNearVertex(pos);

		Vector3 rootV = vertices[rootVertIndex];
		for (int i = 0; i < vertices.Length; i++)
		{
			// Distancia respecto al Vertice RAIZ
			float distance = (vertices[i] - rootV).sqrMagnitude;

			// Si entra dentro del radio de efecto recibe una fuerza
			if (distance < effectRadius)
			{
				float force = Mathf.Pow( 1f - distance / (effectRadius + 1), 1 + Mathf.Abs(scaleFactor) );
				
				newData.vertices[i] += deformation * force;

				// Limitamos la altura a un maximo y minimo
				newData.vertices[i].y = Mathf.Clamp(newData.vertices[i].y, min, max);
			}
		}

		return newData;
	}

	public int getNearVertex(Vector3 pos)
	{
		// Buscamos el Vertice mas cercano a la posicion
		float minDist = Mathf.Infinity;
		int index = 0;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector2 v2 = new Vector2(vertices[i].x, vertices[i].z);
			Vector2 p2 = new Vector2(pos.x, pos.z);

			//float distance = (vertices[i] - pos).sqrMagnitude;
			float distance = (v2 - p2).sqrMagnitude;

			if (distance < minDist)
			{
				minDist = distance;
				index = i;
			}
		}

		return index;
	}

	public float[,] getHeightMap()
	{
		// Primero calculamos el Maximo y Minimo
		//Vector2 minmax = getMinMaxHeigth();
		//float min = minmax.x, max = minmax.y;
		float max = 40;
		float min = -40;

		// Y calculamos la altura como una interpolacion entre el maximo y minimo
		float[,] heightMap = new float[width, height];

		for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			float vertHeight = vertices[x + y * width].y;
			heightMap[x, y] = Mathf.InverseLerp(min, max, vertHeight);
		}

		return heightMap;
	}

	public Vector2 getMinMaxHeigth()
	{
		float max = Mathf.NegativeInfinity, min = Mathf.Infinity;

		for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			float vertHeight = vertices[x + y * width].y;
			min = Mathf.Min(min, vertHeight);
			max = Mathf.Max(max, vertHeight);
		}

		return new Vector2(min, max);
	}

	// Este Metodo no puede hacerse en otro Hilo
	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh
		{
			vertices = vertices,
			triangles = triangles,
			uv = uvs,
		};

		mesh.RecalculateNormals();

		return mesh;
	}
	
}