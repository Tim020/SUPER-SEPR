// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.UI;

public class RoboticonGuiElementScript : MonoBehaviour {

	/// <summary>
	/// The roboticon name text.
	/// </summary>
	public Text roboticonName;

	/// <summary>
	/// The install button.
	/// </summary>
	public GameObject installButton;

	/// <summary>
	/// The upgrade button.
	/// </summary>
	public GameObject upgradeButton;

	/// <summary>
	/// The roboticon this element represents.
	/// </summary>
	public Roboticon roboticon;

	/// <summary>
	/// The main roboticon window.
	/// </summary>
	private RoboticonWindowScript roboticonWindow;

	/// <summary>
	/// Sets the roboticon.
	/// </summary>
	/// <param name="roboticon">The roboticon this element represents.</param>
	public void SetRoboticon(Roboticon roboticon) {
		this.roboticon = roboticon;
		roboticonName.text = roboticon.GetName();
	}

	/// <summary>
	/// Sets the button event listeners.
	/// </summary>
	/// <param name="roboticonWindow">The main roboticon window.</param>
	public void SetButtonEventListeners(RoboticonWindowScript roboticonWindow) {
		this.roboticonWindow = roboticonWindow;
		installButton.GetComponent<Button>().onClick.AddListener(OnInstallClick);
		upgradeButton.GetComponent<Button>().onClick.AddListener(OnUpgradeClick);
	}

	/// <summary>
	/// Shows the install button.
	/// </summary>
	public void ShowInstallButton() {
		if (!roboticon.IsInstalledToTile()) {
			installButton.SetActive(true);
		}
		upgradeButton.SetActive(false);
	}

	/// <summary>
	/// Shows the upgrade button.
	/// </summary>
	public void ShowUpgradeButton() {
		installButton.SetActive(false);
		upgradeButton.SetActive(true);
	}

	/// <summary>
	/// Hides all buttons.
	/// </summary>
	public void HideButtons() {
		installButton.SetActive(false);
		upgradeButton.SetActive(false);
	}

	/// <summary>
	/// Called when the install button is clicked.
	/// </summary>
	public void OnInstallClick() {
		if (roboticonWindow.InstallRoboticon(roboticon)) {
			upgradeButton.SetActive(false);
		}
	}

	/// <summary>
	/// Called when the upgrade button is clicked.
	/// </summary>
	public void OnUpgradeClick() {
		roboticonWindow.UpgradeRoboticon(roboticon);
	}

}