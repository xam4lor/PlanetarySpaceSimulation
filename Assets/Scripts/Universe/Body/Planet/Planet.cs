using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Body {
    public float terrainHeight;
    public float waterLevel;
    public int chunkDensity;
    public bool useLOD;
    public float[] threshold;
    public int chunkTargetLevel;
    public int destroyIterationMaxCount;
    public NoiseSettings[] noiseSettings;
    public Gradient terrainGradient;


	[SerializeField, HideInInspector]
	private GameObject chunksContainer;
	
    [SerializeField, HideInInspector]
	private PlanetChunks[] planetChunks;
    private Stack<GameObject> destroyGORef;
	private TerrainGenerator terrainGenerator;



	new private void OnEnable() {
		base.OnEnable();
	}

	new private void Update() {
        base.Update();

        if (lastScale != bodyScale) {
			lastScale = bodyScale;
			this.generate();
		}
	}



	public void initialize(
		Vector3 position, Vector3 initialVelocity, int scale, float mass, Vector3 rotationAxis, float rotationPulsation,
		bool useLOD, float[] threshold, int chunkTargetLevel, int chunkDensity, int destroyIterationMaxCount,
        float terrainHeight, float waterLevel, NoiseSettings[] noiseSettings, Gradient terrainGradient
	) {
		base.initialize(position, initialVelocity, scale, mass, rotationAxis, rotationPulsation);

		this.useLOD = useLOD;
		this.threshold = threshold;
		this.chunkTargetLevel = chunkTargetLevel;
		this.chunkDensity = chunkDensity;
		this.destroyIterationMaxCount = destroyIterationMaxCount;
		this.terrainHeight = terrainHeight;
		this.waterLevel = waterLevel;
		this.noiseSettings = noiseSettings;
		this.terrainGradient = terrainGradient;

        this.terrainGenerator = new TerrainGenerator(noiseSettings, this);
        this.destroyGORef = new Stack<GameObject>();
        this.planetChunks = new PlanetChunks[6];

        this.generate();

        StartCoroutine(PlanetGenerationLoop());
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

		planetChunks[0] = new PlanetChunks(this, Vector3.zero, "upX", chunksContainer);
		planetChunks[1] = new PlanetChunks(this, Vector3.zero, "downX", chunksContainer);
		planetChunks[2] = new PlanetChunks(this, Vector3.zero, "upY", chunksContainer);
		planetChunks[3] = new PlanetChunks(this, Vector3.zero, "downY", chunksContainer);
		planetChunks[4] = new PlanetChunks(this, Vector3.zero, "upZ", chunksContainer);
		planetChunks[5] = new PlanetChunks(this, Vector3.zero, "downZ", chunksContainer);
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
}