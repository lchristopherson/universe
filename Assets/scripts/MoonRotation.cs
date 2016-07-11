using UnityEngine;
using System.Collections;

public class MoonRotation : MonoBehaviour {

	public Transform center;
	public float rotationDegreesPerSecond = 45f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(center.position, Vector3.up, rotationDegreesPerSecond * Time.deltaTime);
	}
}
