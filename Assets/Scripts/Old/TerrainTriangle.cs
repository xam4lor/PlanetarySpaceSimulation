using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTriangle {
	public int resolution;
	private Mesh mesh;

	public TerrainTriangle(int resolution, Mesh mesh) {
		this.resolution = resolution;
		this.mesh = mesh;
	}


	public void constructMesh(Vector3 rotation, Vector3 translation, float scale) {
		Vector3 rotVector = rotation * 180 / Mathf.PI;
		Quaternion rot = Quaternion.Euler(rotVector.x, rotVector.y, rotVector.z);

		// False
		int[] triangles = new int[3 * (resolution + 1) * (resolution + 1)]; // Triangle Number
		Vector3[] vertices = new Vector3[(resolution + 2) * (resolution + 3) / 2]; // Sum_k (k+2)

		int indexCurVertex = 0;
		int indexCurTriangles = 0;
		int lastIndexCurTriangles = 0;

		// Generate vertices and triangles for each basis triangle
		for (int i = 0; i < resolution + 2; i++) { // For each layer in triangle, going from top to bottom
			float yPos = 1 - (float) 1 / (resolution + 1) * i; // Invert
			int drawVerticesNumber = i + 1;

			float xPos = (float) 1 / (2 * resolution + 2) * (resolution + 1 - i);

			if (i == 0) { // top vertex
                vertices[0] = getVector(xPos, yPos, rot, translation, scale);
				continue;
			}

			// foreach i+2 vertices for layer i
			for (int j = 0; j < drawVerticesNumber; j++) {
				// Vertex
                vertices[indexCurVertex + j + 1] = getVector(xPos, yPos, rot, translation, scale);

				// Triangles
				// Normal rotation triangle (not if in right boundary)
				if (j != drawVerticesNumber - 1) {
					triangles[indexCurTriangles + 0] = indexCurVertex + j + 1 + 0;
					triangles[indexCurTriangles + 1] = indexCurVertex + j + 1 + 1;
					triangles[indexCurTriangles + 2] = lastIndexCurTriangles + j + 0;
					indexCurTriangles += 3;

					// Flipped triangle (only if not in left boundary)
					if (j != 0) {
						triangles[indexCurTriangles + 0] = indexCurVertex + j + 1 + 0;
						triangles[indexCurTriangles + 1] = lastIndexCurTriangles + j + 0;
						triangles[indexCurTriangles + 2] = lastIndexCurTriangles + j - 1;
						indexCurTriangles += 3;
					}
				}

				xPos += (float) 1 / (resolution + 1);
			}

			xPos += (float) 1 / (2 * resolution + 2);

			lastIndexCurTriangles = i * (i + 1) / 2;
			indexCurVertex += drawVerticesNumber;
		}

		/* vertices = new Vector3[]{new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0)};
		triangles = new int[]{1, 2, 0, 0, 2, 3}; */

		// Computes Mesh
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}



	private void rotateAroundAxis(string axis, float angle, ref Vector3 vec) {
		float cosA = Mathf.Cos(angle);
		float sinA = Mathf.Sin(angle);

		if (axis == "Y") {
            vec.y = cosA * vec.y - sinA * vec.z;
            vec.z = sinA * vec.y + cosA * vec.z;
		}
		else if (axis == "X") {
            vec.x =  cosA * vec.x + sinA * vec.z;
            vec.z = -sinA * vec.x + cosA * vec.z;
		}
		else {
            vec.x = cosA * vec.x - sinA * vec.y;
            vec.y = sinA * vec.x + cosA * vec.y;
		}
	}


    private Vector3 getVector(float xPos, float yPos, Quaternion rotation, Vector3 translation, float scale) {
		float zPos = 1;

		Vector3 v = new Vector3(xPos - 0.5f, yPos, zPos);
		return (rotation * v + translation) * scale;
	}
}