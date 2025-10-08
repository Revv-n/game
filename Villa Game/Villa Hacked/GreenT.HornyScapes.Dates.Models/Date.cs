using System;
using System.Collections.Generic;
using GreenT.Data;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.Dates.Models;

public class Date : ISavableState
{
	[Serializable]
	public class DateMemento : Memento
	{
		public EntityStatus State;

		public DateMemento(Date date)
			: base(date)
		{
			State = date.State.Value;
		}
	}

	private const string SaveKey = "date.{0}";

	public int ID { get; }

	public int DateNumber { get; private set; }

	public Queue<DatePhrase> Steps { get; } = new Queue<DatePhrase>();


	public ReactiveProperty<EntityStatus> State { get; } = new ReactiveProperty<EntityStatus>(EntityStatus.Blocked);


	public DateIconData IconData { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Date(int id, IEnumerable<DatePhrase> steps)
	{
		ID = id;
		AddSteps(steps);
	}

	public void SetDateNumber(int dateNumber)
	{
		DateNumber = dateNumber;
	}

	public void SetState(EntityStatus status)
	{
		if (State.Value != EntityStatus.Rewarded)
		{
			State.Value = status;
		}
	}

	public void SetBundleData(DateIconData iconData)
	{
		IconData = iconData;
	}

	private void AddSteps(IEnumerable<DatePhrase> steps)
	{
		foreach (DatePhrase step in steps)
		{
			AddStep(step);
		}
	}

	private void AddStep(DatePhrase phrase)
	{
		Steps.Enqueue(phrase);
	}

	public string UniqueKey()
	{
		return $"date.{ID}";
	}

	public Memento SaveState()
	{
		return new DateMemento(this);
	}

	public void LoadState(Memento memento)
	{
		DateMemento dateMemento = (DateMemento)memento;
		State.Value = dateMemento.State;
	}
}
