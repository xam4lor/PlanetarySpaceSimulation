using Unity;
using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {
    [Header("Planet settings")]
    public float mass = 1e8f;
    public int bodyScale = 6300;
    public Vector3 rotationAxis = new Vector3(2f, 1.3f, 5.1f);
    public float rotationPulsation = 3f;

    protected PlayerLastStats lastPlayerStats;
    protected int lastScale = 10;



    protected void OnEnable() {
        this.lastPlayerStats = new PlayerLastStats(Vector3.one, Vector3.one, 0f, 0, "null");
    }

    protected void Update() {
        // Units : km, kg
        this.transform.RotateAround(this.transform.position, rotationAxis, rotationPulsation * Time.deltaTime);
    }




    public float getScale() {
        return this.bodyScale;
    }

    public struct PlayerLastStats {
        public Vector3 playerPos;
        public Vector3 collisionPos;
        public float distance;
        public int chunkID;
        public string chunkName;

        public PlayerLastStats(Vector3 playerPos, Vector3 collisionPos, float distance, int chunkID, string chunkName) {
            this.playerPos = playerPos;
            this.collisionPos = collisionPos;
            this.distance = distance;
            this.chunkID = chunkID;
            this.chunkName = chunkName;
        }
    };

    public void onPlayerSeeChunk(object[] obj) {
        this.lastPlayerStats = new PlayerLastStats((Vector3)obj[0], (Vector3)obj[1], (float)obj[2], (int)obj[3], (string)obj[4]);
    }
}