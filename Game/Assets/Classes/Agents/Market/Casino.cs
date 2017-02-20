// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe
using UnityEngine;
using System;

/// <summary>
/// The casino used to gamble with
/// </summary>
public class Casino {

	/// <summary>
	/// The max win percentage.
	/// </summary>
	public int maxWinPercentage{ private set; get; }

	/// <summary>
	/// Percentage of the time you win the gamble.
	/// </summary>
	public int minRollNeeded{ private set; get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Casino"/> class.
	/// </summary>
	/// <param name="minWinRoll">The maximum value for the minimum roll needed to win that can be generated.</param>
	/// <example>Setting the parameter to 35 will randomly generate the number needed to win up to that. In effect the maximum chance you have to win is 35</example>
	public Casino(int maxWinPercentage) {
		this.maxWinPercentage = maxWinPercentage;
		this.minRollNeeded = 100 - UnityEngine.Random.Range(5, this.maxWinPercentage + 1);
	}

	/// <summary>
	/// Gambles the money.
	/// Generates a random win amount each time also, up to the max value set.
	/// </summary>
	/// <param name="player">The player gambling.</param>
	/// <param name="gambleAmount">The amount of money to gamble with.</param>
	public Data.Tuple<int, bool> GambleMoney(AbstractPlayer player, int gambleAmount) {
		if (player.GetMoney() >= gambleAmount) {
			int roll = UnityEngine.Random.Range(0, 101);
			if (roll >= minRollNeeded) {
				player.GiveMoney(2 * gambleAmount);
			} else {
				player.DeductMoney(gambleAmount);
			}
			this.minRollNeeded = 100 - UnityEngine.Random.Range(5, this.maxWinPercentage + 1);
			return new Data.Tuple<int, bool>(roll, roll >= minRollNeeded);
		} else {
			throw new ArgumentException("The gamble is invalid as the player does not have enough money!");
			return new Data.Tuple<int, bool>(0, false);
		}
	}
}