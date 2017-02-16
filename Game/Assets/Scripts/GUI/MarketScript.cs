// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.UI;
using System;

public class MarketScript : MonoBehaviour {

	public CanvasScript canvas;

	public Text foodBuyPrice;
	public Text foodSellPrice;
	public Text energyBuyPrice;
	public Text energySellPrice;
	public Text oreBuyPrice;
	public Text oreSellPrice;
	public Text roboticonBuyPrice;
	public Text marketFoodAmount;
	public Text marketEnergyAmount;
	public Text marketOreAmount;
	public Text marketRoboticonAmount;

	public Text totalBuyPrice;
	public Text totalSellPrice;

	public InputField foodBuyAmount;
	public InputField foodSellAmount;
	public InputField energyBuyAmount;
	public InputField energySellAmount;
	public InputField oreBuyAmount;
	public InputField oreSellAmount;
	public InputField roboticonBuyAmount;
	public Text marketMoney;

	private Market market;

	public char ValidatePositiveInput(string text, int charIndex, char addedChar) {
		int tryParseResult;

		if (int.TryParse(addedChar.ToString(), out tryParseResult)) { //Only accept characters which are integers (no '-')
			return addedChar;
		} else {
			return '\0'; //Empty string character
		}
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		market = GameHandler.GetGameManager().market;
		SetMarketValues();

		foodBuyAmount.onValidateInput += ValidatePositiveInput; //Add the ValidatePositiveInput function to
		energyBuyAmount.onValidateInput += ValidatePositiveInput; //each GUI Text.
		oreBuyAmount.onValidateInput += ValidatePositiveInput;

		foodSellAmount.onValidateInput += ValidatePositiveInput;
		energySellAmount.onValidateInput += ValidatePositiveInput;
		oreSellAmount.onValidateInput += ValidatePositiveInput;
	}

	/// <summary>
	/// Enables the interactions with the input boxes for resource amounts.
	/// </summary>
	public void EnableResourceInteractions() {
		foodBuyAmount.interactable = true;
		energyBuyAmount.interactable = true;
		oreBuyAmount.interactable = true;
		foodSellAmount.interactable = true;
		energySellAmount.interactable = true;
		oreSellAmount.interactable = true;
	}

	/// <summary>
	/// Disables the interactions with the input boxes for resource amounts.
	/// </summary>
	public void DisableResourceInteractions() {
		foodBuyAmount.interactable = false;
		energyBuyAmount.interactable = false;
		oreBuyAmount.interactable = false;
		foodSellAmount.interactable = false;
		energySellAmount.interactable = false;
		oreSellAmount.interactable = false;
	}

	public void OnBuyButtonPress() {
		marketMoney.text = "£" + market.GetMarketMoney().ToString();

		ResourceGroup resourcesToBuy = new ResourceGroup();
		resourcesToBuy.food = int.Parse(foodBuyAmount.text);
		resourcesToBuy.energy = int.Parse(energyBuyAmount.text);
		resourcesToBuy.ore = int.Parse(oreBuyAmount.text);
		int roboticonsToBuy = int.Parse(roboticonBuyAmount.text);
		int buyPrice = int.Parse(totalBuyPrice.text.Substring(1));

		if (GameHandler.GetGameManager().GetHumanPlayer().GetMoney() >= buyPrice && market.GetNumRoboticonsForSale() >= roboticonsToBuy) {
			try {
				GameHandler.GetGameManager().market.BuyFrom(GameManager.instance.GetHumanPlayer(), resourcesToBuy);
			} catch (ArgumentException) {
				canvas.marketScript.PlayPurchaseDeclinedAnimation();
				return;
			}
			for (int i = 0; i < roboticonsToBuy; i++) {
				Roboticon newRoboticon = market.BuyRoboticon(GameManager.instance.GetHumanPlayer());
				canvas.AddRoboticonToList(newRoboticon);
			}
			canvas.GetHumanGui().UpdateResourceBar();
		} else {
			canvas.marketScript.PlayPurchaseDeclinedAnimation();
		}
	}

	public void OnSellButtonPress() {
		marketMoney.text = "£" + market.GetMarketMoney().ToString();

		ResourceGroup resourcesToSell = new ResourceGroup();
		resourcesToSell.food = int.Parse(foodSellAmount.text);
		resourcesToSell.energy = int.Parse(energySellAmount.text);
		resourcesToSell.ore = int.Parse(oreSellAmount.text);

		int sellPrice = int.Parse(totalSellPrice.text.Substring(1));

		ResourceGroup humanResources = GameHandler.GetGameManager().GetHumanPlayer().GetResources();
		bool hasEnoughResources = humanResources.food >= resourcesToSell.food && humanResources.energy >= resourcesToSell.energy && humanResources.ore >= resourcesToSell.ore;
		if (hasEnoughResources) {
			try {
				GameHandler.GetGameManager().market.SellTo(GameManager.instance.GetHumanPlayer(), resourcesToSell);
			} catch (ArgumentException e) {
				//TODO - Implement separate animation for when the market does not have enough resources
				canvas.marketScript.PlaySaleDeclinedAnimation();
				return;
			}
			canvas.GetHumanGui().UpdateResourceBar();
		} else {
			canvas.marketScript.PlaySaleDeclinedAnimation();
		}
	}

	public void PlayPurchaseDeclinedAnimation() {
		totalBuyPrice.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
	}

	public void PlaySaleDeclinedAnimation() {
		totalSellPrice.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
	}

	public void SetMarketValues() {
		UpdateShownMarketPrices();
		UpdateTotalBuyPrice();
		UpdateTotalSellPrice();
		UpdateMarketResourceAmounts();
	}

	public void UpdateTotalBuyPrice() {
		ResourceGroup buyingPrices = market.GetResourceBuyingPrices();

		int foodPrice = int.Parse(foodBuyAmount.text) * buyingPrices.food;
		int energyPrice = int.Parse(energyBuyAmount.text) * buyingPrices.energy;
		int orePrice = int.Parse(oreBuyAmount.text) * buyingPrices.ore;
		int roboticonPrice = int.Parse(roboticonBuyAmount.text) * market.GetRoboticonSellingPrice();

		totalBuyPrice.text = "£" + (foodPrice + energyPrice + orePrice + roboticonPrice).ToString();

	}

	public void UpdateTotalSellPrice() {
		ResourceGroup sellingPrices = market.GetResourceSellingPrices();

		int foodPrice = int.Parse(foodSellAmount.text) * sellingPrices.food;
		int energyPrice = int.Parse(energySellAmount.text) * sellingPrices.energy;
		int orePrice = int.Parse(oreSellAmount.text) * sellingPrices.ore;

		totalSellPrice.text = "£" + (foodPrice + energyPrice + orePrice).ToString();

	}

	private void UpdateShownMarketPrices() {
		ResourceGroup sellingPrices = market.GetResourceSellingPrices();
		ResourceGroup buyingPrices = market.GetResourceBuyingPrices();

		foodBuyPrice.text = "£" + buyingPrices.food.ToString();
		energyBuyPrice.text = "£" + buyingPrices.energy.ToString();
		oreBuyPrice.text = "£" + buyingPrices.ore.ToString();
		roboticonBuyPrice.text = "£" + market.GetRoboticonSellingPrice().ToString();

		foodSellPrice.text = "£" + sellingPrices.food.ToString();
		energySellPrice.text = "£" + sellingPrices.energy.ToString();
		oreSellPrice.text = "£" + sellingPrices.ore.ToString();

		marketMoney.text = "£" + market.GetMarketMoney().ToString();
	}

	private void UpdateMarketResourceAmounts() {
		ResourceGroup marketResources = market.GetResources();
		marketFoodAmount.text = "/" + marketResources.food.ToString();
		marketEnergyAmount.text = "/" + marketResources.energy.ToString();
		marketOreAmount.text = "/" + marketResources.ore.ToString();
		marketRoboticonAmount.text = "/" + market.GetNumRoboticonsForSale();
	}

}