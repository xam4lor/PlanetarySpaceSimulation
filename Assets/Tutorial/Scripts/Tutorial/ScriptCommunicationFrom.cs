using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScriptCommunicationFrom : MonoBehaviour {
	// Start is called before the first frame update
	public event Action testAction;

	void Start() {
		
	}

	// Update is called once per frame
	void Update() {
		if (true) {
			if (testAction != null) {
				testAction();
			}
		}
	}
}
