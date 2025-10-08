using System;
using GreenT.Data;

namespace StripClub.NewEvent.Data;

[Serializable]
public class MigrationDeleteMissingItemsMemento : Memento
{
	public int MainFieldMigrationsDone { get; private set; }

	public int EventFieldMigrationsDone { get; private set; }

	public MigrationDeleteMissingItemsMemento(MigrationDeleteMissingItems migrationDeleteMissingItems)
		: base(migrationDeleteMissingItems)
	{
		MainFieldMigrationsDone = migrationDeleteMissingItems.MainFieldMigrationsDone;
		EventFieldMigrationsDone = migrationDeleteMissingItems.EventFieldMigrationsDone;
	}
}
