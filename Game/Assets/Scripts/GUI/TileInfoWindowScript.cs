// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.UI;

public class TileInfoWindowScript : MonoBehaviour {

	/// <summary>
	/// The user interface canvas.
	/// </summary>
	public CanvasScript uiCanvas;

	/// <summary>
	/// The acquire tile button.
	/// </summary>
	public GameObject acquireTileButton;

	/// <summary>
	/// The install roboticon button.
	/// </summary>
	public GameObject installRoboticonButton;

	/// <summary>
	/// The price text.
	/// </summary>
	public Text priceText;

	/// <summary>
	/// The owner text.
	/// </summary>
	public Text ownerText;

	/// <summary>
	/// The food base amount text.
	/// </summary>
	public Text foodBase;

	/// <summary>
	/// The energy base amount text.
	/// </summary>
	public Text energyBase;

	/// <summary>
	/// The ore base amount text.
	/// </summary>
	public Text oreBase;

	/// <summary>
	/// The food total text.
	/// </summary>
	public Text foodTotal;

	/// <summary>
	/// The energy total text.
	/// </summary>
	public Text energyTotal;

	/// <summary>
	/// The ore total text.
	/// </summary>
	public Text oreTotal;

	/// <summary>
	/// The current tile.
	/// </summary>
	private Tile currentTile;

	/// <summary>
	/// Show the info window for the specified tile.
	/// </summary>
	/// <param name="tile">The tile to show info for.</param>
	public void Show(Tile tile) {
		currentTile = tile;
		UpdateResourceTexts();
		UpdateOwnerText(tile.GetOwner());
		UpdatePriceText(tile.GetPrice());
		uiCanvas.RefreshRoboticonList();

		Data.GameState gamePhase = GameHandler.GetGameManager().GetCurrentState();

		switch (gamePhase) {
			case Data.GameState.TILE_PURCHASE:
				installRoboticonButton.SetActive(false);
				if (tile.GetOwner() == null) {
					acquireTileButton.SetActive(true);
				} else {
					acquireTileButton.SetActive(false);
				}
				break;

			case Data.GameState.ROBOTICON_PLACEMENT:
				acquireTileButton.SetActive(false);
				if (tile.GetOwner() == GameHandler.GetGameManager().GetHumanPlayer()) {
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

	/// <summary>
	/// Refresh this window.
	/// </summary>
	public void Refresh() {
		if (currentTile != null) {
			uiCanvas.RefreshRoboticonList();
			Show(currentTile);
		}
	}

	/// <summary>
	/// Hide this window.
	/// </summary>
	public void Hide() {
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Called when the player presses the buy button.
	/// </summary>
	public void AcquireTile() {
		if (currentTile != null) {
			uiCanvas.PurchaseTile(currentTile);
			UpdateOwnerText(currentTile.GetOwner());
		}
	}

	/// <summary>
	/// Opens the UI element that allows roboticons to be selected for installation.
	/// </summary>
	public void InstallRoboticon() {
		uiCanvas.ShowRoboticonList();
	}

	/// <summary>
	/// Plaies the purchase declined animation.
	/// </summary>
	public void PlayPurchaseDeclinedAnimation() {
		priceText.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
	}

	/// <summary>
	/// Updates the resource amount text items.
	/// </summary>
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

	/// <summary>
	/// Updates the price text.
	/// </summary>
	/// <param name="price">The tile price.</param>
	private void UpdatePriceText(int price) {
		priceText.text = "£" + price.ToString();
	}

	/// <summary>
	/// Updates the owner text.
	/// </summary>
	/// <param name="owner">The tile owner.</param>
	private void UpdateOwnerText(AbstractPlayer owner) {
		if (owner == null) {
			ownerText.text = "Unowned";
		} else if (owner == GameHandler.GetGameManager().GetHumanPlayer()) {
			ownerText.text = "You";
		} else {
			ownerText.text = owner.GetName();
		}
	}

}