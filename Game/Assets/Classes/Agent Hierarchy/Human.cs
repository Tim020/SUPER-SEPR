// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

public class Human : Player {

    private HumanGui humanGui;

    public Human(ResourceGroup resources, string name, int money) {
        this.resources = resources;
        this.money = money;
        this.name = name;
    }

    public void SetHumanGui(HumanGui gui) {
        humanGui = gui;
    }

    public override void Act(GameManager.States state) {
        humanGui.DisplayGui(this, state);
    }

}