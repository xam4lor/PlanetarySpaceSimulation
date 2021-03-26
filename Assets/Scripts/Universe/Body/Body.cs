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
        this.transform.Rotate(rotationAxis.normalized, rotationPulsation * Time.deltaTime, Space.World);
    }

    public void initialize(Vector3 position, Vector3 initialVelocity, int scale, float mass, Vector3 rotationAxis, float rotationPulsation) {
        this.transform.position = position;
        this.bodyScale = scale;
        this.mass = mass;
        this.rotationAxis = rotationAxis;
        this.rotationPulsation = rotationPulsation;
    }




    public float getScale() {
        return this.bodyScale;
    }

    public void onPlayerSeeChunk(object[] obj) {
        this.lastPlayerStats = new PlayerLastStats((Vector3)obj[0], (Vector3)obj[1], (float)obj[2], (int)obj[3], (string)obj[4]);
    }
}