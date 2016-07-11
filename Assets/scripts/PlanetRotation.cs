using UnityEngine;
using System.Collections;

public class PlanetRotation : MonoBehaviour {

	public float rotationDegreesPerSecond = 45f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float currentAngle = transform.rotation.eulerAngles.y;
		transform.rotation = Quaternion.AngleAxis(currentAngle + (Time.deltaTime * rotationDegreesPerSecond), Vector3.up);
	}
}
