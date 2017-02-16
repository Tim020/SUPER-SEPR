// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using UnityEngine;

public class RoboticonWindowScript : MonoBehaviour {

	public CanvasScript canvas;
	public GameObject roboticonIconsList;
	//Roboticon gui elements are added to this GUI content

	private GameObject roboticonTemplate;
	private List<GameObject> currentlyDisplayedRoboticons = new List<GameObject>();
	private const string ROBOTICON_TEMPLATE_PATH = "Prefabs/GUI/TemplateRoboticon";

	/// <summary>
	/// Display a new set of roboticons to the GUI. Overwrite any previously displayed
	/// roboticons.
	/// </summary>
	/// <param name="roboticonsToDisplay"></param>
	public void DisplayRoboticonList(List<Roboticon> roboticonsToDisplay) {
		ClearRoboticonList();

		gameObject.SetActive(true);

		foreach (Roboticon roboticon in roboticonsToDisplay) {
			AddRoboticon(roboticon);
		}

		Data.GameState currentState = GameHandler.GetGameManager().GetCurrentState();
		if (currentState == Data.GameState.AUCTION) {
			ShowRoboticonUpgradeButtons();
		} else if (currentState == Data.GameState.ROBOTICON_PLACEMENT) {
			HumanGui humanGui = canvas.GetHumanGui();
			if (humanGui.GetCurrentSelectedTile().GetOwner() == GameHandler.GetGameManager().GetHumanPlayer()) {
				ShowRoboticonInstallButtons();
			}
		} else {
			HideInstallAndUpgradeButtons();
		}
	}

	public void HideRoboticonList() {
		ClearRoboticonList();
		currentlyDisplayedRoboticons = new List<GameObject>();
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Add a roboticon to the displayed list of roboticons in the UI.
	/// Returns the gameobject representing the roboticon in the scene.
	/// </summary>
	/// <param name="roboticon"></param>
	/// <returns></returns>
	public void AddRoboticon(Roboticon roboticon) {
		LoadRoboticonTemplate();

		GameObject roboticonGuiObject = (GameObject)Instantiate(roboticonTemplate);
		roboticonGuiObject.transform.SetParent(roboticonIconsList.transform, true);
		RectTransform guiObjectTransform = roboticonGuiObject.GetComponent<RectTransform>();

		guiObjectTransform.localScale = new Vector3(1, 1, 1); //Undo Unity's instantiation meddling

		RoboticonGuiElementScript roboticonElementScript = guiObjectTransform.GetComponent<RoboticonGuiElementScript>();
		roboticonElementScript.SetRoboticon(roboticon);
		roboticonElementScript.SetButtonEventListeners(this);

		currentlyDisplayedRoboticons.Add(roboticonGuiObject);
	}

	/// <summary>
	/// Show the Upgrade button for each roboticon in the window.
	/// </summary>
	public void ShowRoboticonUpgradeButtons() {
		foreach (GameObject roboticonElement in currentlyDisplayedRoboticons) {
			roboticonElement.GetComponent<RoboticonGuiElementScript>().ShowUpgradeButton();
		}
	}

	/// <summary>
	/// Show the install button for each roboticon in the window.
	/// </summary>
	public void ShowRoboticonInstallButtons() {
		foreach (GameObject roboticonElement in currentlyDisplayedRoboticons) {
			roboticonElement.GetComponent<RoboticonGuiElementScript>().ShowInstallButton();
		}
	}

	public void HideInstallAndUpgradeButtons() {
		foreach (GameObject roboticonElement in currentlyDisplayedRoboticons) {
			roboticonElement.GetComponent<RoboticonGuiElementScript>().HideButtons();
		}
	}

	public void UpgradeRoboticon(Roboticon roboticon) {
		canvas.ShowRoboticonUpgradesWindow(roboticon);
	}

	/// <summary>
	/// Install the given roboticon to the current selected tile.
	/// </summary>
	/// <param name="roboticon"></param>
	public bool InstallRoboticon(Roboticon roboticon) {
		return canvas.InstallRoboticon(roboticon);
	}

	private void ClearRoboticonList() {
		if (currentlyDisplayedRoboticons.Count > 0) {
			for (int i = currentlyDisplayedRoboticons.Count - 1; i >= 0; i--) {
				Destroy(currentlyDisplayedRoboticons[i]);
			}

			currentlyDisplayedRoboticons = new List<GameObject>();
		}
	}

	/// <summary>
	/// Loads the roboticon template if not already loaded.
	/// </summary>
	private void LoadRoboticonTemplate() {
		if (roboticonTemplate == null) {
			roboticonTemplate = (GameObject)Resources.Load(ROBOTICON_TEMPLATE_PATH);

			if (roboticonTemplate == null) {
				throw new ArgumentException("Cannot find roboticon template at the specified path.");
			}
		}
	}

}