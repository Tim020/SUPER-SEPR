// Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//NEW class that represents an individual trade in the list of player trades.
public class TradeGuiElementScript : MonoBehaviour {

	/// <summary>
	/// The quantity of resource text.
	/// </summary>
	public Text quantity;

	/// <summary>
	/// The unit price text.
	/// </summary>
	public Text unitPrice;

	/// <summary>
	/// The total price text.
	/// </summary>
	public Text totalPrice;

	/// <summary>
	/// The player text.
	/// </summary>
	public Text player;

	/// <summary>
	/// The trade this represents.
	/// </summary>
	public Market.P2PTrade trade;

	/// <summary>
	/// The main trades window.
	/// </summary>
	private AuctionTradesWindow tradesWindow;

	/// <summary>
	/// Sets the trade for this item.
	/// </summary>
	/// <param name="trade">The player trade.</param>
	public void SetTrade(Market.P2PTrade trade) {
		this.trade = trade;
		this.quantity.text = trade.resourceAmount.ToString();
		this.unitPrice.text = trade.unitPrice.ToString();
		this.totalPrice.text = trade.GetTotalCost().ToString();
		this.player.text = trade.host.GetName();
	}

	/// <summary>
	/// Sets the button event listeners.
	/// </summary>
	/// <param name="auctionTradesWindow">Auction trades window.</param>
	public void SetButtonEventListeners(AuctionTradesWindow auctionTradesWindow) {
		this.tradesWindow = auctionTradesWindow;
		gameObject.GetComponent<Button>().onClick.AddListener(OnTradeClicked);
	}

	/// <summary>
	/// Called when the trade item is clicked on.
	/// </summary>
	private void OnTradeClicked() {
		tradesWindow.TradeClicked(this);
	}
}