using System;
using System.Collections.Generic;
using Merge;
using StripClub.NewEvent.Save;
using UnityEngine;

namespace StripClub.NewEvent.Data.Merge;

[Serializable]
public class EventMergeFieldData : IEventSavableState
{
	[Serializable]
	public class FieldMemento : EventMemento
	{
		public List<GIData> Data;

		public FieldMemento(EventMergeFieldData pocketRepository)
			: base(pocketRepository)
		{
			Data = pocketRepository.data;
		}
	}

	[SerializeField]
	private List<GIData> data;

	public List<GIData> GameItems
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
		}
	}

	public string UniqueKey()
	{
		return "merge_field";
	}

	public EventMergeFieldData(List<GIData> data)
	{
		this.data = data;
	}

	public EventMemento SaveState()
	{
		return new FieldMemento(this);
	}

	public void LoadState(EventMemento memento)
	{
		FieldMemento fieldMemento = (FieldMemento)memento;
		data = fieldMemento.Data;
	}
}
