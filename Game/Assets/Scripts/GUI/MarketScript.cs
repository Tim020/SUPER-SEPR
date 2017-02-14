// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.UI;

public class MarketScript : MonoBehaviour {

    public CanvasScript uiCanvas;

    #region Resource price labels

    public Text foodBuyPrice;
    public Text foodSellPrice;
    public Text energyBuyPrice;
    public Text energySellPrice;
    public Text oreBuyPrice;
    public Text oreSellPrice;
    public Text roboticonBuyPrice;

    public Text totalBuyPrice;
    public Text totalSellPrice;

	public Text marketMoney;
    #endregion

    #region Resource amount labels

    public InputField foodBuyAmount;
    public InputField foodSellAmount;
    public InputField energyBuyAmount;
    public InputField energySellAmount;
    public InputField oreBuyAmount;
    public InputField oreSellAmount;
    public InputField roboticonBuyAmount;

    #endregion

    private Market market;

    public char ValidatePositiveInput(string text, int charIndex, char addedChar) {
        int tryParseResult;

        if (int.TryParse(addedChar.ToString(), out tryParseResult)) //Only accept characters which are integers (no '-')
        {
            return addedChar;
        } else {
            return '\0'; //Empty string character
        }
    }

    // Use this for initialization
    void Start() {
        market = GameHandler.GetGameManager().market;
        SetShownMarketPrices();

        foodBuyAmount.onValidateInput += ValidatePositiveInput; //Add the ValidatePositiveInput function to
        energyBuyAmount.onValidateInput += ValidatePositiveInput; //each GUI Text.
        oreBuyAmount.onValidateInput += ValidatePositiveInput;

        foodSellAmount.onValidateInput += ValidatePositiveInput;
        energySellAmount.onValidateInput += ValidatePositiveInput;
        oreSellAmount.onValidateInput += ValidatePositiveInput;
    }

    public void OnBuyButtonPress() {
		marketMoney.text = "£" + market.GetMarketMoney().ToString ();

        ResourceGroup resourcesToBuy = new ResourceGroup();
        resourcesToBuy.food = int.Parse(foodBuyAmount.text);
        resourcesToBuy.energy = int.Parse(energyBuyAmount.text);
        resourcesToBuy.ore = int.Parse(oreBuyAmount.text);
        int roboticonsToBuy = int.Parse(roboticonBuyAmount.text);

        int buyPrice = int.Parse(totalBuyPrice.text.Substring(1));

        uiCanvas.BuyFromMarket(resourcesToBuy, roboticonsToBuy, buyPrice);

    }

    public void OnSellButtonPress() {
		marketMoney.text = "£" + market.GetMarketMoney().ToString ();

        ResourceGroup resourcesToSell = new ResourceGroup();
        resourcesToSell.food = int.Parse(foodSellAmount.text);
        resourcesToSell.energy = int.Parse(energySellAmount.text);
        resourcesToSell.ore = int.Parse(oreSellAmount.text);

        int sellPrice = int.Parse(totalSellPrice.text.Substring(1));

        uiCanvas.SellToMarket(resourcesToSell, sellPrice);
	}

    public void PlayPurchaseDeclinedAnimation() {
        totalBuyPrice.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
    }

    public void PlaySaleDeclinedAnimation() {
        totalSellPrice.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
    }

    public void SetShownMarketPrices() {
        UpdateShownMarketPrices();
        UpdateTotalBuyPrice();
        UpdateTotalSellPrice();
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

		marketMoney.text = "£" + market.GetMarketMoney().ToString ();
    }

}