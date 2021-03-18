using Unity;
using UnityEngine;
using System.Collections;

public class Universe : MonoBehaviour {
    public BodyTypePlanet[] bodyPlanetList;

    private ArrayList bodyListGo;
    private ArrayList bodyList;


    private void OnEnable() {
        bodyListGo = new ArrayList();
        bodyList   = new ArrayList();

        this.generateBody();
    }


    private void generateBody() {
        // Add planets to bodyList
        for (int i = 0; i < this.bodyPlanetList.Length; i++) {
            BodyTypePlanet t = this.bodyPlanetList[i];
            GameObject go = new GameObject(t.name);

            go.transform.parent = gameObject.transform;
            go.AddComponent<Planet>();
            go.GetComponent<Planet>().initialize(
                t.position, t.initialVelocity, t.scale, t.mass, t.rotationAxis, t.rotationPulsation,
                t.useLOD, t.threshold, t.chunkTargetLevel, t.chunkDensity, t.destroyIterationMaxCount,
                t.terrainHeight, t.waterLevel, t.noiseSettings, t.terrainGradient
            );

            bodyListGo.Add(go);
            bodyList  .Add((Body) go.GetComponent<Body>());
        }
    }


    public Body[] getBodys() {
        return (Body[]) this.bodyList.ToArray(typeof(Body));
    }

    public Body getNearestBody(Vector3 position) {
        return (Body) this.bodyList[0];
    }







    [System.Serializable]
    public class BodyType {
        [Header("Main parameters")]
        public string name;

        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 initialVelocity = new Vector3(0, 0, 0);
        public int scale = 1000;

        public float mass = 1e8f;
        public Vector3 rotationAxis = new Vector3(0, 0, 0);
        public float rotationPulsation = 10f;
    }


    [System.Serializable]
    public class BodyTypePlanet : BodyType {
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
        public int chunkDensity = 10;
        public int destroyIterationMaxCount = 10;


        [Header("Terrain Configuration")]
        public float terrainHeight = 10;
        public float waterLevel = 0.1f;
        public NoiseSettings[] noiseSettings;
        public Gradient terrainGradient;
    }
}