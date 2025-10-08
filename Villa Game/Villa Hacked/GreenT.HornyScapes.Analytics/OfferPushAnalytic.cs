using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Bank;
using GreenT.Types;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class OfferPushAnalytic : OfferShowAnalyticBase
{
	private const string VIEW_SOURCE = "push";

	public OfferPushAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, GameStarter gameStarter, IDictionary<ContentType, OfferSettings.Manager> offerCluster)
		: base(amplitude, gameStarter, offerCluster)
	{
	}

	protected override void AddToTrack(OfferSettings offer)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<OfferSettings>(offer.OnPushed, (Action<OfferSettings>)SendEventByPass), (ICollection<IDisposable>)onNewStream);
	}

	protected override string GetViewSource()
	{
		return "push";
	}
}
