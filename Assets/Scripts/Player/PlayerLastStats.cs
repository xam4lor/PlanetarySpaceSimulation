using Unity;
using UnityEngine;
using System.Collections;

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