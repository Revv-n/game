using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.marketingEventSender = marketingEventSender;
		this.gameStarter = gameStarter;
	}

	public override void Track()
	{
		TrackPlayButtonEventWithDelay();
	}

	private void TrackPlayButtonEventWithDelay()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Delay<bool>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), PLAY_BUTTON_EVENT_DELAY), (Action<bool>)delegate
		{
			marketingEventSender.SendPlayButtonEvent();
		}), (ICollection<IDisposable>)compositeDisposable);
	}

	public override void Dispose()
	{
		base.Dispose();
		CompositeDisposable obj = compositeDisposable;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
