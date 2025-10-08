using System;
using System.Collections.Generic;
using StripClub.Model;
using StripClub.NewEvent.Save;
using UnityEngine;

namespace StripClub.NewEvent.Data;

public class EventDataStorage : IDisposableEventInformation, IDisposable, IEventSavableState
{
	[Serializable]
	public class EventStorageMemento : EventMemento
	{
		[field: SerializeField]
		public List<LinkedContent.Map> ContentMap { get; private set; }

		public EventStorageMemento(EventDataStorage storage)
			: base(storage)
		{
		}
	}

	public void Dispose()
	{
	}

	public string UniqueKey()
	{
		return "storage";
	}

	public EventMemento SaveState()
	{
		return new EventStorageMemento(this);
	}

	public void LoadState(EventMemento memento)
	{
	}
}
