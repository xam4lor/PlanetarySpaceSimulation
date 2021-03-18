using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator {
    private float maxHeight = 0;
    private float minHeight = 10000000000;

    private NoiseGenerator noiseGenerator;
    private Planet planet;

    public TerrainGenerator(NoiseSettings[] settings, Planet planet) {
        this.noiseGenerator = new NoiseGenerator(settings);
        this.planet = planet;
        this.maxHeight = planet.terrainHeight;
    }

    public void initialize() {
        this.noiseGenerator.initialize();
    }

    public float getAltitudeAt(Vector3 unitarySpherePos) {
        float altitudePercent = 0;

        // Computes altitude
        altitudePercent = noiseGenerator.evaluate(unitarySpherePos);

        // Computes real altitude from percentage
        float realAltitude = altitudePercent * this.planet.terrainHeight * this.planet.getScale() / 100;
        if (realAltitude > maxHeight)
            maxHeight = realAltitude;
        if (realAltitude < minHeight)
            minHeight = realAltitude;

        if (altitudePercent <= planet.waterLevel)
            return planet.waterLevel * this.planet.terrainHeight * this.planet.getScale() / 100;

        return realAltitude;
    }

    public Color getColorAtAltitude(float altitude) {
        float z = altitude / this.maxHeight;
        return planet.terrainGradient.Evaluate(z);
    }
}