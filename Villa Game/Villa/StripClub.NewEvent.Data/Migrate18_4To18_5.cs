using System;
using GreenT.Data;
using GreenT.HornyScapes;
using GreenT.Types;
using StripClub.Model;
using StripClub.NewEvent.Model;

namespace StripClub.NewEvent.Data;

[MementoHolder]
public class Migrate18_4To18_5 : ISavableState
{
	[Serializable]
	public class MigrateVersionDataMemento : Memento
	{
		public bool IsMigrate;

		public MigrateVersionDataMemento(Migrate18_4To18_5 savableState)
			: base(savableState)
		{
			IsMigrate = savableState.IsMigrate;
		}
	}

	private readonly GreenT.HornyScapes.IPlayerBasics playerBasics;

	public static Migrate18_4To18_5 Instance;

	public string version = "18.4";

	private readonly string saveKey;

	public bool IsMigrate { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Migrate18_4To18_5(ISaver saver, GreenT.HornyScapes.IPlayerBasics playerBasics)
	{
		this.playerBasics = playerBasics;
		saveKey = "MigrateVersionData_" + version;
		saver.Add(this);
		Instance = this;
	}

	public static bool NeedMigrate()
	{
		if (Instance == null)
		{
			return false;
		}
		return !Instance.IsMigrate;
	}

	public static int GetCurrency(EventCurrencyType eventCurrencyType)
	{
		int result = 0;
		if (Instance == null)
		{
			return 0;
		}
		switch (eventCurrencyType)
		{
		case EventCurrencyType.XP:
			result = Instance.playerBasics.Balance[CurrencyType.EventXP, default(CompositeIdentificator)].Count.Value;
			break;
		case EventCurrencyType.Core:
			result = Instance.playerBasics.Balance[CurrencyType.Event, default(CompositeIdentificator)].Count.Value;
			break;
		}
		return result;
	}

	public static void SetMigrate()
	{
		if (Instance != null)
		{
			Instance.IsMigrate = true;
		}
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
