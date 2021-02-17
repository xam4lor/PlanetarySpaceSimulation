using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetOld : MonoBehaviour {
	[Range(0, 100)]
	public int resolution = 0;

	[SerializeField, HideInInspector]
	private MeshFilter[] meshFilters;
    [SerializeField, HideInInspector]
	private GameObject meshContainer;
    private TerrainTriangle[] terrainTriangles;

	private void OnValidate() {
		// On script loaded
		this.initialize();
		this.generateMesh();
	}

	private void initialize() {
        if (meshFilters == null || meshFilters.Length == 0) {
            meshFilters = new MeshFilter[8];
        }
        terrainTriangles = new TerrainTriangle[8];

        if (meshContainer == null) {
        	meshContainer = new GameObject("Planet Mesh");
            meshContainer.transform.parent = transform;
		}

        for (int i = 0; i < terrainTriangles.Length; i++) {
            if (meshFilters[i] == null) {
                GameObject meshObj = new GameObject("Mesh " + (i + 1));
                meshObj.transform.parent = meshContainer.transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            terrainTriangles[i] = new TerrainTriangle(resolution, meshFilters[i].sharedMesh);
        }
	}

    private void generateMesh() {
        float delta = 0.25f;
        Vector3 angle = new Vector3(Mathf.PI / 6, Mathf.PI / 2, 0);
        terrainTriangles[0].constructMesh(new Vector3(-angle.x, 0, 0), new Vector3(0, 0, delta), 1);
        terrainTriangles[1].constructMesh(new Vector3(-angle.x, 1*angle.y, 0), new Vector3(delta, 0, 0), 1);
        terrainTriangles[2].constructMesh(new Vector3(-angle.x, 2*angle.y, 0), new Vector3(0, 0, -delta), 1);
        terrainTriangles[3].constructMesh(new Vector3(-angle.x, 3*angle.y, 0), new Vector3(-delta, 0, 0), 1);

        terrainTriangles[4].constructMesh(new Vector3(angle.x + Mathf.PI, 0, 0), new Vector3(0, 0, delta), 1);
        terrainTriangles[5].constructMesh(new Vector3(angle.x + Mathf.PI, 1 * angle.y, 0), new Vector3(delta, 0, 0), 1);
        terrainTriangles[6].constructMesh(new Vector3(angle.x + Mathf.PI, 2 * angle.y, 0), new Vector3(0, 0, -delta), 1);
        terrainTriangles[7].constructMesh(new Vector3(angle.x + Mathf.PI, 3 * angle.y, 0), new Vector3(-delta, 0, 0), 1);
    }
}
