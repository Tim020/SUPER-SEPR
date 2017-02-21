// Game Executable hosted at: https://seprated.github.io/Assessment3/Executables.zip

using UnityEngine;
using UnityEngine.UI;

public class HelpBoxScript : MonoBehaviour {

	/// <summary>
	/// The help box animator.
	/// </summary>
    public Animator helpBoxAnimator;

	/// <summary>
	/// The help box text.
	/// </summary>
    public Text helpBoxText;

	/// <summary>
	/// Shows the help box.
	/// </summary>
	/// <param name="text">Text to display.</param>
    public void ShowHelpBox(string text = "") {
        helpBoxText.text = text;
        helpBoxAnimator.SetBool("helpBoxVisible", true);
    }

	/// <summary>
	/// Hides the help box.
	/// </summary>
    public void HideHelpBox() {
        helpBoxAnimator.SetBool("helpBoxVisible", false);
    }

}