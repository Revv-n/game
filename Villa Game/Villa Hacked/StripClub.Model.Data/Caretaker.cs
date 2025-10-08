using System;
using System.Collections.Generic;
using GreenT.Data;

namespace StripClub.Model.Data;

public class Caretaker : ICaretaker
{
	public void Add(ISavableState savable)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<T> Get<T>() where T : Memento
	{
		throw new NotImplementedException();
	}

	public void Load()
	{
		throw new NotImplementedException();
	}

	public void Remove(ISavableState savable)
	{
		throw new NotImplementedException();
	}

	public void Save()
	{
		throw new NotImplementedException();
	}
}
