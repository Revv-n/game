using GreenT.Data;

namespace StripClub.NewEvent.Data;

[MementoHolder]
public class MigrateToNewBalance : ISavableState
{
	public bool WasMigrated { get; private set; }

	public bool WasMigratedSecond { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public MigrateToNewBalance(Saver saver)
	{
		saver.Add(this);
	}

	public void Migrate()
	{
		WasMigrated = true;
		WasMigratedSecond = true;
	}

	public string UniqueKey()
	{
		return "MigrateToNewBalance";
	}

	public Memento SaveState()
	{
		return new MigrateToNewbalanceMemento(this);
	}

	public void LoadState(Memento memento)
	{
		MigrateToNewbalanceMemento migrateToNewbalanceMemento = (MigrateToNewbalanceMemento)memento;
		WasMigrated = migrateToNewbalanceMemento.WasMigrated;
		WasMigratedSecond = migrateToNewbalanceMemento.WasMigratedSecond;
	}
}
