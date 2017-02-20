using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardScript : MonoBehaviour {

	public Text winnerText;
	public Text winnerScore;
	public Text scoreText;

	public void SetWinnerText(HumanPlayer human, AbstractPlayer winner) {
		winnerText.text = "Winner: " + winner.GetName();
		winnerScore.text = "Winner's score: " + winner.CalculateScore();
		scoreText.text = "Your score: " + human.CalculateScore();
	}
}
