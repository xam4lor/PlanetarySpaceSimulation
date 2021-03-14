using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
    [Header("Planet settings")]
	public float mass = 10e8f;
	public int planetScale = 10;
    public int chunkDensity = 10;
	public Vector3 rotationAxis = new Vector3(2f, 1.3f, 5.1f);
	public float rotationPulsation = 10f;


    [Header("Planet chunks")]
	public bool useLOD = false;
	public float[] threshold = new float[] {
		2,    // threshold 0
		1.5f, // threshold 1
		1,    // threshold 2
		0.7f, // threshold 3
		0.5f,
		0.1f,
		0
	};
	public int chunkTargetLevel = 3;
	public int destroyIterationMaxCount = 10;



	[Header("Terrain Configuration")]
    public float terrainHeight = 10;
    public NoiseSettings[] noiseSettings;
    private TerrainColorsSettings[] terrainColorsSettings;
	public Gradient terrainGradient;


	[SerializeField, HideInInspector]
	private GameObject chunksContainer;
	
    [SerializeField, HideInInspector]
	private PlanetChunks[] planetChunks;
    private Stack<GameObject> destroyGORef;


	private PlayerLastStats lastPlayerStats;
    private int lastScale = 10;


	private TerrainGenerator terrainGenerator;



	private void OnEnable() {
		this.terrainGenerator = new TerrainGenerator(noiseSettings, terrainColorsSettings, this);
		this.destroyGORef     = new Stack<GameObject>();
		this.planetChunks     = new PlanetChunks[6];
		this.lastPlayerStats  = new PlayerLastStats(Vector3.one, Vector3.one, 0f, 0, "null");

		this.generate();

		StartCoroutine(PlanetGenerationLoop());
	}

	private void Update() {
        if (lastScale != planetScale) {
			lastScale = planetScale;
			this.generate();
		}

		// Units : km, kg

		this.transform.RotateAround(this.transform.position, rotationAxis, rotationPulsation * Time.deltaTime);
	}




	public void generate() {
		// Initialize terrain generation
		this.terrainGenerator.initialize();

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
		return this.terrainGenerator.getAltitudeAt(pos);
	}

	public Color getColorAtAltitude(float z) {
        return this.terrainGenerator.getColorAtAltitude(z);
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
            planetChunks[i].divideFromCenter(this.lastPlayerStats.playerPos, this.lastPlayerStats.collisionPos, this.lastPlayerStats.distance);
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
		public Vector3 playerPos;
        public Vector3 collisionPos;
		public float distance;
		public int chunkID;
		public string chunkName;

		public PlayerLastStats(Vector3 playerPos, Vector3 collisionPos, float distance, int chunkID, string chunkName) {
			this.playerPos = playerPos;
            this.collisionPos = collisionPos;
			this.distance = distance;
			this.chunkID  = chunkID;
			this.chunkName = chunkName;
		}
	};
	private void onPlayerSeeChunk(object[] obj) {
		this.lastPlayerStats = new PlayerLastStats((Vector3)obj[0], (Vector3)obj[1], (float)obj[2], (int)obj[3], (string)obj[4]);
	}
}