using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardScript : MonoBehaviour {

	public Text winnerText;
	public Text scoreText;

	public void SetWinnerText(HumanPlayer human, AbstractPlayer winner) {
		winnerText.text = "Winner: " + winner.GetName();
		scoreText.text = "Your score: " + human.CalculateScore();
	}
}
