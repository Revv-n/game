using System;
using System.Collections.Generic;
using GreenT.Data;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

[MementoHolder]
public class TutorialGroupSteps : ISavableState, IDisposable
{
	[Serializable]
	public class TutorGroupMemento : Memento
	{
		public bool IsComplete;

		public TutorGroupMemento(TutorialGroupSteps group)
			: base(group)
		{
			IsComplete = group.IsCompleted.Value;
		}
	}

	public readonly int GroupID;

	public readonly List<TutorialStep> Steps;

	public readonly ILocker Lockers;

	protected readonly Subject<TutorialGroupSteps> onUpdate = new Subject<TutorialGroupSteps>();

	public readonly ReactiveProperty<bool> IsStarted = new ReactiveProperty<bool>();

	public readonly ReactiveProperty<bool> IsCompleted = new ReactiveProperty<bool>();

	private readonly CompositeDisposable stepCompleteStream = new CompositeDisposable();

	private readonly string uniqueKey;

	public IObservable<TutorialGroupSteps> OnUpdate => onUpdate.AsObservable();

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public TutorialGroupSteps(int groupId, List<TutorialStep> steps, ILocker[] lockers)
	{
		GroupID = groupId;
		Steps = steps;
		Lockers = new CompositeLocker(lockers);
		uniqueKey = "TutorialGroupStep." + GroupID;
	}

	internal void Initialize()
	{
		IsStarted.Value = false;
		IsCompleted.Value = false;
		foreach (TutorialStep step in Steps)
		{
			step.Initialize();
		}
	}

	public void Play()
	{
		IsStarted.Value = true;
		stepCompleteStream.Clear();
		Steps[Steps.Count - 1].IsComplete.Where((bool _isComplete) => _isComplete).Take(1).Subscribe(delegate
		{
			SetComplete();
		})
			.AddTo(stepCompleteStream);
		TimeSpan dueTime = TimeSpan.FromSeconds(Steps[0].Data.DelayBeforeStart);
		Observable.Start(() => Steps[0], Scheduler.MainThreadIgnoreTimeScale).Delay(dueTime).Subscribe(delegate(TutorialStep _step)
		{
			_step.StartStep();
		})
			.AddTo(stepCompleteStream);
		for (int i = 0; i < Steps.Count - 1; i++)
		{
			int nextId = i + 1;
			TimeSpan dueTime2 = TimeSpan.FromSeconds(Steps[nextId].Data.DelayBeforeStart);
			Steps[i].OnUpdate.Delay(dueTime2).Subscribe(delegate
			{
				Steps[nextId].StartStep();
			}).AddTo(stepCompleteStream);
		}
		onUpdate.OnNext(this);
	}

	private void SetComplete()
	{
		stepCompleteStream.Clear();
		IsStarted.Value = false;
		IsCompleted.Value = true;
		onUpdate.OnNext(this);
	}

	public void Dispose()
	{
		stepCompleteStream?.Dispose();
	}

	public override string ToString()
	{
		string text = base.ToString();
		int groupID = GroupID;
		return text + " ID: " + groupID;
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public Memento SaveState()
	{
		return new TutorGroupMemento(this);
	}

	public void LoadState(Memento memento)
	{
		TutorGroupMemento tutorGroupMemento = (TutorGroupMemento)memento;
		IsCompleted.Value = tutorGroupMemento.IsComplete;
	}
}
