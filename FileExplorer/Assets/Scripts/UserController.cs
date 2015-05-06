using UnityEngine;
using System.Collections;

public class UserController : MonoBehaviour {

	public float stopThreshold = 0.00001f;

	bool m_canMove = true;
	float m_moveSensitivity = 150000.0f;

	void Update () {
		Transform forward = transform.FindChild ("ForwardDirection");
		RaycastHit hit;
		Ray lookRay = new Ray (transform.position,forward.forward);

		if (Physics.Raycast(lookRay, out hit, 1000.0f)) {
			if (hit.collider.CompareTag("File")) {
				((Behaviour)hit.collider.GetComponent("Halo")).enabled = false;
			}
		}
	}

	public void Move (float distance) {
		if (m_canMove) {
			if (Mathf.Abs(distance) > stopThreshold) {
				GetComponent<Rigidbody>().AddForce(new Vector3(0f,0f,distance*m_moveSensitivity));
			} else {
				GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}
}
