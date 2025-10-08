using System;
using GreenT.Data;

namespace StripClub.NewEvent.Data;

[Serializable]
public class MigrateToNewbalanceMemento : Memento
{
	public bool WasMigrated;

	public bool WasMigratedSecond;

	public MigrateToNewbalanceMemento(MigrateToNewBalance migrateToNewBalance)
		: base(migrateToNewBalance)
	{
		WasMigrated = migrateToNewBalance.WasMigrated;
		WasMigratedSecond = migrateToNewBalance.WasMigratedSecond;
	}

	public void Migrate()
	{
		WasMigrated = true;
		WasMigratedSecond = true;
	}

	public bool WasTotalMigrated()
	{
		if (WasMigrated)
		{
			return WasMigratedSecond;
		}
		return false;
	}
}
