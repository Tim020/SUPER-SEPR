// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

public abstract class RandomEvent {

    /// <summary>
	/// NEW: This is a new section of code that deals with starting the event.
    /// The method all inheriting events must implement. Called when the event starts.
    /// </summary>
    public abstract void InvokeEvent();
}