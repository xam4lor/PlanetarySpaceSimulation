using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainColorsSettings {
    public Color color = new Color(0, 0, 0);
    public float limit = 0.2f; // between 0 and 1
    public float tolerance = 0.2f; // how much smoothed it is (percent)
}