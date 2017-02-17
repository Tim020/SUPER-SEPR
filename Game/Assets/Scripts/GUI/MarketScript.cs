// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.UI;
using System;

public class MarketScript : MonoBehaviour {

	/// <summary>
	/// The canvas.
	/// </summary>
	public CanvasScript canvas;

	/// <summary>
	/// The food buy price text.
	/// </summary>
	public Text foodBuyPrice;

	/// <summary>
	/// The food sell price text.
	/// </summary>
	public Text foodSellPrice;

	/// <summary>
	/// The energy buy price text.
	/// </summary>
	public Text energyBuyPrice;

	/// <summary>
	/// The energy sell price text.
	/// </summary>
	public Text energySellPrice;

	/// <summary>
	/// The ore buy price text.
	/// </summary>
	public Text oreBuyPrice;

	/// <summary>
	/// The ore sell price text.
	/// </summary>
	public Text oreSellPrice;

	/// <summary>
	/// The roboticon buy price text.
	/// </summary>
	public Text roboticonBuyPrice;

	/// <summary>
	/// The market food amount text.
	/// </summary>
	public Text marketFoodAmount;

	/// <summary>
	/// The market energy amount text.
	/// </summary>
	public Text marketEnergyAmount;

	/// <summary>
	/// The market ore amount text.
	/// </summary>
	public Text marketOreAmount;

	/// <summary>
	/// The market roboticon amount text.
	/// </summary>
	public Text marketRoboticonAmount;

	/// <summary>
	/// The total buy price text.
	/// </summary>
	public Text totalBuyPrice;

	/// <summary>
	/// The total sell price text.
	/// </summary>
	public Text totalSellPrice;

	/// <summary>
	/// The food buy amount input.
	/// </summary>
	public InputField foodBuyAmount;

	/// <summary>
	/// The food sell amount input.
	/// </summary>
	public InputField foodSellAmount;

	/// <summary>
	/// The energy buy amount input.
	/// </summary>
	public InputField energyBuyAmount;

	/// <summary>
	/// The energy sell amount input.
	/// </summary>
	public InputField energySellAmount;

	/// <summary>
	/// The ore buy amount input.
	/// </summary>
	public InputField oreBuyAmount;

	/// <summary>
	/// The ore sell amount input.
	/// </summary>
	public InputField oreSellAmount;

	/// <summary>
	/// The roboticon buy amount input.
	/// </summary>
	public InputField roboticonBuyAmount;

	/// <summary>
	/// The market money input.
	/// </summary>
	public Text marketMoney;

	/// <summary>
	/// The market.
	/// </summary>
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

	/// <summary>
	/// Raises the buy button press event.
	/// </summary>
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

	/// <summary>
	/// Raises the sell button press event.
	/// </summary>
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

	/// <summary>
	/// Plays the purchase declined animation.
	/// </summary>
	public void PlayPurchaseDeclinedAnimation() {
		totalBuyPrice.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
	}

	/// <summary>
	/// Plays the sale declined animation.
	/// </summary>
	public void PlaySaleDeclinedAnimation() {
		totalSellPrice.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
	}

	/// <summary>
	/// Sets the market resource values.
	/// </summary>
	public void SetMarketValues() {
		if (gameObject.activeInHierarchy) {
			UpdateShownMarketPrices();
			UpdateTotalBuyPrice();
			UpdateTotalSellPrice();
			UpdateMarketResourceAmounts();
		}
	}

	/// <summary>
	/// Updates the total buy price.
	/// </summary>
	public void UpdateTotalBuyPrice() {
		ResourceGroup buyingPrices = market.GetResourceBuyingPrices();

		int foodPrice = int.Parse(foodBuyAmount.text) * buyingPrices.food;
		int energyPrice = int.Parse(energyBuyAmount.text) * buyingPrices.energy;
		int orePrice = int.Parse(oreBuyAmount.text) * buyingPrices.ore;
		int roboticonPrice = int.Parse(roboticonBuyAmount.text) * market.GetRoboticonSellingPrice();

		totalBuyPrice.text = "£" + (foodPrice + energyPrice + orePrice + roboticonPrice).ToString();

	}

	/// <summary>
	/// Updates the total sell price.
	/// </summary>
	public void UpdateTotalSellPrice() {
		ResourceGroup sellingPrices = market.GetResourceSellingPrices();

		int foodPrice = int.Parse(foodSellAmount.text) * sellingPrices.food;
		int energyPrice = int.Parse(energySellAmount.text) * sellingPrices.energy;
		int orePrice = int.Parse(oreSellAmount.text) * sellingPrices.ore;

		totalSellPrice.text = "£" + (foodPrice + energyPrice + orePrice).ToString();

	}

	/// <summary>
	/// Updates the market resource prices.
	/// </summary>
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

	/// <summary>
	/// Updates the market resource amounts.
	/// </summary>
	private void UpdateMarketResourceAmounts() {
		ResourceGroup marketResources = market.GetResources();
		marketFoodAmount.text = "/" + marketResources.food.ToString();
		marketEnergyAmount.text = "/" + marketResources.energy.ToString();
		marketOreAmount.text = "/" + marketResources.ore.ToString();
		marketRoboticonAmount.text = "/" + market.GetNumRoboticonsForSale();
	}

}