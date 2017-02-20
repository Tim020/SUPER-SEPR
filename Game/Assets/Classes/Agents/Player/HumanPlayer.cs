// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

public class HumanPlayer : AbstractPlayer {

	private HumanGui humanGui;

	/// <summary>
	/// Initializes a new instance of the <see cref="HumanPlayer"/> class.
	/// </summary>
	/// <param name="resources">Starting resources.</param>
	/// <param name="name">Name.</param>
	/// <param name="money">Starting money.</param>
	public HumanPlayer(ResourceGroup resources, int ID, string name, int money) {
		this.playerID = ID;
		this.resources = resources;
		this.name = name;
		this.money = money;
	}

	/// <summary>
	/// Sets up the GUI elements
	/// </summary>
	/// <param name="gui">The HumanGui interface class.</param>
	/// <param name="canvas">The actual UI script.</param>
	public void SetGuiElement(HumanGui gui, CanvasScript canvas) {
		this.humanGui = gui;
		humanGui.SetCanvasScript(canvas);
		canvas.SetHumanGui(humanGui);
		humanGui.DisplayGui(Data.GameState.TILE_PURCHASE);
	}

	/// <summary>
	/// Act based on the specified state.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public override void StartPhase(Data.GameState state) {
		humanGui.DisableGui();
		humanGui.SetCurrentPlayerName(this.name);
		humanGui.DisplayGui(state);
	}

	/// <summary>
	/// Gets the GUI element for this player.
	/// </summary>
	/// <returns>The main GUI overlay.</returns>
	public HumanGui GetHumanGui() {
		return humanGui;
	}

}