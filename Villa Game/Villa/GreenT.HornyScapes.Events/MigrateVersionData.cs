using System;
using GreenT.Data;

namespace GreenT.HornyScapes.Events;

[MementoHolder]
public class MigrateVersionData : ISavableState
{
	[Serializable]
	public class MigrateVersionDataMemento : Memento
	{
		public bool IsMigrate;

		public MigrateVersionDataMemento(MigrateVersionData savableState)
			: base(savableState)
		{
			IsMigrate = savableState.IsMigrate;
		}
	}

	public string version = "12.4";

	public bool IsMigrate;

	private readonly string saveKey;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public MigrateVersionData(ISaver saver)
	{
		saveKey = "MigrateVersionData_" + version;
		saver.Add(this);
	}

	public string UniqueKey()
	{
		return saveKey;
	}

	public Memento SaveState()
	{
		return new MigrateVersionDataMemento(this);
	}

	public void LoadState(Memento memento)
	{
		MigrateVersionDataMemento migrateVersionDataMemento = (MigrateVersionDataMemento)memento;
		IsMigrate = migrateVersionDataMemento.IsMigrate;
	}
}
