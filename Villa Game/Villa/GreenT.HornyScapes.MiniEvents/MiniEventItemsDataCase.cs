using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.Types;
using Merge;

namespace GreenT.HornyScapes.MiniEvents;

[MementoHolder]
public abstract class MiniEventItemsDataCase : ISavableState
{
	[Serializable]
	public class MiniEventItemDataCaseMemento : Memento
	{
		public Dictionary<CompositeIdentificator, List<GIData>> SavedItems { get; }

		public MiniEventItemDataCaseMemento(MiniEventItemsDataCase dataCase)
			: base(dataCase)
		{
			SavedItems = dataCase._items;
		}
	}

	protected string _saveKey = string.Empty;

	protected Dictionary<CompositeIdentificator, List<GIData>> _items = new Dictionary<CompositeIdentificator, List<GIData>>();

	public SavableStatePriority Priority { get; } = SavableStatePriority.Base;


	public void AddItem(GIData item, CompositeIdentificator id)
	{
		if (!_items.ContainsKey(id))
		{
			_items.Add(id, new List<GIData>());
		}
		_items[id].Add(item);
	}

	public void RemoveItem(GIData item, CompositeIdentificator id)
	{
		if (_items.TryGetValue(id, out var value) && value.Contains(item))
		{
			value.Remove(item);
		}
	}

	public abstract void LoadItems(CompositeIdentificator id);

	public void Reset(CompositeIdentificator identificator)
	{
		if (_items.ContainsKey(identificator))
		{
			_items[identificator].Clear();
		}
	}

	public string UniqueKey()
	{
		return _saveKey;
	}

	public Memento SaveState()
	{
		return new MiniEventItemDataCaseMemento(this);
	}

	public void LoadState(Memento memento)
	{
		_ = (MiniEventItemDataCaseMemento)memento;
	}
}
