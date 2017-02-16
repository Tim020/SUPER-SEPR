using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AuctionTradesWindow : MonoBehaviour {

	public CanvasScript canvas;
	public GameObject tradeItemsList;
	private GameObject tradePrefab;
	public Text tradeMaxQuantity;
	private List<GameObject> currentDisplayedTrades = new List<GameObject>();
	private const string ROBOTICON_TEMPLATE_PATH = "Prefabs/GUI/TemplateAuctionTrade";
	private Data.ResourceType selectedResource;
	private int resourceMax;
	private int tradeQuantity;

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
		tradeMaxQuantity.text = "0 / " + resourceMax.ToString();
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
		tradeMaxQuantity.text = "0 / " + resourceMax.ToString();
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
		tradeMaxQuantity.text = "0 / " + resourceMax.ToString();
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
		tradeMaxQuantity.text = tradeQuantity.ToString() + " / " + resourceMax.ToString();
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
		tradeMaxQuantity.text = tradeQuantity.ToString() + " / " + resourceMax.ToString();
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