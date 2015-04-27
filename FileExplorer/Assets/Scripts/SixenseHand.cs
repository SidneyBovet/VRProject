using UnityEngine;
using System.Collections;

public class SixenseHand : MonoBehaviour
{
	public SixenseHands	m_hand;
	public SixenseInput.Controller m_controller = null;
	public float selectThreshold = 1;
	public int selectMinFrams;
	public bool	grab = false;

	public delegate void SelectionCallBack();
	public SelectionCallBack selectionCallBack;

	Animator 	m_animator;
	float 		m_fLastTriggerVal;
	Vector3		m_initialPosition;
	Quaternion 	m_initialRotation;
	private Vector3? m_pointPos = null;
	private int m_numPointingFrame = 0;

	protected void Start() {
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
		fTriggerVal = Mathf.Lerp( m_fLastTriggerVal, fTriggerVal, 0.5f );
		m_fLastTriggerVal = fTriggerVal;
		grab = fTriggerVal > 0.005f;
		m_animator.SetBool ("Fist", grab);
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
			if(!m_pointPos.HasValue && (transform.localPosition - m_pointPos.Value).magnitude > selectThreshold * Time.deltaTime) {
				m_numPointingFrame += 1;
				if(m_numPointingFrame > selectMinFrams) {
					selectionCallBack();
					m_numPointingFrame = 0;
				}
			} else {
				m_numPointingFrame = 0;
			}
			m_pointPos = transform.localPosition;
		} else {
			m_pointPos = null;
			m_numPointingFrame = 0;
		}
	}
}