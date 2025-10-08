namespace GreenT.Data;

public interface ISaver
{
	void Add(ISavableState savable);

	bool Remove(ISavableState savable);

	bool Delete(ISavableState savable);

	bool TryGetMemento(string uniqKey, out Memento memento);

	void DeleteHashTablePoint(string uniqKey);
}
