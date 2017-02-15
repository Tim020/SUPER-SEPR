using System;
using UnityEngine;
using System.Collections.Generic;

public class AuctionTradesWindow : MonoBehaviour {

	public CanvasScript canvas;
	public GameObject tradeItemsList;
	private GameObject tradePrefab;
	private List<GameObject> currentDisplayedTrades = new List<GameObject>();
	private const string ROBOTICON_TEMPLATE_PATH = "Prefabs/GUI/TemplateAuctionTrade";

	public void DisplayCurrentTrades(List<Market.P2PTrade> tradesToDisplay) {
		ClearTradesList();

		gameObject.SetActive(true);

		foreach (Market.P2PTrade trade in tradesToDisplay) {
			AddTrade(trade);
		}
	}

	public void HideRoboticonList() {
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