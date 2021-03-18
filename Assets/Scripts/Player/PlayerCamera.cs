using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
	public Universe universe;

    private void Update() {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo)) {
			if (hitInfo.collider.tag == "Chunk") {
                Vector3 collisionPoint = hitInfo.point;
				string chunkName = hitInfo.collider.name;
                int chunkID = int.Parse(chunkName.Substring(0, 1));
				
                universe
                    .getNearestBody(transform.position)
                    .SendMessage("onPlayerSeeChunk", new object[]{ transform.position, collisionPoint, hitInfo.distance, chunkID, chunkName })
                ;
                Debug.DrawLine(ray.origin, hitInfo.point, Color.green);
			}
        }
	}
}
