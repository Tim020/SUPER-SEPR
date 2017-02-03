// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

public class HumanPlayer : AbstractPlayer {

	private HumanGui humanGui;

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

	public void SetGuiElement(HumanGui gui, CanvasScript canvas) {
		this.humanGui = gui;
		humanGui.SetCanvasScript(canvas);
		humanGui.SetGameManager(GameHandler.GetGameManager());
		canvas.SetHumanGui(humanGui);
		humanGui.DisplayGui(this, Data.GameState.ACQUISITION);
	}

	/// <summary>
	/// Act based on the specified state.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public override void Act(Data.GameState state) {
		humanGui.DisableGui();
		humanGui.SetCurrentPlayerName(this.name);
		//TODO: Something?
		humanGui.DisplayGui(this, state);
	}

	public HumanGui GetHumanGui() {
		return humanGui;
	}

}