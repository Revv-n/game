using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Bank;
using GreenT.Types;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public abstract class OfferShowAnalyticBase : BaseEntityAnalytic<OfferSettings>
{
	private readonly GameStarter gameStarter;

	private readonly IDictionary<ContentType, OfferSettings.Manager> offerCluster;

	protected OfferShowAnalyticBase(IAmplitudeSender<AmplitudeEvent> amplitude, GameStarter gameStarter, IDictionary<ContentType, OfferSettings.Manager> offerCluster)
		: base(amplitude)
	{
		this.gameStarter = gameStarter;
		this.offerCluster = offerCluster;
	}

	public override void Track()
	{
		ClearStreams();
		OnGameStart().Subscribe(delegate
		{
			TrackOffers();
		}).AddTo(onNewStream);
		IObservable<bool> OnGameStart()
		{
			return gameStarter.IsGameActive.Where((bool x) => x);
		}
	}

	public override void SendEventByPass(OfferSettings tuple)
	{
		amplitude.AddEvent(new ShowOfferAmplitudeEvent(tuple, GetViewSource()));
	}

	private void TrackOffers()
	{
		SubscribeOnClickOffer();
	}

	private void SubscribeOnClickOffer()
	{
		foreach (ContentType value in Enum.GetValues(typeof(ContentType)))
		{
			foreach (OfferSettings item in offerCluster[value].Collection)
			{
				AddToTrack(item);
			}
		}
	}

	protected abstract void AddToTrack(OfferSettings offer);

	protected abstract string GetViewSource();
}
