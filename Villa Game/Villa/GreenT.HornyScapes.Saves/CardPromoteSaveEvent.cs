using System;
using StripClub.Model.Cards;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class CardPromoteSaveEvent : SaveEvent
{
	private CardsCollection cards;

	private GameStarter gameStarter;

	[Inject]
	public void Init(CardsCollection cards, GameStarter gameStarter)
	{
		this.cards = cards;
		this.gameStarter = gameStarter;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		gameStarter.IsGameActive.First((bool _isActive) => _isActive).ContinueWith(cards.Promote.Values.ToObservable()).SelectMany((IPromote _promote) => _promote.Level.Skip(1))
			.Merge(ObserveCardLevelOnUnlock())
			.Subscribe(delegate
			{
				Save();
			})
			.AddTo(saveStreams);
		IObservable<int> ObserveCardLevelOnUnlock()
		{
			return cards.OnCardUnlock.Select((ICard _card) => cards.GetPromoteOrDefault(_card)).SelectMany((IPromote _promote) => _promote.Level);
		}
	}
}
