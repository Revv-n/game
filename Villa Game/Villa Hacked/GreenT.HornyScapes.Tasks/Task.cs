using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Quest;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[MementoHolder]
public class Task : ITask, ISavableState, IDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[field: SerializeField]
		public StateType state { get; private set; }

		public Memento(Task task)
			: base(task)
		{
			state = task.State;
		}
	}

	protected Subject<Task> onUpdate = new Subject<Task>();

	private CompositeDisposable objectivesStream = new CompositeDisposable();

	private Dictionary<StateType, TaskStateBase> _states;

	private TaskStateBase _currentState;

	private readonly string uniqueKey;

	public IObservable<Task> OnUpdate => Observable.AsObservable<Task>((IObservable<Task>)onUpdate);

	public int ID { get; }

	public LinkedContent Reward { get; }

	public IGoal Goal { get; }

	public ILocker Lock { get; }

	public ContentType ContentType { get; }

	public StateType State => _currentState.State;

	public bool IsLocked => _currentState.State == StateType.Locked;

	public bool IsActive
	{
		get
		{
			if (_currentState.State != StateType.Complete)
			{
				return _currentState.State == StateType.Active;
			}
			return true;
		}
	}

	public bool ReadyToComplete => _currentState.State == StateType.Complete;

	public bool IsRewarded => _currentState.State == StateType.Rewarded;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Task(int taskId, IGoal goal, LinkedContent reward, ILocker locker, ContentType contentType, string alternativeUniqueKey = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		ID = taskId;
		Reward = reward;
		Goal = goal;
		Lock = locker;
		ContentType = contentType;
		uniqueKey = ((alternativeUniqueKey == null) ? "task." : alternativeUniqueKey) + ID;
	}

	public virtual void Initialize()
	{
		Unsubscribe();
		SelectState(StateType.Locked);
		Goal.Initialize();
		TrackObjectives();
	}

	public void SetStates(Dictionary<StateType, TaskStateBase> states)
	{
		_states = states;
		SelectState(StateType.Locked);
	}

	public virtual void SelectState(StateType stateType)
	{
		SetState(_states[stateType]);
	}

	public void Unsubscribe()
	{
		objectivesStream.Clear();
	}

	public void UpdateTask()
	{
		if (State != StateType.Rewarded && State != StateType.Locked)
		{
			bool flag = Goal.Objectives.All((IObjective _objective) => _objective.GetProgress() >= _objective.GetTarget());
			SelectState((!flag) ? StateType.Active : StateType.Complete);
		}
	}

	public void ForceUpdate()
	{
		onUpdate.OnNext(this);
	}

	private void TrackObjectives()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IObjective>(Observable.Merge<IObjective>(Goal.Objectives.Select((IObjective _objective) => _objective.OnUpdate)), (Action<IObjective>)delegate
		{
			UpdateTask();
		}), (ICollection<IDisposable>)objectivesStream);
	}

	private void SetState(TaskStateBase newState)
	{
		if (_currentState == newState)
		{
			onUpdate.OnNext(this);
			return;
		}
		if (_currentState != null)
		{
			_currentState.Exit();
		}
		_currentState = newState;
		_currentState.Enter();
		onUpdate.OnNext(this);
	}

	public void Dispose()
	{
		onUpdate?.Dispose();
		CompositeDisposable obj = objectivesStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}

	public override string ToString()
	{
		return base.ToString() + " ID: " + ID + " State: " + State;
	}

	public virtual GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public virtual void LoadState(GreenT.Data.Memento memento)
	{
		if (((Memento)memento).state == StateType.Rewarded)
		{
			_currentState = _states[StateType.Rewarded];
		}
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}
}
