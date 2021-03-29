using Unity;
using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {
    [Header("Planet settings")]
    public float mass = 1e8f;
    public float bodyScale = 6300;
    public Vector3 rotationAxis = new Vector3(2f, 1.3f, 5.1f);
    public float rotationPulsation = 3f;

    private Vector3 velocity;
    private Vector3 acceleration;
    private Universe universe;

    protected PlayerLastStats lastPlayerStats;
    protected float lastScale = 10;



    protected void OnEnable() {
        this.lastPlayerStats = new PlayerLastStats(Vector3.one, Vector3.one, 0f, 0, "null");
    }

    protected void Update() {
        // Units : km, kg
        this.transform.Rotate(rotationAxis.normalized, rotationPulsation * Time.deltaTime * universe.simulationSpeed, Space.World);

        this.transform.position += this.velocity * Time.deltaTime * universe.simulationSpeed;
        this.velocity += this.acceleration * Time.deltaTime * universe.simulationSpeed;

        this.acceleration = this.computeForces() / this.mass;
    }

    public void initialize(Universe universe, Vector3 position, Vector3 initialVelocity, float scale, float mass, Vector3 rotationAxis, float rotationPulsation) {
        this.universe = universe;
        
        this.transform.position = position;
        this.bodyScale = scale;
        this.mass = mass;
        this.rotationAxis = rotationAxis;
        this.rotationPulsation = rotationPulsation;
        this.velocity = initialVelocity;

        this.acceleration = Vector3.zero;

        // Trail
        if (universe.showBodyTrails) {
            GameObject ps = new GameObject("Particle System Trail");
            ps.transform.localScale = Vector3.one * 50;

            TrailRenderer psRenderer = ps.AddComponent<TrailRenderer>();
            psRenderer.time = 80f;
            psRenderer.minVertexDistance = 0.1f;
            psRenderer.alignment = LineAlignment.TransformZ;

            Gradient grad = new Gradient();
            GradientColorKey[] colorKey = new GradientColorKey[2];
            Color col = new Color(117f / 255f, 194f / 255f, 245f / 255f);
            colorKey[0].color = col;
            colorKey[0].time = 0.0f;
            colorKey[1].color = col;
            colorKey[1].time = 1.0f;

            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 0.0f;
            alphaKey[1].time = 1.0f;

            grad.colorKeys = colorKey;
            grad.alphaKeys = alphaKey;
            grad.mode = GradientMode.Blend;

            psRenderer.colorGradient = grad;
            psRenderer.material = new Material(Shader.Find("Sprites/Default"));

            ps.transform.parent = transform;
            ps.transform.localPosition = Vector3.zero;
        }
    }




    private Vector3 computeForces() {
        Vector3 forces = Vector3.zero;

        // Gravity
        Body[] bodys = this.universe.getBodys();
        for (int i = 0; i < bodys.Length; i++) {
            if (bodys[i] == this)
                continue;

            Vector3 r = this.transform.position - bodys[i].transform.position;
            forces += -(float) Constants.G * this.mass * bodys[i].mass * r / (r.sqrMagnitude*r.magnitude);
        }

        return forces;
    }

    public float getScale() {
        return this.bodyScale;
    }

    public void onPlayerSeeChunk(object[] obj) {
        this.lastPlayerStats = new PlayerLastStats((Vector3)obj[0], (Vector3)obj[1], (float)obj[2], (int)obj[3], (string)obj[4]);
    }
}