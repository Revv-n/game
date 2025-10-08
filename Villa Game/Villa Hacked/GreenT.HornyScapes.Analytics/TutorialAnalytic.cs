using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Tutorial;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

[MementoHolder]
public class TutorialAnalytic : BaseEntityAnalytic<TutorialStep>, ISavableState
{
	[Serializable]
	public class TutorialAnalyticMemento : Memento
	{
		public int MaxTutorialStep;

		public bool IsPixelCreated;

		public TutorialAnalyticMemento(TutorialAnalytic analytic)
			: base(analytic)
		{
			MaxTutorialStep = analytic.maxTutorialStepId;
			IsPixelCreated = analytic.isPixelCreated;
		}
	}

	private const string ANALYTIC_EVENT = "tutorial_step";

	private List<TutorialStep> steps;

	private int maxTutorialStepId;

	protected bool isPixelCreated;

	protected TutorialStep lastTutorialStep;

	protected CompositeDisposable disposables = new CompositeDisposable();

	private TutorialGroupManager tutorialGroupManager;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public TutorialAnalytic(ISaver saver, IAmplitudeSender<AmplitudeEvent> amplitude, TutorialGroupManager tutorialGroupManager)
		: base(amplitude)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.tutorialGroupManager = tutorialGroupManager;
		base.amplitude = amplitude;
		saver.Add(this);
	}

	public override void Track()
	{
		ClearStreams();
		steps = tutorialGroupManager.Collection.SelectMany((TutorialGroupSteps _group) => _group.Steps).ToList();
		if (steps.Count != 0)
		{
			TutorialStep tutorialStep = steps.FirstOrDefault((TutorialStep _step) => _step.IsInProgressStep.Value);
			if (tutorialStep != null)
			{
				SendEventIfIsValid(tutorialStep);
			}
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<TutorialStep>(OnCompleteStepObservable(), (Action<TutorialStep>)base.SendEventIfIsValid), (ICollection<IDisposable>)disposables);
			lastTutorialStep = steps.Last();
		}
	}

	protected IObservable<TutorialStep> OnCompleteStepObservable()
	{
		return Observable.Merge<TutorialStep>(steps.Where((TutorialStep _step) => !itemsStreams.ContainsKey(_step.StepID) && !_step.IsComplete.Value).Select(EmitStepOnComplete));
	}

	private IObservable<TutorialStep> EmitStepOnComplete(TutorialStep _step)
	{
		return Observable.Take<TutorialStep>(Observable.Select<bool, TutorialStep>(Observable.Where<bool>((IObservable<bool>)_step.IsComplete, (Func<bool, bool>)((bool _value) => _value)), (Func<bool, TutorialStep>)((bool _) => _step)), 1);
	}

	protected override bool IsValid(TutorialStep entity)
	{
		return entity.StepID > maxTutorialStepId;
	}

	public override void SendEventByPass(TutorialStep entity)
	{
		SendToAmplitude(entity);
		maxTutorialStepId = entity.StepID;
	}

	private void SendToAmplitude(TutorialStep entity)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("tutorial_step");
		((AnalyticsEvent)amplitudeEvent).AddEventParams("tutorial_step", (object)entity.StepID);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent);
	}

	public override void Dispose()
	{
		base.Dispose();
		CompositeDisposable obj = disposables;
		if (obj != null)
		{
			obj.Dispose();
		}
	}

	public string UniqueKey()
	{
		return "Tutorial.Analytic";
	}

	public Memento SaveState()
	{
		return new TutorialAnalyticMemento(this);
	}

	public void LoadState(Memento memento)
	{
		TutorialAnalyticMemento tutorialAnalyticMemento = (TutorialAnalyticMemento)memento;
		maxTutorialStepId = tutorialAnalyticMemento.MaxTutorialStep;
		isPixelCreated = tutorialAnalyticMemento.IsPixelCreated;
	}
}
