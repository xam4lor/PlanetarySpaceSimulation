using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour {
	public Transform targetTransform;
    public float speed = 5;

	void Update() {
		Vector3 displacementFromTarget = targetTransform.position - transform.position;
		Vector3 directionToTarget = displacementFromTarget.normalized;
		Vector3 velocity = directionToTarget * speed;

		float r = displacementFromTarget.magnitude;
		
		if (r > 1.5f)
			transform.Translate(velocity * Time.deltaTime);
	}
}
