// Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//NEW class that is attached to the casino UI.
public class CasinoScript : MonoBehaviour {

	/// <summary>
	/// The casino.
	/// </summary>
	private Casino casino;

	/// <summary>
	/// The canvas.
	/// </summary>
	public CanvasScript canvas;

	/// <summary>
	/// The gamble amount input.
	/// </summary>
	public InputField moneyInput;

	/// <summary>
	/// The win chance digit 0.
	/// </summary>
	public Image chanceDigit0;

	/// <summary>
	/// The win chance digit 1.
	/// </summary>
	public Image chanceDigit1;

	/// <summary>
	/// The win chance digit 2.
	/// </summary>
	public Image chanceDigit2;

	/// <summary>
	/// The roll digit 0.
	/// </summary>
	public Image rollDigit0;

	/// <summary>
	/// The roll digit 1.
	/// </summary>
	public Image rollDigit1;

	/// <summary>
	/// The roll digit 2.
	/// </summary>
	public Image rollDigit2;

	/// <summary>
	/// The message at the bottom of the UI.
	/// </summary>
	public Text textMessage;

	/// <summary>
	/// Sprite for 0.
	/// </summary>
	public Sprite num0;

	/// <summary>
	/// Sprite for 1.
	/// </summary>
	public Sprite num1;

	/// <summary>
	/// Sprite for 2.
	/// </summary>
	public Sprite num2;

	/// <summary>
	/// Sprite for 3.
	/// </summary>
	public Sprite num3;

	/// <summary>
	/// Sprite for 4.
	/// </summary>
	public Sprite num4;

	/// <summary>
	/// Sprite for 5.
	/// </summary>
	public Sprite num5;

	/// <summary>
	/// Sprite for 6.
	/// </summary>
	public Sprite num6;

	/// <summary>
	/// Sprite for 7.
	/// </summary>
	public Sprite num7;

	/// <summary>
	/// Sprite for 8.
	/// </summary>
	public Sprite num8;

	/// <summary>
	/// Sprite for 9.
	/// </summary>
	public Sprite num9;

	/// <summary>
	/// Validates for a positive number input.
	/// </summary>
	/// <returns>The char that was entered if allowed, else the empty character.</returns>
	/// <param name="text">Text.</param>
	/// <param name="charIndex">Char index.</param>
	/// <param name="addedChar">Added char.</param>
	public char ValidatePositiveInput(string text, int charIndex, char addedChar) {
		int tryParseResult;
		if (int.TryParse(addedChar.ToString(), out tryParseResult)) {
			return addedChar;
		} else {
			return '\0'; //Empty string character
		}
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
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

	/// <summary>
	/// Sets the minimum roll number display on the UI.
	/// </summary>
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

	/// <summary>
	/// Sets the player roll number on the UI.
	/// </summary>
	/// <param name="roll">The number the player rolled.</param>
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

	/// <summary>
	/// Gets the sprite for number input.
	/// </summary>
	/// <returns>The sprite for the specified int.</returns>
	/// <param name="i">A number.</param>
	/// <exception cref="System.ArgumentOutOfRangeException">When the number is not a digit (ie greater than 9).</exception>
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
