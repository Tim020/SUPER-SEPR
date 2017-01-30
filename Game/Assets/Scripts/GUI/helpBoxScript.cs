// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class helpBoxScript : MonoBehaviour
{

    public Animator helpBoxAnimator;
    public Text helpBoxText;

    public void ShowHelpBox(string text = "")
    {
        helpBoxText.text = text;
        helpBoxAnimator.SetBool("helpBoxVisible", true);
    }

    public void HideHelpBox()
    {
        helpBoxAnimator.SetBool("helpBoxVisible", false);
    }
}
