// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

public abstract class Agent {

	/// <summary>
	/// The money of this agent.
	/// </summary>
    protected int money;

	/// <summary>
	/// The resources owned by this agent.
	/// </summary>
    protected ResourceGroup resources;

	/// <summary>
	/// Gets the resources.
	/// </summary>
	/// <returns>The resources.</returns>
    public ResourceGroup GetResources() {
        return resources;
    }

	/// <summary>
	/// Sets the resources of the agent.
	/// </summary>
	/// <param name="resourcesToSet">Resources to set.</param>
    public void SetResources(ResourceGroup resourcesToSet) {
        resources = resourcesToSet;
    }

	/// <summary>
	/// Gets the money for this agent.
	/// </summary>
	/// <returns>The money.</returns>
    public int GetMoney() {
        return money;
    }

	/// <summary>
	/// Sets the agent's money.
	/// </summary>
	/// <param name="moneyToSet">Amount of money.</param>
    public void SetMoney(int moneyToSet) {
        money = moneyToSet;
    }

}