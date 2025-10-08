using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Merge<int>(Observable.SelectMany<IPromote, int>(Observable.ContinueWith<bool, IPromote>(Observable.First<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool _isActive) => _isActive)), Observable.ToObservable<IPromote>((IEnumerable<IPromote>)cards.Promote.Values)), (Func<IPromote, IObservable<int>>)((IPromote _promote) => Observable.Skip<int>((IObservable<int>)_promote.Level, 1))), new IObservable<int>[1] { ObserveCardLevelOnUnlock() }), (Action<int>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
		IObservable<int> ObserveCardLevelOnUnlock()
		{
			return Observable.SelectMany<IPromote, int>(Observable.Select<ICard, IPromote>(cards.OnCardUnlock, (Func<ICard, IPromote>)((ICard _card) => cards.GetPromoteOrDefault(_card))), (Func<IPromote, IObservable<int>>)((IPromote _promote) => (IObservable<int>)_promote.Level));
		}
	}
}
