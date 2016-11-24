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
	/// The position that the camera is set to at the start of the game
	/// </summary>
    private Vector3 startPosition;

    void Start() {
        startPosition = Camera.main.transform.position;
    }

	/// <summary>
	/// Called every update cycle, checks whether the LMB is pressed and transforms the camera position accordingly.
    /// Also checks if the MMB is clicked to return to the camera to the centre of the game world
	/// </summary>
	void Update() {
        if (Input.GetMouseButtonDown(2)) {
            Camera.main.transform.position = startPosition;
            return;
        }

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
