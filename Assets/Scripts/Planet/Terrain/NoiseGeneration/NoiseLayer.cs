using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseLayer {
    private NoiseSettings settings;
    private Noise noise;
    private Vector3[] octavesOffset;


    public NoiseLayer(NoiseSettings settings) {
        this.settings = settings;
    }

    public void initialize() {
        this.noise = new Noise(27092001);
        this.octavesOffset = new Vector3[settings.octaves];

        // Initialize octaves offset
        int range = 100000;
        for (int i = 0; i < settings.octaves; i++) {
            this.octavesOffset[i] = new Vector3(
                Random.Range(-range, range),
                Random.Range(-range, range),
                Random.Range(-range, range)
            ) + settings.phaseOrigin;
        }
    }

    public float evaluate(Vector3 pos) {
        if (!settings.activated)
            return 0;

        float noiseVal = 0;
        float frequency = settings.baseLacunarity;
        float amplitude = 1;

        for (int i = 0; i < octavesOffset.Length; i++) {
            float v = this.noise.Evaluate(pos * frequency + octavesOffset[i]);
            noiseVal += (v + 1) * 0.5f * amplitude;

            frequency *= settings.lacunarity;
            amplitude *= settings.persistence;
        }

        noiseVal = Mathf.Max(0, noiseVal - settings.minValue);
        return noiseVal * settings.strength;
    }
}