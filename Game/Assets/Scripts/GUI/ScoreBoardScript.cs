using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//NEW class attached to the score board.
public class ScoreBoardScript : MonoBehaviour {

	/// <summary>
	/// The winning player's name text.
	/// </summary>
	public Text winnerText;

	/// <summary>
	/// The winning score text.
	/// </summary>
	public Text winnerScore;

	/// <summary>
	/// The player's score text.
	/// </summary>
	public Text scoreText;

	/// <summary>
	/// Sets the winner text.
	/// </summary>
	/// <param name="human">The human player.</param>
	/// <param name="winner">The winning player.</param>
	public void SetWinnerText(HumanPlayer human, AbstractPlayer winner) {
		winnerText.text = "Winner: " + winner.GetName();
		winnerScore.text = "Winner's score: " + winner.CalculateScore();
		scoreText.text = "Your score: " + human.CalculateScore();
	}
}
