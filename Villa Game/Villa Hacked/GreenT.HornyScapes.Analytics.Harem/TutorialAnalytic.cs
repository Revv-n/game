using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Tutorial;
using UniRx;

namespace GreenT.HornyScapes.Analytics.Harem;

public class TutorialAnalytic : GreenT.HornyScapes.Analytics.TutorialAnalytic
{
	private IMarketingEventSender marketingEventSender;

	public TutorialAnalytic(ISaver saver, IAmplitudeSender<AmplitudeEvent> amplitude, TutorialGroupManager tutorialGroupManager, IMarketingEventSender marketingEventSender)
		: base(saver, amplitude, tutorialGroupManager)
	{
		this.marketingEventSender = marketingEventSender;
	}

	public override void Track()
	{
		base.Track();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<TutorialStep>(OnCompleteStepObservable(), (Action<TutorialStep>)SendTutorialStepEvent), (ICollection<IDisposable>)disposables);
	}

	private void SendTutorialStepEvent(TutorialStep entity)
	{
		marketingEventSender.SendTutorStepEvent(entity.StepID);
	}
}
