using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeGuiElementScript : MonoBehaviour {

	public Text quantity;
	public Text unitPrice;
	public Text totalPrice;
	public Text player;
	public Market.P2PTrade trade;
	private AuctionTradesWindow tradesWindow;

	public void SetTrade(Market.P2PTrade trade) {
		this.trade = trade;
		this.quantity.text = trade.resourceAmount.ToString();
		this.unitPrice.text = trade.unitPrice.ToString();
		this.totalPrice.text = trade.GetTotalCost().ToString();
		this.player.text = trade.host.GetName();
	}

	public void SetButtonEventListeners(AuctionTradesWindow auctionTradesWindow) {
		this.tradesWindow = auctionTradesWindow;
		gameObject.GetComponent<Button>().onClick.AddListener(OnTradeClicked);
	}

	private void OnTradeClicked() {
		tradesWindow.TradeClicked(this);
	}
}