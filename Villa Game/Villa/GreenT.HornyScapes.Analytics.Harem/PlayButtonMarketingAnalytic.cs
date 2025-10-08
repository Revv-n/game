using System;
using UniRx;

namespace GreenT.HornyScapes.Analytics.Harem;

public class PlayButtonMarketingAnalytic : BaseAnalytic<User>
{
	private IMarketingEventSender marketingEventSender;

	private GameStarter gameStarter;

	private CompositeDisposable compositeDisposable = new CompositeDisposable();

	private readonly TimeSpan PLAY_BUTTON_EVENT_DELAY = new TimeSpan(0, 0, 2);

	public PlayButtonMarketingAnalytic(GameStarter gameStarter, IMarketingEventSender marketingEventSender, IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
		this.marketingEventSender = marketingEventSender;
		this.gameStarter = gameStarter;
	}

	public override void Track()
	{
		TrackPlayButtonEventWithDelay();
	}

	private void TrackPlayButtonEventWithDelay()
	{
		gameStarter.IsGameActive.Where((bool x) => x).Delay(PLAY_BUTTON_EVENT_DELAY).Subscribe(delegate
		{
			marketingEventSender.SendPlayButtonEvent();
		})
			.AddTo(compositeDisposable);
	}

	public override void Dispose()
	{
		base.Dispose();
		compositeDisposable?.Dispose();
	}
}
