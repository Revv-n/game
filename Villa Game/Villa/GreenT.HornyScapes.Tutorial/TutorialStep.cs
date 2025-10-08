using System;
using GreenT.Data;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.ToolTips;
using Merge;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

[MementoHolder]
public class TutorialStep : ISavableState, ITutorialModel
{
	[Serializable]
	public class TutorMemento : Memento
	{
		public bool IsComplete;

		public TutorMemento(TutorialStep step)
			: base(step)
		{
			if (step.Data.Skip)
			{
				IsComplete = step.IsComplete.Value;
			}
		}
	}

	public readonly TutorialStepSO Data;

	protected readonly ToolTipTutorialOpener toolTipTutorialOpener;

	protected readonly TutorialLightningSystem lightningSystem;

	protected Subject<TutorialStep> onUpdate = new Subject<TutorialStep>();

	private readonly string uniqueKey;

	public int GroupId => Data.GroupID;

	public int StepID => Data.StepID;

	public ReactiveProperty<bool> IsComplete { get; } = new ReactiveProperty<bool>(initialValue: false);


	public ReactiveProperty<bool> IsInProgressStep { get; } = new ReactiveProperty<bool>(initialValue: false);


	public IObservable<TutorialStep> OnUpdate => onUpdate.AsObservable();

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public TutorialStep(TutorialStepSO data, ToolTipTutorialOpener toolTipTutorialOpener, TutorialLightningSystem lightningSystem)
	{
		Data = data;
		this.toolTipTutorialOpener = toolTipTutorialOpener;
		this.lightningSystem = lightningSystem;
		uniqueKey = "TutorialStep." + StepID;
	}

	public void Initialize()
	{
		IsComplete.Value = false;
		IsInProgressStep.Value = false;
	}

	public virtual void StartStep()
	{
		if (IsComplete.Value)
		{
			FireNext();
			return;
		}
		Controller<InputController>.Instance.SetBlockedByTutorial(Data.BlockMerge);
		toolTipTutorialOpener.Init(Data);
		toolTipTutorialOpener.Open();
		IsInProgressStep.Value = true;
	}

	public virtual void SetComplete(bool state)
	{
		if (IsComplete.Value != state)
		{
			toolTipTutorialOpener.Close(Data);
			Controller<InputController>.Instance.SetBlockedByTutorial(blocked: false);
			IsInProgressStep.Value = false;
			IsComplete.Value = state;
			FireNext();
		}
	}

	private void FireNext()
	{
		onUpdate.OnNext(this);
	}

	public override string ToString()
	{
		return $"{GetType().Name} {GroupId}:{StepID}";
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public virtual Memento SaveState()
	{
		return new TutorMemento(this);
	}

	public virtual void LoadState(Memento memento)
	{
		TutorMemento tutorMemento = (TutorMemento)memento;
		if (Data.Skip)
		{
			IsComplete.Value = tutorMemento.IsComplete;
		}
	}
}
