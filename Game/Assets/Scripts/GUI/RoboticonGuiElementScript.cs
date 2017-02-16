// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.UI;

public class RoboticonGuiElementScript : MonoBehaviour {

	public Text roboticonNameObject;
	public GameObject installButton;
	public GameObject upgradeButton;
	public Roboticon roboticon;
	private RoboticonWindowScript roboticonWindow;

	public void SetRoboticon(Roboticon roboticon) {
		this.roboticon = roboticon;
		roboticonNameObject.text = roboticon.GetName();
	}

	public void SetButtonEventListeners(RoboticonWindowScript roboticonWindow) {
		this.roboticonWindow = roboticonWindow;
		installButton.GetComponent<Button>().onClick.AddListener(OnInstallClick);
		upgradeButton.GetComponent<Button>().onClick.AddListener(OnUpgradeClick);
	}

	public void ShowInstallButton() {
		if (!roboticon.IsInstalledToTile()) {
			installButton.SetActive(true);
		}
		upgradeButton.SetActive(false);
	}

	public void ShowUpgradeButton() {
		installButton.SetActive(false);
		upgradeButton.SetActive(true);
	}

	public void HideButtons() {
		installButton.SetActive(false);
		upgradeButton.SetActive(false);
	}

	public void OnInstallClick() {
		if (roboticonWindow.InstallRoboticon(roboticon)) {
			upgradeButton.SetActive(false);
		}
	}

	public void OnUpgradeClick() {
		roboticonWindow.UpgradeRoboticon(roboticon);
	}

}