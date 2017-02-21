// Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing
public abstract class RandomEvent {

    /// <summary>
	/// NEW: This is a new section of code that deals with starting the event.
    /// The method all inheriting events must implement. Called when the event starts.
    /// </summary>
    public abstract void InvokeEvent();
}