using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings {
    public int octaves = 3; // layers number
    public float lacunarity = 2f; // frequency
    public float persistence = 0.5f; // amplitude
    public float strength = 1f;

    public float minValue = 0f;
    public float baseLacunarity = 1f;
    public Vector3 phaseOrigin = new Vector3(0, 0, 0);

    public bool activated = true;
}