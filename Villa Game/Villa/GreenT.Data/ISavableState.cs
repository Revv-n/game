namespace GreenT.Data;

[MementoHolder]
public interface ISavableState
{
	SavableStatePriority Priority { get; }

	string UniqueKey();

	Memento SaveState();

	void LoadState(Memento memento);
}
