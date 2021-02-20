using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Planet : MonoBehaviour {
	public int planetScale = 10;
    public int chunkDensity = 10;

	public float[] threshold = new float[] {
		2,    // threshold 0
		1.5f, // threshold 1
		1,    // threshold 2
		0.7f, // threshold 3
		0.5f,
		0.1f,
		0
	};

	public int destroyIterationMaxCount = 10;


	[SerializeField, HideInInspector]
	private GameObject chunksContainer;
	
    [SerializeField, HideInInspector]
	private PlanetChunks[] planetChunks;
	private PlayerLastStats lastPlayerStats;

	private Stack<GameObject> destroyGORef;

    private int lastScale = 10;


	private void OnEnable() {
		this.destroyGORef = new Stack<GameObject>();
		this.planetChunks = new PlanetChunks[6];
		this.lastPlayerStats = new PlayerLastStats(Vector3.one, 0f, 0, "null");

		this.generate();

		StartCoroutine(PlanetGenerationLoop());
	}

	private void Update() {
        if (lastScale != planetScale) {
			lastScale = planetScale;
			this.generate();
		}
	}




	private void generate() {
		// Initialize planet chunks
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}

		chunksContainer = new GameObject("Planet Chunks");
		chunksContainer.transform.parent = transform;


		planetChunks[0] = new PlanetChunks(this, gameObject.transform.position, "upX", chunksContainer);
		planetChunks[1] = new PlanetChunks(this, gameObject.transform.position, "downX", chunksContainer);
		planetChunks[2] = new PlanetChunks(this, gameObject.transform.position, "upY", chunksContainer);
		planetChunks[3] = new PlanetChunks(this, gameObject.transform.position, "downY", chunksContainer);
		planetChunks[4] = new PlanetChunks(this, gameObject.transform.position, "upZ", chunksContainer);
		planetChunks[5] = new PlanetChunks(this, gameObject.transform.position, "downZ", chunksContainer);
	}

	public float getAltitudeAt(Vector3 pos) {
		float scl = 10;
		return Mathf.PerlinNoise(pos.x / scl, pos.z / scl) / 15f;
	}


    // Generate mesh every second
    private IEnumerator PlanetGenerationLoop() {
        while (true) {
            this.handleChunksAsync();
            yield return new WaitForSeconds(0.1f);
        }
    }

	private void handleChunksAsync() {
        // Divide chunks
        for (int i = 0; i < planetChunks.Length; i++) {
            planetChunks[i].divideFromCenter(this.lastPlayerStats.pos, this.lastPlayerStats.distance);
        }
        // Destroy chunks
        for (int i = 0; i < destroyIterationMaxCount; i++) {
			if (this.destroyGORef.Count == 0)
				break;
            Destroy(this.destroyGORef.Pop());
        }
	}


	public void destroyGameObject(GameObject go) {
		this.destroyGORef.Push(go);
	}





	public float getScale() {
		return this.planetScale;
	}


	struct PlayerLastStats {
		public Vector3 pos;
		public float distance;
		public int chunkID;
		public string chunkName;

		public PlayerLastStats(Vector3 pos, float distance, int chunkID, string chunkName) {
			this.pos = pos;
			this.distance = distance;
			this.chunkID  = chunkID;
			this.chunkName = chunkName;
		}
	};
	private void onPlayerSeeChunk(object[] obj) {
		this.lastPlayerStats = new PlayerLastStats((Vector3)obj[0], (float)obj[1], (int)obj[2], (string)obj[3]);
	}
}