// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

public class HumanPlayer : AbstractPlayer {

	/// <summary>
	/// Initializes a new instance of the <see cref="HumanPlayer"/> class.
	/// </summary>
	/// <param name="resources">Starting resources.</param>
	/// <param name="name">Name.</param>
	/// <param name="money">Starting money.</param>
	public HumanPlayer(ResourceGroup resources, string name, int money) {
		this.resources = resources;
		this.name = name;
		this.money = money;
	}

	/// <summary>
	/// Act based on the specified state.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public override void Act(GameManager.States state) {
	}

}