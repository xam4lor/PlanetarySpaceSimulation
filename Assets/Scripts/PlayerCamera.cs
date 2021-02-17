using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
	public GameObject planetChunks;

    private void Update() {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo)) {
			if (hitInfo.collider.tag == "Chunk") {
                Vector3 collisionPoint = hitInfo.point;
				string chunkName = hitInfo.collider.name;
                int chunkID = int.Parse(chunkName.Substring(0, 1));
				
                planetChunks.GetComponent<Planet>().SendMessage("onPlayerSeeChunk", new object[4]{ collisionPoint, hitInfo.distance, chunkID, chunkName });
                Debug.DrawLine(ray.origin, hitInfo.point, Color.green);
			}
        }
	}
}
