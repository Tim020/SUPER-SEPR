using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	/// <summary>
	/// The speed the camera gets moved at when dragging
	/// </summary>
	public float dragSpeed = 1;

	/// <summary>
	/// The position that that drag was started at
	/// </summary>
	private Vector3 dragOrigin;

	/// <summary>
	/// Called every update cycle, checks whether the LMB is pressed and transforms the camera position accordingly
	/// </summary>
	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			dragOrigin = Input.mousePosition;
			return;
		}

		if (!Input.GetMouseButton (0)) {
			return;
		}

		Vector3 pos = Camera.main.ScreenToViewportPoint (dragOrigin - Input.mousePosition);
		Vector3 move = new Vector3 (pos.x * dragSpeed, pos.y * dragSpeed, 0);

		transform.Translate (move, Space.World);  
	}
}
