using System;

namespace GreenT.Data;

[Serializable]
public class MigrationDummyMemento : Memento
{
	public MigrationDummyMemento(ISavableState savableState)
		: base(savableState)
	{
	}
}
