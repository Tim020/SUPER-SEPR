using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class MarketTests {

	[Test]
	public void TradeValidTest() {
		MarketController market = new MarketController();
		market.OnStartServer();

		Player player = new Player();
		player.OnStartServer();

		//Check we can't buy more resource than the market has
		player.SetFunds(10);
		market.SetResourceAmount(Data.ResourceType.ORE, 0);
		Assert.IsFalse(market.IsTradeValid(false, Data.ResourceType.ORE, 1, player));

		//Check the market can't buy more than it has money available
		player.SetResource(Data.ResourceType.ORE, 9999);
		market.SetFunds(0);
		Assert.IsFalse(market.IsTradeValid(true, Data.ResourceType.ORE, 1, player));

		//Check we can't sell more than we have resource
		player.SetResource(Data.ResourceType.ORE, 0);
		market.SetFunds(10);
		Assert.IsFalse(market.IsTradeValid(true, Data.ResourceType.ORE, 1, player));

		//Check we can't buy more than we have money
		player.SetFunds(0);
		market.SetResourceAmount(Data.ResourceType.ORE, 1);
		Assert.IsFalse(market.IsTradeValid(false, Data.ResourceType.ORE, 1, player));
	}
}
