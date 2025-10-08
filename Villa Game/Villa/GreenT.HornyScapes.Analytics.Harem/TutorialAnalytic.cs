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
		OnCompleteStepObservable().Subscribe(SendTutorialStepEvent).AddTo(disposables);
	}

	private void SendTutorialStepEvent(TutorialStep entity)
	{
		marketingEventSender.SendTutorStepEvent(entity.StepID);
	}
}
