using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCasterTerrain : MonoBehaviour {
	public Transform objectToPlace;
	public Camera gameCamera;


	void Update() {
		Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo)) {
            objectToPlace.position = hitInfo.point;
            /* Debug.DrawLine(ray.origin, hitInfo.point, Color.red); */
            objectToPlace.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
	}
}
