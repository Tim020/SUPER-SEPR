using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AuctionTradesWindow : MonoBehaviour {

	/// <summary>
	/// The canvas.
	/// </summary>
	public CanvasScript canvas;

	/// <summary>
	/// The trade items list.
	/// </summary>
	public GameObject tradeItemsList;

	/// <summary>
	/// The trade item prefab.
	/// </summary>
	private GameObject tradePrefab;

	/// <summary>
	/// The current displayed trades.
	/// </summary>
	private List<GameObject> currentDisplayedTrades = new List<GameObject>();

	/// <summary>
	/// The location of the roboticon prefab template.
	/// </summary>
	private const string ROBOTICON_TEMPLATE_PATH = "Prefabs/GUI/TemplateAuctionTrade";

	/// <summary>
	/// The selected resource for player trades.
	/// </summary>
	private Data.ResourceType selectedResource;

	/// <summary>
	/// The maximum amount of the resource the player can trade.
	/// </summary>
	private int resourceMax;

	/// <summary>
	/// The trade quantity text.
	/// </summary>
	public Text tradeQuantityText;

	/// <summary>
	/// The trade unit price text.
	/// </summary>
	public Text tradeUnitPriceText;

	/// <summary>
	/// The trade total price.
	/// </summary>
	public Text tradeTotalPrice;

	/// <summary>
	/// The trade quantity.
	/// </summary>
	private int tradeQuantity;

	/// <summary>
	/// The unit price for the trade.
	/// </summary>
	private int unitPrice;

	/// <summary>
	/// The trade confirmation popup.
	/// </summary>
	public GameObject tradeConfirmation;

	/// <summary>
	/// The resource type in the confirmation popup.
	/// </summary>
	public Text resourceTypeConfirmation;

	/// <summary>
	/// The resource amount in the confirmation popup.
	/// </summary>
	public Text resourceAmountConfirmation;

	/// <summary>
	/// The unit price in the confirmation popup.
	/// </summary>
	public Text unitPriceConfirmation;

	/// <summary>
	/// The total price in the confirmation popup.
	/// </summary>
	public Text totalPriceConfirmation;

	/// <summary>
	/// The popup used when a trade is clicked on.
	/// </summary>
	public GameObject purchaseRemoveConfirmation;

	/// <summary>
	/// The resource type in the trade popup.
	/// </summary>
	public Text resourceTypePurchaseRemove;

	/// <summary>
	/// The resource amount in the trade popup.
	/// </summary>
	public Text resourceAmountPurchaseRemove;

	/// <summary>
	/// The unit price in the trade popup.
	/// </summary>
	public Text unitPricePurchaseRemove;

	/// <summary>
	/// The total price in the trade popup.
	/// </summary>
	public Text totalPricePurchaseRemove;

	/// <summary>
	/// The player in the trade popup.
	/// </summary>
	public Text playerPurchaseRemove;

	/// <summary>
	/// The confirm trade button.
	/// </summary>
	public GameObject confirmTrade;

	/// <summary>
	/// The cancel trade button.
	/// </summary>
	public GameObject cancelTrade;

	/// <summary>
	/// The currently selected trade.
	/// </summary>
	private TradeGuiElementScript selectedTrade;

	/// <summary>
	/// Displays the list of P2PTrades given.
	/// </summary>
	/// <param name="tradesToDisplay">Trades to display.</param>
	public void DisplayCurrentTrades(List<Market.P2PTrade> tradesToDisplay) {
		ClearTradesList();
		foreach (Market.P2PTrade trade in tradesToDisplay) {
			AddTrade(trade);
		}
	}

	/// <summary>
	/// Hides the trades list.
	/// </summary>
	public void HideTradesList() {
		ClearTradesList();
		currentDisplayedTrades = new List<GameObject>();
	}

	/// <summary>
	/// Add a P2PTrade to the displayed list of trades in the UI.
	/// </summary>
	/// <param name="trade">The P2PTrade</param>
	/// <returns></returns>
	public void AddTrade(Market.P2PTrade trade) {
		LoadTradeTemplate();

		GameObject tradeGuiObject = (GameObject)Instantiate(tradePrefab);
		tradeGuiObject.transform.SetParent(tradeItemsList.transform, true);
		RectTransform guiObjectTransform = tradeGuiObject.GetComponent<RectTransform>();

		guiObjectTransform.localScale = new Vector3(1, 1, 1); //Undo Unity's instantiation meddling

		TradeGuiElementScript tradeElementScript = guiObjectTransform.GetComponent<TradeGuiElementScript>();
		tradeElementScript.SetTrade(trade);
		tradeElementScript.SetButtonEventListeners(this);

		currentDisplayedTrades.Add(tradeGuiObject);
	}

	/// <summary>
	/// Selects food as the trade resource.
	/// </summary>
	public void SelectFood() {
		Debug.Log("Press food");
		selectedResource = Data.ResourceType.FOOD;
		resourceMax = GameManager.instance.GetHumanPlayer().GetResourceAmount(Data.ResourceType.FOOD);
		tradeQuantity = 0;
		unitPrice = 0;
		tradeQuantityText.text = "0 / " + resourceMax.ToString();
		tradeUnitPriceText.text = "£0";
		tradeTotalPrice.text = "Total: £0";
		List<Market.P2PTrade> trades = new List<Market.P2PTrade>();
		foreach (Market.P2PTrade t in GameManager.instance.market.GetPlayerTrades()) {
			if (t.resource == Data.ResourceType.FOOD) {
				trades.Add(t);
			}
		}
		DisplayCurrentTrades(trades);
	}

	/// <summary>
	/// Selects energy as the trade resource.
	/// </summary>
	public void SelectEnergy() {
		Debug.Log("Press energy");
		selectedResource = Data.ResourceType.ENERGY;
		resourceMax = GameManager.instance.GetHumanPlayer().GetResourceAmount(Data.ResourceType.ENERGY);
		tradeQuantity = 0;
		unitPrice = 0;
		tradeQuantityText.text = "0 / " + resourceMax.ToString();
		tradeUnitPriceText.text = "£0";
		tradeTotalPrice.text = "Total: £0";
		List<Market.P2PTrade> trades = new List<Market.P2PTrade>();
		foreach (Market.P2PTrade t in GameManager.instance.market.GetPlayerTrades()) {
			if (t.resource == Data.ResourceType.ENERGY) {
				trades.Add(t);
			}
		}
		DisplayCurrentTrades(trades);
	}

	/// <summary>
	/// Selects ore as the trade resource.
	/// </summary>
	public void SelectOre() {
		Debug.Log("Press ore");
		selectedResource = Data.ResourceType.ORE;
		resourceMax = GameManager.instance.GetHumanPlayer().GetResourceAmount(Data.ResourceType.ORE);
		tradeQuantity = 0;
		unitPrice = 0;
		tradeQuantityText.text = "0 / " + resourceMax.ToString();
		tradeUnitPriceText.text = "£0";
		tradeTotalPrice.text = "Total: £0";
		List<Market.P2PTrade> trades = new List<Market.P2PTrade>();
		foreach (Market.P2PTrade t in GameManager.instance.market.GetPlayerTrades()) {
			if (t.resource == Data.ResourceType.ORE) {
				trades.Add(t);
			}
		}
		DisplayCurrentTrades(trades);
	}

	/// <summary>
	/// Increases the trade quantity.
	/// </summary>
	public void IncreaseQuantity() {
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			//Increase by 5?
			tradeQuantity = Math.Min(tradeQuantity + 5, resourceMax);
		} else {
			//Increase by 1
			tradeQuantity = Math.Min(tradeQuantity + 1, resourceMax);
		}
		tradeQuantityText.text = tradeQuantity.ToString() + " / " + resourceMax.ToString();
		tradeTotalPrice.text = "Total: £" + (tradeQuantity * unitPrice).ToString();
	}

	/// <summary>
	/// Decreases the trade quantity.
	/// </summary>
	public void DecreaseQuantity() {
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			//Decrease by 5?
			tradeQuantity = Math.Max(tradeQuantity - 5, 0);
		} else {
			//Decrease by 1
			tradeQuantity = Math.Max(tradeQuantity - 1, 0);
		}
		tradeQuantityText.text = tradeQuantity.ToString() + " / " + resourceMax.ToString();
		tradeTotalPrice.text = "Total: £" + (tradeQuantity * unitPrice).ToString();
	}

	/// <summary>
	/// Increases the trade unit price.
	/// </summary>
	public void IncreaseUnitPrice() {
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			//Increase by 5?
			unitPrice = unitPrice + 5;
		} else {
			//Increase by 1
			unitPrice = unitPrice + 1;
		}
		tradeUnitPriceText.text = "£" + unitPrice.ToString();
		tradeTotalPrice.text = "Total: £" + (tradeQuantity * unitPrice).ToString();
	}

	/// <summary>
	/// Decreases the trade unit price.
	/// </summary>
	public void DecreaseUnitPrice() {
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			//Decrease by 5?
			unitPrice = Math.Max(unitPrice - 5, 0);
		} else {
			//Decrease by 1
			unitPrice = Math.Max(unitPrice - 1, 0);
		}
		tradeUnitPriceText.text = "£" + unitPrice.ToString();
		tradeTotalPrice.text = "Total: £" + (tradeQuantity * unitPrice).ToString();
	}

	/// <summary>
	/// Submits the current trade.
	/// </summary>
	public void SubmitTrade() {
		if (selectedResource != null && tradeQuantity > 0 && unitPrice > 0) {
			if (canvas.ShowTradeConfirmation) {
				resourceTypeConfirmation.text = "Resource Type: " + selectedResource;
				resourceAmountConfirmation.text = "Quantity: " + tradeQuantity.ToString();
				unitPriceConfirmation.text = "Unit Price: £" + unitPrice.ToString();
				totalPriceConfirmation.text = "Total Price: £" + (tradeQuantity * unitPrice).ToString();
				tradeConfirmation.SetActive(true);
			} else {
				CreateAndSubmitTrade();
			}
		}
	}

	/// <summary>
	/// Creates and submits a new player trade.
	/// </summary>
	public void CreateAndSubmitTrade() {
		GameManager.instance.market.CreatePlayerTrade(GameManager.instance.GetHumanPlayer(), selectedResource, tradeQuantity, unitPrice);
		ResetUI();
	}

	public void FirstShow() {
		selectedResource = Data.ResourceType.NONE;
		ResetUI();
	}

	/// <summary>
	/// Resets the auction UI.
	/// </summary>
	public void ResetUI() {
		if (gameObject.activeInHierarchy) {
			ClearTradesList();
			canvas.GetHumanGui().UpdateResourceBar();
			tradeConfirmation.SetActive(false);
			purchaseRemoveConfirmation.SetActive(false);
			confirmTrade.SetActive(false);
			cancelTrade.SetActive(false);
			selectedTrade = null;
			switch (selectedResource) {
				case Data.ResourceType.FOOD:
					SelectFood();
					break;
				case Data.ResourceType.ENERGY:
					SelectEnergy();
					break;
				case Data.ResourceType.ORE:
					SelectOre();
					break;
				case Data.ResourceType.NONE:
					break;
			}
		}
	}

	/// <summary>
	/// Closes the purchase/cancel popup menu.
	/// </summary>
	public void ClosePurchaseCancelPopup() {
		ResetUI();
	}

	/// <summary>
	/// Called when the player clicks on the button in the confirmation window to cancel their own trade.
	/// </summary>
	public void CancelTrade() {
		if (selectedTrade != null) {
			GameManager.instance.market.CancelPlayerTrade(GameManager.instance.GetHumanPlayer(), selectedTrade.trade);
		}
		ResetUI();
	}

	/// <summary>
	/// Called when the player clicks the button in the confirmation menu to purchase the trade.
	/// </summary>
	public void PurchaseTrade() {
		if (selectedTrade != null) {
			GameManager.instance.market.PurchasePlayerTrade(GameManager.instance.GetHumanPlayer(), selectedTrade.trade);
		}
		ResetUI();
	}

	/// <summary>
	/// Called when a trade item is clicked on the list.
	/// Opens a dialog box corresponding to that trade.
	/// </summary>
	/// <param name="tradeElement">Trade element that was clicked.</param>
	public void TradeClicked(TradeGuiElementScript tradeElement) {	
		selectedTrade = tradeElement;
		resourceTypePurchaseRemove.text = "Resource Type: " + tradeElement.trade.resource;
		resourceAmountPurchaseRemove.text = "Quantity: " + tradeElement.trade.resourceAmount.ToString();
		unitPricePurchaseRemove.text = "Unit Price: £" + tradeElement.trade.unitPrice.ToString();
		totalPricePurchaseRemove.text = "Total Price: £" + (tradeElement.trade.resourceAmount * tradeElement.trade.unitPrice).ToString();
		playerPurchaseRemove.text = "Player: " + tradeElement.trade.host.GetName();
		if (tradeElement.trade.host == GameManager.instance.GetHumanPlayer()) {
			cancelTrade.SetActive(true);
			confirmTrade.SetActive(false);
		} else {
			confirmTrade.SetActive(true);
			cancelTrade.SetActive(false);
		}
		purchaseRemoveConfirmation.SetActive(true);
	}

	/// <summary>
	/// Clears the trades list. 
	/// </summary>
	private void ClearTradesList() {
//		Debug.Log(currentDisplayedTrades.Count);
		if (currentDisplayedTrades.Count > 0) {
			for (int i = 0; i < currentDisplayedTrades.Count; i++) {
				Destroy(currentDisplayedTrades[i]);
			}
		}
		currentDisplayedTrades = new List<GameObject>();
	}

	/// <summary>
	/// Loads the roboticon template if not already loaded.
	/// </summary>
	private void LoadTradeTemplate() {
		if (tradePrefab == null) {
			tradePrefab = (GameObject)Resources.Load(ROBOTICON_TEMPLATE_PATH);

			if (tradePrefab == null) {
				throw new ArgumentException("Cannot find trade template at the specified path.");
			}
		}
	}

}