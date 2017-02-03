// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

public class HumanPlayer : AbstractPlayer {

	public HumanPlayer(ResourceGroup resources, string name, int money) {
		this.resources = resources;
		this.money = money;
		this.name = name;
	}

	public override void Act(GameManager.States state) {
	}

}