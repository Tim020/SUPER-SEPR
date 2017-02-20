using System.Collections;
using System.Collections.Generic;
using System;

public class DummyMarket : Market {

	/// <summary>
	/// Buy resources from the market.
	/// </summary>
	/// <param name="player">The player buying from the market.</param>
	/// <param name="resourcesToBuy">The resources the player is wishing to buy</param>
	/// <exception cref="System.ArgumentException">When the market does not have enough resources to complete the transaction</exception>
	public override void BuyFrom(AbstractPlayer player, ResourceGroup resourcesToBuy) {
		if (resourcesToBuy.GetFood() < 0 || resourcesToBuy.GetEnergy() < 0 || resourcesToBuy.GetOre() < 0) {
			throw new ArgumentException("Market cannot complete a transaction for negative resources.");
		}
		bool hasEnoughResources = !(resourcesToBuy.food > resources.food || resourcesToBuy.energy > resources.energy || resourcesToBuy.ore > resources.ore);
		if (hasEnoughResources) {
			UpdateMarketSupplyOnBuy(resourcesToBuy);
			resources -= resourcesToBuy;
			money = money + (resourcesToBuy * resourceSellingPrices).Sum();
			player.SetResources(player.GetResources() + resourcesToBuy);
			player.DeductMoney((resourcesToBuy * resourceSellingPrices).Sum());
		} else {
			throw new ArgumentException("Market does not have enough resources to perform this transaction.");
		}
	}

	/// <summary>
	/// Sell resources to the market.
	/// </summary>
	/// <param name="player">The player selling to the market.</param>
	/// <param name="resourcesToSell">The resources the player wishes to sell to the market</param>
	/// <exception cref="System.ArgumentException">When the market does not have enough money to complete the transaction.</exception>
	public override void SellTo(AbstractPlayer player, ResourceGroup resourcesToSell) {
		if (resourcesToSell.GetFood() < 0 || resourcesToSell.GetEnergy() < 0 || resourcesToSell.GetOre() < 0) {
			throw new ArgumentException("Market cannot complete a transaction for negative resources.");
		}
		int price = (resourcesToSell * resourceBuyingPrices).Sum();
		if (money >= price) {
			UpdateMarketSupplyOnSell(resourcesToSell);
			resources += resourcesToSell;
			money = money - price;
			player.SetResources(player.GetResources() - resourcesToSell);
			player.GiveMoney(price);
		} else {
			throw new ArgumentException("Market does not have enough money to perform this transaction.");
		}
	}

	/// <summary>
	/// Buy a Roboticon from the market if there are any.
	/// </summary>
	/// <returns>The roboticon bought by the player.</returns>
	/// <param name="player">The player buying the roboticon.</param>
	public override Roboticon BuyRoboticon(AbstractPlayer player) {
		if (GetNumRoboticonsForSale() > 0) {
			if (player.GetMoney() >= roboticonSellPrice) {
				Roboticon r = new Roboticon();
				player.AcquireRoboticon(r);
				player.DeductMoney(roboticonSellPrice);
				money += roboticonSellPrice;
				numRoboticonsForSale--;
				return r;
			}
		} 
		return null;
	}

}
