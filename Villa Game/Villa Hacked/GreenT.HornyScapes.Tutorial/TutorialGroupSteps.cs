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

	public IObservable<TutorialGroupSteps> OnUpdate => Observable.AsObservable<TutorialGroupSteps>((IObservable<TutorialGroupSteps>)onUpdate);

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public TutorialGroupSteps(int groupId, List<TutorialStep> steps, ILocker[] lockers)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)Steps[Steps.Count - 1].IsComplete, (Func<bool, bool>)((bool _isComplete) => _isComplete)), 1), (Action<bool>)delegate
		{
			SetComplete();
		}), (ICollection<IDisposable>)stepCompleteStream);
		TimeSpan timeSpan = TimeSpan.FromSeconds(Steps[0].Data.DelayBeforeStart);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<TutorialStep>(Observable.Delay<TutorialStep>(Observable.Start<TutorialStep>((Func<TutorialStep>)(() => Steps[0]), Scheduler.MainThreadIgnoreTimeScale), timeSpan), (Action<TutorialStep>)delegate(TutorialStep _step)
		{
			_step.StartStep();
		}), (ICollection<IDisposable>)stepCompleteStream);
		for (int i = 0; i < Steps.Count - 1; i++)
		{
			int nextId = i + 1;
			TimeSpan timeSpan2 = TimeSpan.FromSeconds(Steps[nextId].Data.DelayBeforeStart);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<TutorialStep>(Observable.Delay<TutorialStep>(Steps[i].OnUpdate, timeSpan2), (Action<TutorialStep>)delegate
			{
				Steps[nextId].StartStep();
			}), (ICollection<IDisposable>)stepCompleteStream);
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
		CompositeDisposable obj = stepCompleteStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}

	public override string ToString()
	{
		return base.ToString() + " ID: " + GroupID;
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
