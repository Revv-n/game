using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes.Tasks;

public class Goal : IGoal, IDisposable
{
	protected Subject<IGoal> onUpdate = new Subject<IGoal>();

	private IDisposable disposable;

	public IObservable<IGoal> OnUpdate => onUpdate.AsObservable();

	public string Description { get; }

	public IObjective[] Objectives { get; }

	public ActionButtonType ActionButtonType { get; }

	public bool IsComplete { get; protected set; }

	public Goal(string description, ActionButtonType actionButtonType, IObjective[] objectives)
	{
		Description = description ?? string.Empty;
		ActionButtonType = actionButtonType;
		Objectives = objectives;
	}

	public void Initialize()
	{
		disposable?.Dispose();
		IObjective[] objectives = Objectives;
		for (int i = 0; i < objectives.Length; i++)
		{
			objectives[i].Initialize();
		}
		TrackObjective();
	}

	private void TrackObjective()
	{
		disposable = (from _subTasks in Objectives.Select((IObjective _subTask) => _subTask.OnUpdate).CombineLatest()
			select _subTasks.All((IObjective _subTask) => _subTask.IsComplete)).Subscribe(delegate(bool _complete)
		{
			IsComplete = _complete;
			onUpdate.OnNext(this);
		});
	}

	public void Dispose()
	{
		onUpdate?.Dispose();
		disposable?.Dispose();
	}
}
