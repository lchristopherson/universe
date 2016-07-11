using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T)) {
			GL.wireframe = true;
			Debug.Log("Switched to wireframe rendering");
		} else if (Input.GetKeyDown(KeyCode.R)) {
			GL.wireframe = false;
			Debug.Log("Switched to default rendering");
		}
	}
}
