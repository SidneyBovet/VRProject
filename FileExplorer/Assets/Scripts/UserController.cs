using UnityEngine;
using System.Collections;

public class UserController : MonoBehaviour {

	bool m_canMove = true;
	float m_moveSensitivity = 0.1f;

	public void Move (float distance) {
		if (m_canMove) {
			transform.position += new Vector3(0f,0f,distance*m_moveSensitivity);
		}
	}
}
