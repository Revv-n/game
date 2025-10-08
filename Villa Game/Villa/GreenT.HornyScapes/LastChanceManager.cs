using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

[Serializable]
[MementoHolder]
public sealed class LastChanceManager : SimpleManager<LastChance>, ISavableState
{
	[Serializable]
	public class LastChanceManagerMemento : Memento
	{
		public List<LastChance> Collection;

		public LastChanceManagerMemento(LastChanceManager lastChanceManager)
			: base(lastChanceManager)
		{
			Save(lastChanceManager);
		}

		public Memento Save(LastChanceManager lastChanceManager)
		{
			Collection = lastChanceManager.collection;
			return this;
		}
	}

	private const string UNIQUE_KEY_PREFIX = "lastchance.manager";

	public List<LastChance> ListCollection => collection;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public override void Add(LastChance entity)
	{
		LastChance lastChance2 = collection.FirstOrDefault((LastChance lastChance) => lastChance.EventId == entity.EventId && lastChance.LastChanceType == entity.LastChanceType);
		if (lastChance2 != null)
		{
			lastChance2.UpdateDates(entity.StartDate, entity.EndDate);
		}
		else
		{
			base.Add(entity);
		}
	}

	public bool Remove(LastChance lastChance)
	{
		return collection.Remove(lastChance);
	}

	public string UniqueKey()
	{
		return "lastchance.manager";
	}

	public Memento SaveState()
	{
		return new LastChanceManagerMemento(this);
	}

	public void LoadState(Memento memento)
	{
		LastChanceManagerMemento lastChanceManagerMemento = (LastChanceManagerMemento)memento;
		AddRange(lastChanceManagerMemento.Collection);
	}
}
