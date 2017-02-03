// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

public class GuiTextStore {

    private static string[] helpBoxText = {
        "This is the Acquisition Phase. Click on an unowned (white) tile, then click Acquire to purchase it.",
        "This is the Purchase and Customisation Phase. Click on the 'market' button in the top right to open the market. Click 'roboticons' to upgrade your robiticons.",
        "This is the Installation Phase. In the roboticons window, click 'Install' to install the roboticon to the currently selected tile.",
        "This is the Production Phase. Your resources are being produced.",
        "This is the Auction Phase."
    };

    public static string GetHelpBoxText(GameManager.GameState phase) {
        return helpBoxText[(int) phase];
    }

}