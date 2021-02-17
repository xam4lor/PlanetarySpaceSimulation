using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour {
	public LayerMask mask;

	void Update() {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo, 100, mask, QueryTriggerInteraction.Ignore)) {
			Destroy(hitInfo.collider.gameObject);
			Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
		}
		else {
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.green);
		}
	}
}
