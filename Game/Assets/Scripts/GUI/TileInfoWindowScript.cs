// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.UI;

public class TileInfoWindowScript : MonoBehaviour {

	public CanvasScript uiCanvas;
	public GameObject acquireTileButton;
	public GameObject installRoboticonButton;
	public Text priceText;
	public Text ownerText;

	#region Resource Labels

	public Text foodBase;
	public Text energyBase;
	public Text oreBase;
	public Text foodTotal;
	public Text energyTotal;
	public Text oreTotal;

	#endregion

	private Tile currentTile;

	public void Show(Tile tile) {
		currentTile = tile;
		UpdateResourceTexts();
		UpdateOwnerText(tile.GetOwner());
		UpdatePriceText(tile.GetPrice());
		uiCanvas.RefreshRoboticonList();

		Data.GameState gamePhase = GameHandler.GetGameManager().GetCurrentState();

		switch (gamePhase) {
			case Data.GameState.ACQUISITION:
				installRoboticonButton.SetActive(false);
				if (tile.GetOwner() == null) {
					acquireTileButton.SetActive(true);
				} else {
					acquireTileButton.SetActive(false);
				}
				break;

			case Data.GameState.INSTALLATION:
				acquireTileButton.SetActive(false);
				if (tile.GetOwner() == uiCanvas.GetHumanGui().GetCurrentHuman()) {
					installRoboticonButton.SetActive(true);
				} else {
					installRoboticonButton.SetActive(false);
				}
				break;

			default:
				installRoboticonButton.SetActive(false);
				acquireTileButton.SetActive(false);
				break;
		}

		gameObject.SetActive(true);
	}

	public void Refresh() {
		if (currentTile != null) {
			uiCanvas.RefreshRoboticonList();
			Show(currentTile);
		}
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void AcquireTile() {
		if (currentTile != null) {
			uiCanvas.PurchaseTile(currentTile);
			UpdateOwnerText(currentTile.GetOwner());
		}
	}

	public void InstallRoboticon() {
		uiCanvas.ShowRoboticonList();
	}

	public void PlayPurchaseDeclinedAnimation() {
		priceText.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
	}

	private void UpdateResourceTexts() {
		ResourceGroup tileBaseResources = currentTile.GetBaseResourcesGenerated();
		ResourceGroup tileTotalResources = currentTile.GetTotalResourcesGenerated();

		foodBase.text = tileBaseResources.GetFood().ToString();
		energyBase.text = tileBaseResources.GetEnergy().ToString();
		oreBase.text = tileBaseResources.GetOre().ToString();

		foodTotal.text = tileTotalResources.GetFood().ToString();
		energyTotal.text = tileTotalResources.GetEnergy().ToString();
		oreTotal.text = tileTotalResources.GetOre().ToString();
	}

	private void UpdatePriceText(int price) {
		priceText.text = "£" + price.ToString();
	}

	private void UpdateOwnerText(AbstractPlayer owner) {
		if (owner == null) {
			ownerText.text = "Unowned";
		} else if (owner == GameHandler.GetGameManager().GetCurrentPlayer()) {
			ownerText.text = "You";
		} else {
			ownerText.text = owner.GetName();
		}
	}

}