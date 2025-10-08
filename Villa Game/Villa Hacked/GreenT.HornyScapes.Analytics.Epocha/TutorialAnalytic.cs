using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Tutorial;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Epocha;

public class TutorialAnalytic : GreenT.HornyScapes.Analytics.TutorialAnalytic
{
	private readonly PartnerSender _partnerSender;

	private readonly IEvent _loadEvent;

	public TutorialAnalytic(ISaver saver, IAmplitudeSender<AmplitudeEvent> amplitude, TutorialGroupManager tutorialGroupManager, [Inject(Id = "Tutorial")] IEvent loadEvent, PartnerSender partnerSender)
		: base(saver, amplitude, tutorialGroupManager)
	{
		_loadEvent = loadEvent;
		_partnerSender = partnerSender;
	}

	public override void Track()
	{
		base.Track();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<TutorialStep>(OnCompleteStepObservable(), (Action<TutorialStep>)SendPixel), (ICollection<IDisposable>)disposables);
	}

	public override void SendEventByPass(TutorialStep entity)
	{
		SendToPartner(entity);
		base.SendEventByPass(entity);
	}

	private void SendPixel(TutorialStep entity)
	{
		if (lastTutorialStep == entity && !isPixelCreated)
		{
			_loadEvent.Send();
			isPixelCreated = true;
		}
	}

	private void SendToPartner(TutorialStep entity)
	{
		_partnerSender.AddEvent(new TutorialStepPartnerEvent(entity.StepID));
	}
}
