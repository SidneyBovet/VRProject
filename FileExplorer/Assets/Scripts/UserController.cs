using UnityEngine;
using System.Collections;

public class UserController : MonoBehaviour {

	public float stopThreshold = 0.00001f;

	private bool m_canMove = true;
	private float m_moveSensitivity = 150000.0f;
	private Collider m_selection = null;
	private Vector3 defaultPos;

	void Start() {
		defaultPos = transform.position;
	}

	void Update () {
		Transform forward = transform.FindChild ("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
		RaycastHit hit;
		Ray lookRay = new Ray (transform.position,forward.forward);

		// Select the planet that the plazer is looking at
		if (Physics.Raycast(lookRay, out hit, 1000.0f) && m_selection != hit.collider) {
			if (hit.collider.CompareTag("File") || hit.collider.CompareTag("Folder")) {
				((Behaviour)(hit.collider.GetComponent("Halo"))).enabled = true;
				if(m_selection != null)
					((Behaviour)(m_selection.GetComponent("Halo"))).enabled = false;
				m_selection = hit.collider;
			}
		}
		
		if (Input.GetKeyDown (KeyCode.R)) {
			transform.position = defaultPos;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	// move the player of distance. This is typically user to scroll along the list of files inside a folder
	public void Move (float distance) {
		if (m_canMove) {
			if (Mathf.Abs(distance) > stopThreshold) {
				GetComponent<Rigidbody>().AddForce(new Vector3(0f,0f,distance*m_moveSensitivity));
			} else {
				GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}

	public Collider GetSelection () {
		return m_selection;
	}
}
