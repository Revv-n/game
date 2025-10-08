namespace StripClub.NewEvent.Save;

public interface IEventSavableState
{
	string UniqueKey();

	EventMemento SaveState();

	void LoadState(EventMemento memento);
}
