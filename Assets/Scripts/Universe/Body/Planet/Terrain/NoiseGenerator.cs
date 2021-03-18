using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator {
    private NoiseLayer[] layer;

    public NoiseGenerator(NoiseSettings[] settings) {
        this.layer = new NoiseLayer[settings.Length];
        for (int i = 0; i < settings.Length; i++) {
            this.layer[i] = new NoiseLayer(settings[i]);
        }
    }

    public void initialize() {
        for (int i = 0; i < layer.Length; i++) {
            this.layer[i].initialize();
        }
    }

    public float evaluate(Vector3 pos) {
        float val = 0;
        for (int i = 0; i < layer.Length; i++) {
            val += this.layer[i].evaluate(pos);
        }
        return val;
    }
}