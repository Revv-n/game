using System;
using System.Linq;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventCardsDisplay : MonoBehaviour
{
	[SerializeField]
	private ContentType playType = ContentType.Event;

	private CardsCollection cards;

	private EventGirlCardView.Manager cardViewManager;

	private GameStarter gameStarter;

	[Inject]
	private void InnerInit(CardsCollection cards, EventGirlCardView.Manager cardViewManager, GameStarter gameStarter)
	{
		this.cards = cards;
		this.cardViewManager = cardViewManager;
		this.gameStarter = gameStarter;
	}

	protected virtual void Awake()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ICard>(Observable.Where<ICard>(cards.OnCardUnlock, (Func<ICard, bool>)((ICard _card) => _card.ContentType == ContentType.Event)), (Action<ICard>)OnUnlock), (Component)this);
	}

	protected virtual void OnEnable()
	{
		if (gameStarter.IsGameActive.Value)
		{
			Display(playType);
		}
	}

	public void Display(ContentType playType)
	{
		ICard[] array = cards.Owned.Where((ICard _card) => _card.ContentType.Equals(playType)).ToArray();
		cardViewManager.HideAll();
		ICard[] array2 = array;
		foreach (ICard card in array2)
		{
			Display(card);
		}
	}

	private void Display(ICard card)
	{
		cardViewManager.Display(card);
	}

	private void OnUnlock(ICard card)
	{
		Display(card);
	}
}
