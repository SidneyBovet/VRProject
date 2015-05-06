using UnityEngine;
using System.Collections;

public class UserController : MonoBehaviour {

	public float stopThreshold = 0.00001f;

	bool m_canMove = true;
	float m_moveSensitivity = 100000.0f;

	public void Move (float distance) {
		if (m_canMove) {
			Debug.Log("Distance "+distance);
			if (Mathf.Abs(distance) > stopThreshold) {
				GetComponent<Rigidbody>().AddForce(new Vector3(0f,0f,distance*m_moveSensitivity));
			} else {
				GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}
}
