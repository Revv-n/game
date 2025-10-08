using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;

namespace GreenT.HornyScapes.Events;

[MementoHolder]
public class EventSaveMigrationFromEventIDToCalendarId : ISavableState
{
	[Serializable]
	private class IdMemento : Memento
	{
		public IdMemento(EventSaveMigrationFromEventIDToCalendarId savableState)
			: base(savableState)
		{
		}
	}

	private readonly ISaver saver;

	public static EventSaveMigrationFromEventIDToCalendarId Instance;

	private HashSet<Event> events = new HashSet<Event>();

	public SavableStatePriority Priority => SavableStatePriority.EventSaveMigrationFromEventIDToCalendarId;

	public EventSaveMigrationFromEventIDToCalendarId(ISaver saver)
	{
		this.saver = saver;
		saver.Add(this);
		Instance = this;
	}

	public void AddTomMigration(Event targetEvent)
	{
		if (saver.TryGetMemento(targetEvent.OldEventSaveKey(), out var _))
		{
			targetEvent.SetNewSaveKey(targetEvent.OldEventSaveKey());
			events.Add(targetEvent);
		}
	}

	public string UniqueKey()
	{
		return "EventSaveMigrationFromEventIDToCalendarId";
	}

	public Memento SaveState()
	{
		if (events.Count <= 0)
		{
			return new IdMemento(this);
		}
		foreach (Event @event in events)
		{
			@event.SetNewSaveKey(@event.EventSaveKey());
			saver.DeleteHashTablePoint(@event.OldEventSaveKey());
		}
		events = events.Where((Event p) => p.IsOldEventKey()).ToHashSet();
		return new IdMemento(this);
	}

	public void LoadState(Memento memento)
	{
	}
}
