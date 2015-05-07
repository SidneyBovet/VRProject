using UnityEngine;
using System.Collections;

public class SixenseHand : MonoBehaviour
{
	public SixenseHands	m_hand;
	public SixenseInput.Controller m_controller = null;
	public float minSelectionSpeed = 0.01f;
	public float selectionDistance = 0.01f;
	public bool	grab = false;

	Animator 	m_animator;
	float 		m_fLastTriggerVal;
	Vector3		m_initialPosition;
	Quaternion 	m_initialRotation;
	private Vector3? m_pointPos = null;
	private float m_selectionDistance = 0f;
	Transform m_sceneController;

	protected void Start() {
		m_sceneController = transform.Find("controller");
		m_animator = gameObject.GetComponent<Animator>();
		m_initialRotation = transform.localRotation;
		m_initialPosition = transform.localPosition;
	}

	protected void Update() {
		if ( m_controller == null ) {
			m_controller = SixenseInput.GetController( m_hand );
		} else if ( m_animator != null ) {
			UpdateHandAnimation();
			Select();
		}
	}

	protected void UpdateHandAnimation() {
		// Point
		m_animator.SetBool( "Point", m_controller.GetButton(SixenseButtons.BUMPER));
				
		// Fist
		float fTriggerVal = m_controller.Trigger;
		float lerpTriggerVal = Mathf.Lerp( m_fLastTriggerVal, fTriggerVal, 0.5f );
		m_fLastTriggerVal = lerpTriggerVal;
		grab = fTriggerVal > 0.005f;
		m_animator.SetBool ("Fist", lerpTriggerVal > 0.005f);
		m_animator.SetFloat("FistAmount", fTriggerVal);
	}

	public Quaternion InitialRotation {
		get { return m_initialRotation; }
	}
	
	public Vector3 InitialPosition {
		get { return m_initialPosition; }
	}

	private void Select() {
		if (m_controller.GetButton (SixenseButtons.BUMPER)) {
			if(m_pointPos.HasValue && (transform.localPosition - m_pointPos.Value).magnitude > minSelectionSpeed * Time.deltaTime) {
				m_selectionDistance += (transform.localPosition - m_pointPos.Value).magnitude;
				if(m_selectionDistance > selectionDistance) {

					m_sceneController.GetComponent<PlanetController>().Selection();

					m_selectionDistance = 0f;
				}
			} else {
				m_selectionDistance = 0f;
			}
			m_pointPos = transform.localPosition;
		} else {
			m_pointPos = null;
			m_selectionDistance = 0f;
		}
	}
}