using UnityEngine;
using System.Collections;

public class SixenseHandsController : MonoBehaviour {
	SixenseHand[] 	m_hands;

	Vector3	m_baseOffset;
	float 	m_sensitivity = 0.001f; // Sixense units are in mm
	bool 	m_bInitialized;
	private double? m_scrollAverage = null;
	
	void Start () {
		m_hands = GetComponentsInChildren<SixenseHand>();
	}

	void Update () {
		bool bResetHandPosition = false;

		foreach ( SixenseHand hand in m_hands ) {
			if ( IsControllerActive( hand.m_controller ) && hand.m_controller.GetButtonDown( SixenseButtons.START ) ) {
				bResetHandPosition = true;
			}

			if ( m_bInitialized ) {
				UpdateHand( hand );
			}
		}
				
		if ( bResetHandPosition ) {
			m_bInitialized = true;

			m_baseOffset = Vector3.zero;

			// Get the base offset assuming forward facing down the z axis of the base
			foreach ( SixenseHand hand in m_hands ) {
				m_baseOffset += hand.m_controller.Position;
			}
			Grab();

			m_baseOffset /= 2;
		}
	}

	void UpdateHand( SixenseHand hand ) {
		bool bControllerActive = IsControllerActive( hand.m_controller );

		if ( bControllerActive ) {
			hand.transform.localPosition = ( hand.m_controller.Position - m_baseOffset ) * m_sensitivity;
			hand.transform.localRotation = hand.m_controller.Rotation * hand.InitialRotation;
		} else {
			// use the inital position and orientation because the controller is not active
			hand.transform.localPosition = hand.InitialPosition;
			hand.transform.localRotation  = hand.InitialRotation;
		}
	}
	
	private void Grab() {
		bool scrolling = true;
		double average = 0f;
		foreach ( SixenseHand hand in m_hands ) {
			if(!hand.grab) {
				scrolling = false;
				break;
			} else {
				average += hand.transform.localPosition.z;
			}
		}
		
		if (scrolling) {
			average /= m_hands.Length;
			if (m_scrollAverage.HasValue) {
				//TODO call camera with m_scrollAverage - average
				print ("Grab: "+ (m_scrollAverage.Value - average));
			}
			m_scrollAverage = average;
		} else {
			m_scrollAverage = null;
		}
	}
	
	void OnGUI() {
		if ( !m_bInitialized ) {
			GUI.Box( new Rect( Screen.width / 2 - 50, Screen.height - 40, 100, 30 ),  "Press Start" );
		}
	}

	bool IsControllerActive( SixenseInput.Controller controller ) {
		return ( controller != null && controller.Enabled && !controller.Docked );
	}
}