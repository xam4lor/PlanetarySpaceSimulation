using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator {
    private NoiseGenerator noiseGenerator;
    private TerrainColorsSettings[] terrainColorsSettings;
    private Planet planet;

    public TerrainGenerator(NoiseSettings[] settings, TerrainColorsSettings[] terrainColorsSettings, Planet planet) {
        this.noiseGenerator = new NoiseGenerator(settings);
        this.planet = planet;
        this.terrainColorsSettings = terrainColorsSettings;
    }

    public void initialize() {
        this.noiseGenerator.initialize();
    }

    public float getAltitudeAt(Vector3 unitarySpherePos) {
        float altitudePercent = 0;

        // Computes altitude
        altitudePercent = noiseGenerator.evaluate(unitarySpherePos);

        // Computes real altitude from percentage
        return altitudePercent * this.planet.terrainHeight * this.planet.planetScale / 100;
    }

    public Color getColorAtAltitude(float altitude) {
        float z = altitude / planet.terrainHeight;

        for (int i = 0; i < terrainColorsSettings.Length - 1; i++) {
            if (z < terrainColorsSettings[i].limit) {
                if (z > terrainColorsSettings[i].limit - terrainColorsSettings[i].tolerance * terrainColorsSettings[i].limit)
                    return Color.Lerp(
                        terrainColorsSettings[i].color,
                        terrainColorsSettings[i + 1].color,
                        (z - terrainColorsSettings[i].limit) / (terrainColorsSettings[i + 1].limit - terrainColorsSettings[i].limit));
                return terrainColorsSettings[i].color;
            }
        }

        return terrainColorsSettings[terrainColorsSettings.Length - 1].color;
    }
}