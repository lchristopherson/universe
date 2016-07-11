using UnityEngine;
using System.Collections;

public class ShipMove : MonoBehaviour {

	public float movespeed;
	private Rigidbody m_Rigidbody;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.W)) {
			m_Rigidbody.AddForce (transform.forward * movespeed, ForceMode.Impulse);
		} else if (Input.GetKey (KeyCode.S)) {
			m_Rigidbody.AddForce (-transform.forward * movespeed, ForceMode.Impulse);
		}

		if (Input.GetKey (KeyCode.A)) {
			m_Rigidbody.AddForce (-transform.right * movespeed, ForceMode.Impulse);
		} else if (Input.GetKey (KeyCode.D)) {
			m_Rigidbody.AddForce (transform.right * movespeed, ForceMode.Impulse);
		}

	}


}
