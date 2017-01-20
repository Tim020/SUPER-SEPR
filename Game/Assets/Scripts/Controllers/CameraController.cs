using UnityEngine;
using System.Collections;

/// <summary>
/// Class to handle the movement of the camera through various control methods
/// </summary>
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

	/// <summary>
	/// The furthest in you are allowed to zoom
	/// </summary>
	public static float maxZoom { get ; private set; }

	/// <summary>
	/// The furthest out you are allowed to zoom
	/// </summary>
	public static float minZoom { get; private set; }

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		startPosition = Camera.main.transform.position;
		minZoom = Camera.main.orthographicSize;
		maxZoom = 4;
	}

	/// <summary>
	/// Called every update cycle, checks whether the RMB is pressed and transforms the camera position accordingly.
	/// Also checks if the MMB is clicked to return to the camera to the centre of the game world
	/// Also checks for scroll wheel and will zoom in and out, this also scales the drag speed so it's the same rate at each level of zoom
	/// </summary>
	void Update() {
		if (Input.GetMouseButtonDown(2)) {
			Camera.main.transform.position = startPosition;
			return;
		}

		if (Input.GetMouseButtonDown(1)) {
			dragOrigin = Input.mousePosition;
			return;
		}

		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			float newZoom = Camera.main.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * -2f;
			if (newZoom > minZoom) {
				newZoom = minZoom;
			}
			if (newZoom < maxZoom) {
				newZoom = maxZoom;
			}
			Camera.main.orthographicSize = newZoom;
		}

		if (!Input.GetMouseButton(1)) {
			return;
		}

		//TODO: Clamp these so you can't go past the map bounds
		Vector3 pos = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
		Vector3 move = new Vector3(pos.x * dragSpeed * Camera.main.orthographicSize / minZoom, pos.y * dragSpeed * Camera.main.orthographicSize / minZoom, 0);

		transform.Translate(move, Space.World);
	}
}
