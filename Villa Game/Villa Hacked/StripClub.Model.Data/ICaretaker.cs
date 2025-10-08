using System.Collections.Generic;
using GreenT.Data;

namespace StripClub.Model.Data;

public interface ICaretaker
{
	void Add(ISavableState savable);

	void Remove(ISavableState savable);

	IEnumerable<T> Get<T>() where T : Memento;

	void Load();

	void Save();
}
