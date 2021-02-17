using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float speed = 0;
	private Vector3 velocity;
	private int coinCount;

	Rigidbody cRigidbody;

	// Start is called before the first frame update
	void Start() {
		cRigidbody = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update() {
		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector3 direction = input.normalized;
		velocity = speed * direction;
	}

	void FixedUpdate() {
		cRigidbody.position += velocity * Time.deltaTime;
	}


	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Coin") {
			Destroy(other.gameObject);
            coinCount++;
		}
	}
}
