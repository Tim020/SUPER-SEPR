using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AuctionTradesWindow : MonoBehaviour {

	public CanvasScript canvas;
	public GameObject tradeItemsList;
	private GameObject tradePrefab;
	private List<GameObject> currentDisplayedTrades = new List<GameObject>();
	private const string ROBOTICON_TEMPLATE_PATH = "Prefabs/GUI/TemplateAuctionTrade";
	private Data.ResourceType selectedResource;
	private int resourceMax;

	public Text tradeQuantityText;
	public Text tradeUnitPriceText;
	public Text tradeTotalPrice;
	private int tradeQuantity;
	private int unitPrice;

	public GameObject tradeConfirmation;
	public Text resourceTypeConfirmation;
	public Text resourceAmountConfirmation;
	public Text unitPriceConfirmation;
	public Text totalPriceConfirmation;

	/// <summary>
	/// Displays the list of P2PTrades given.
	/// </summary>
	/// <param name="tradesToDisplay">Trades to display.</param>
	public void DisplayCurrentTrades(List<Market.P2PTrade> tradesToDisplay) {
		ClearTradesList();

		gameObject.SetActive(true);

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
		gameObject.SetActive(false);
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

	public void CreateAndSubmitTrade() {
		GameManager.instance.market.CreatePlayerTrade(GameManager.instance.GetHumanPlayer(), selectedResource, tradeQuantity, unitPrice);
		ResetUI();
	}

	public void ResetUI() {
		canvas.GetHumanGui().UpdateResourceBar();
		tradeConfirmation.SetActive(false);
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
		}
	}

	public void TradeClicked(TradeGuiElementScript tradeGuiElementScript) {
		
	}

	/// <summary>
	/// Clears the trades list. 
	/// </summary>
	private void ClearTradesList() {
		if (currentDisplayedTrades.Count > 0) {
			for (int i = currentDisplayedTrades.Count - 1; i >= 0; i--) {
				Destroy(currentDisplayedTrades[i]);
			}
			currentDisplayedTrades = new List<GameObject>();
		}
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