using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour {
	public GameObject coinPrefab;

	// Start is called before the first frame update
	void Start() {
		
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Vector3 rPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
            Vector3 rRot = Vector3.up * Random.Range(0, 360);

			GameObject newCoin = (GameObject) Instantiate(coinPrefab, rPos, Quaternion.Euler(rRot));
			newCoin.transform.parent = transform;
		}
	}
}
