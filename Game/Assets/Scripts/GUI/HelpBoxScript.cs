// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.UI;

public class HelpBoxScript : MonoBehaviour {

	public Text helpBoxText;
	private bool move;
	private bool hidden;
	private Vector3 startPos;

	public void ShowHelpBox(string text = "") {
		helpBoxText.text = text;
		move = true;
		hidden = false;
	}

	public void HideHelpBox() {
		move = false;
		hidden = true;
	}

	void Start() {
		startPos = gameObject.transform.position;
	}

	void Update() {
		if (move) {
			if (hidden) {
				Vector3 pos = gameObject.transform.position;
				pos.y--;
				gameObject.transform.position = pos;
			} else {

			}
		}
	}

}