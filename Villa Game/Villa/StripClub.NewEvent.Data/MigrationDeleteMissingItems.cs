using System;
using GreenT.Data;
using GreenT.HornyScapes.Constants;
using GreenT.Types;

namespace StripClub.NewEvent.Data;

[MementoHolder]
public class MigrationDeleteMissingItems : ISavableState
{
	public readonly int MAIN_FIELD_DELETE_MIGRATION_VERSION;

	public readonly int EVENT_FIELD_DELETE_MIGRATION_VERSION;

	public int MainFieldMigrationsDone { get; private set; }

	public int EventFieldMigrationsDone { get; private set; }

	public bool IsNeedMainFieldMigration => true;

	public bool IsNeedEventFieldMigration => true;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public MigrationDeleteMissingItems(Saver saver, IConstants<int> intConstants)
	{
		saver.Add(this);
		MAIN_FIELD_DELETE_MIGRATION_VERSION = intConstants["main_field_delete_migration_version"];
		EVENT_FIELD_DELETE_MIGRATION_VERSION = intConstants["event_field_delete_migration_version"];
	}

	public void Migrate(ContentType mergeFieldType)
	{
		switch (mergeFieldType)
		{
		case ContentType.Main:
			MainFieldMigrate();
			break;
		case ContentType.Event:
			EventFieldMigrate();
			break;
		default:
			throw new Exception($"Unknown merge field type {mergeFieldType} for delete missing items migration");
		}
	}

	private void MainFieldMigrate()
	{
		MainFieldMigrationsDone = MAIN_FIELD_DELETE_MIGRATION_VERSION;
	}

	private void EventFieldMigrate()
	{
		EventFieldMigrationsDone = EVENT_FIELD_DELETE_MIGRATION_VERSION;
	}

	public string UniqueKey()
	{
		return "MigrationDeleteMissingItems";
	}

	public Memento SaveState()
	{
		return new MigrationDeleteMissingItemsMemento(this);
	}

	public void LoadState(Memento memento)
	{
		MigrationDeleteMissingItemsMemento migrationDeleteMissingItemsMemento = (MigrationDeleteMissingItemsMemento)memento;
		MainFieldMigrationsDone = migrationDeleteMissingItemsMemento.MainFieldMigrationsDone;
		EventFieldMigrationsDone = migrationDeleteMissingItemsMemento.EventFieldMigrationsDone;
	}
}
