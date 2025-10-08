using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.NewEvent.Save;
using UniRx;

namespace StripClub.NewEvent.Data;

[MementoHolder]
public class EventDataSaver : ISavableState, IEventDataSaver, IDisposable
{
	[Serializable]
	public class EventDataMemento : Memento
	{
		public List<EventMemento> Data;

		public EventDataMemento(EventDataSaver savableState)
			: base(savableState)
		{
			Data = savableState.GetData();
		}
	}

	private readonly Dictionary<string, IEventSavableState> dictionary = new Dictionary<string, IEventSavableState>();

	private string saveKey;

	private readonly IDisposable saveKeyStream;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public EventDataSaver(IReadOnlyReactiveProperty<string> eventId)
	{
		SetSaveKey(eventId.Value);
		saveKeyStream = eventId.Subscribe(SetSaveKey);
	}

	private void SetSaveKey(string eventSaveKey)
	{
		saveKey = eventSaveKey + "_data";
	}

	public string UniqueKey()
	{
		return saveKey;
	}

	public Memento SaveState()
	{
		return new EventDataMemento(this);
	}

	public void LoadState(Memento memento)
	{
		EventDataMemento eventDataMemento = (EventDataMemento)memento;
		SetData(eventDataMemento.Data);
	}

	public void Add(IEventSavableState savableState)
	{
		if (!dictionary.ContainsKey(savableState.UniqueKey()))
		{
			dictionary.Add(savableState.UniqueKey(), savableState);
		}
	}

	private List<EventMemento> GetData()
	{
		return dictionary.Values.Select((IEventSavableState p) => p.SaveState()).ToList();
	}

	private void SetData(List<EventMemento> data)
	{
		foreach (EventMemento item in data.Where((EventMemento item) => dictionary.ContainsKey(item.UniqueKey)))
		{
			dictionary[item.UniqueKey].LoadState(item);
		}
	}

	public void Dispose()
	{
		saveKeyStream?.Dispose();
	}
}
