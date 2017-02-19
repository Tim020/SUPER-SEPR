using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CasinoScript : MonoBehaviour {

	private Casino casino;
	public CanvasScript canvas;
	public InputField moneyInput;
	public Image chanceDigit0;
	public Image chanceDigit1;
	public Image chanceDigit2;
	public Image rollDigit0;
	public Image rollDigit1;
	public Image rollDigit2;
	public Text textMessage;
	public Sprite num0;
	public Sprite num1;
	public Sprite num2;
	public Sprite num3;
	public Sprite num4;
	public Sprite num5;
	public Sprite num6;
	public Sprite num7;
	public Sprite num8;
	public Sprite num9;

	public char ValidatePositiveInput(string text, int charIndex, char addedChar) {
		int tryParseResult;

		if (int.TryParse(addedChar.ToString(), out tryParseResult)) { //Only accept characters which are integers (no '-')
			return addedChar;
		} else {
			return '\0'; //Empty string character
		}
	}

	// Use this for initialization
	void Start() {
		this.casino = GameManager.instance.casino;
		SetMinRollNumber();
		moneyInput.onValidateInput += ValidatePositiveInput;
	}

	/// <summary>
	/// Called when the gamble button is pressed.
	/// </summary>
	public void GambleButtonPressed() {
		if (moneyInput.text != "") {
			int moneyToGamble = int.Parse(moneyInput.text);
			if (GameManager.instance.GetHumanPlayer().GetMoney() >= moneyToGamble) {
				Data.Tuple<int, bool> casinoReturn = casino.GambleMoney(GameManager.instance.GetHumanPlayer(), moneyToGamble);
				SetPlayerRollNumber(casinoReturn.Head);
				if (casinoReturn.Tail) {
					textMessage.text = "You won! :)";
				} else {
					textMessage.text = "You lost! :(";
				}
			} else {
				textMessage.text = "Not enough money!";
			}
			moneyInput.text = "";
			SetMinRollNumber();
			canvas.GetHumanGui().UpdateResourceBar();
		} else {
			textMessage.text = "No money given!";
		}
	}

	public void SetMinRollNumber() {
		string s = casino.minRollNeeded.ToString();
		if (s.Length == 3) {
			chanceDigit0.sprite = GetSpriteForInt(int.Parse(s[0].ToString()));
			chanceDigit1.sprite = GetSpriteForInt(int.Parse(s[1].ToString()));
			chanceDigit2.sprite = GetSpriteForInt(int.Parse(s[2].ToString()));
		} else if (s.Length == 2) {
			chanceDigit0.sprite = GetSpriteForInt(0);
			chanceDigit1.sprite = GetSpriteForInt(int.Parse(s[0].ToString()));
			chanceDigit2.sprite = GetSpriteForInt(int.Parse(s[1].ToString()));
		} else if (s.Length == 1) {
			chanceDigit0.sprite = GetSpriteForInt(0);
			chanceDigit1.sprite = GetSpriteForInt(0);
			chanceDigit2.sprite = GetSpriteForInt(int.Parse(s[0].ToString()));
		}
	}

	public void SetPlayerRollNumber(int roll) {
		string s = roll.ToString();
		if (s.Length == 3) {
			rollDigit0.sprite = GetSpriteForInt(int.Parse(s[0].ToString()));
			rollDigit1.sprite = GetSpriteForInt(int.Parse(s[1].ToString()));
			rollDigit2.sprite = GetSpriteForInt(int.Parse(s[2].ToString()));
		} else if (s.Length == 2) {
			rollDigit0.sprite = GetSpriteForInt(0);
			rollDigit1.sprite = GetSpriteForInt(int.Parse(s[0].ToString()));
			rollDigit2.sprite = GetSpriteForInt(int.Parse(s[1].ToString()));
		} else if (s.Length == 1) {
			rollDigit0.sprite = GetSpriteForInt(0);
			rollDigit1.sprite = GetSpriteForInt(0);
			rollDigit2.sprite = GetSpriteForInt(int.Parse(s[0].ToString()));
		}
	}

	private Sprite GetSpriteForInt(int i) {
		if (i <= 9 && i >= 0) {
			switch (i) {
				case 0:
					return num0;
					break;
				case 1:
					return num1;
					break;
				case 2:
					return num2;
					break;
				case 3:
					return num3;
					break;
				case 4:
					return num4;
					break;
				case 5:
					return num5;
					break;
				case 6:
					return num6;
					break;
				case 7:
					return num7;
					break;
				case 8:
					return num8;
					break;
				case 9:
					return num9;
					break;
			}
		} else {
			throw new ArgumentOutOfRangeException("Input was out of the range 0-9");
		}
		return null;
	}
}
