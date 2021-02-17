using UnityEngine;

public class PlanetChunks {
	private GameObject parentGo;
	private QuadTree.Chunk mainChunk;

	public Quaternion localRotation;

	private Vector3 translation;
	private float scale;
	private Planet planet;

	private float lastPlayerDistance;

	public PlanetChunks(Planet planet, Vector3 translation, string localUp, GameObject parentGo)  {
		this.planet = planet;
		this.translation = translation;
		this.lastPlayerDistance = 10e10f;

		int id = -1;
		switch(localUp) {
			case "upX":
				localRotation = Quaternion.Euler(90, 0, 0);
				id = 0;
				break;
			case "downX":
				localRotation = Quaternion.Euler(-90, 0, 0);
				id = 1;
				break;

			case "upY":
				localRotation = Quaternion.Euler(0, 0, 0);
				id = 2;
				break;
			case "downY":
				localRotation = Quaternion.FromToRotation(Vector3.up, Vector3.down);
				id = 3;
				break;

			case "upZ":
				localRotation = Quaternion.Euler(0, 0, 90);
				id = 4;
				break;
			case "downZ":
				localRotation = Quaternion.Euler(0, 0, -90);
				id = 5;
				break;
		}
		this.parentGo = parentGo;

		this.mainChunk = new QuadTree.Chunk(this, new QuadTree.BoundingBox(0, 0, this.planet.getScale()), 0, -1, parentGo, null, id + "hunk ");
		this.mainChunk.subdivide();
	}

	public void playerSeeChunks(Vector3 pos, float distance, string chunkName) {
		// from [0] to [1], max computedTargetDepth
		int[] depthsTable = new int[]{10, 100, 1000, 10000, 100000, 1000000};
		int computedTargetDepth = 15; // max value

		for (int i = 0; i < depthsTable.Length; i++) {
            if (distance < depthsTable[i]) {
				break;
            }
            computedTargetDepth -= 1;
		}

/*         QuadTree.Chunk ch = this.getChunkWithName(chunkName);
        if (ch.depth <= computedTargetDepth) {
            ch.subdivide();
            ch.backPropagate();
        } */
	}

	public QuadTree.Chunk getChunkWithName(string chunkName) {
		return this.mainChunk.getChunkWithName(chunkName.Substring(6));
	}

	public void drawDebug() {
		this.mainChunk.drawDebug();
	}



	public Vector3 getTranslation() {
		return this.translation;
	}

	public float getScale() {
		return this.planet.getScale();
	}
}