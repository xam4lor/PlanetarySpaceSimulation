using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCommunicationTo : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {
        // Find game object
		/* GameObject obj = GameObject.Find("Name_Of_The_Object"); */
		/* GameObject obj = GameObject.FindWithTag("Tag_of_the_object"); */
		ScriptCommunicationFrom player = FindObjectOfType<ScriptCommunicationFrom>();
        player.testAction += SayEventTarget;
	}

	// Update is called once per frame
	void Update() {

	}

	void SayEventTarget() {

	}
}
