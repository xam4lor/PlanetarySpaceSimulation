using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
	public int scale = 10;
    public static int chunkDensity = 10;

	[SerializeField, HideInInspector]
	private GameObject chunksContainer;
	
    [SerializeField, HideInInspector]
	private PlanetChunks[] planetChunks;
	private PlayerLastStats lastPlayerStats;

    private int lastScale = 10;


	private void OnEnable() {
		this.planetChunks = new PlanetChunks[6];
		this.lastPlayerStats = new PlayerLastStats(Vector3.one, 0f, 0, "null");

		this.generate();

		StartCoroutine(PlanetGenerationLoop());
	}

	private void Update() {
        if (lastScale != scale) {
			lastScale = scale;
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


	// Generate mesh every second
	private IEnumerator PlanetGenerationLoop() {
		while (true) {
			if (!this.lastPlayerStats.chunkName.Equals("null")) {
				this.planetChunks[this.lastPlayerStats.chunkID]
					.playerSeeChunks(this.lastPlayerStats.pos, this.lastPlayerStats.distance, this.lastPlayerStats.chunkName);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}


	public float getScale() {
		return this.scale;
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