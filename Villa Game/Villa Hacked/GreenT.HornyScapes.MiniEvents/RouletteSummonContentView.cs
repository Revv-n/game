using StripClub.Model.Cards;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class RouletteSummonContentView : MonoView
{
	[SerializeField]
	protected Transform _smallOptionsContainer;

	[SerializeField]
	protected Transform _bigOptionsContainer;

	protected SummonRouletteCardDropView.Manager _bigCardsViewManager;

	protected SummonRouletteCardDropView.Manager _smallCardsViewManager;

	protected CardsCollection _cards;

	[Inject]
	public void Init([Inject(Id = "BidCardsManager")] SummonRouletteCardDropView.Manager bigCardsViewManager, [Inject(Id = "SmallCardsManager")] SummonRouletteCardDropView.Manager smallCardsViewManager, CardsCollection cards)
	{
		_bigCardsViewManager = bigCardsViewManager;
		_smallCardsViewManager = smallCardsViewManager;
		_cards = cards;
	}

	protected void HideAll()
	{
		_bigCardsViewManager.HideAll();
		_smallCardsViewManager.HideAll();
	}

	protected void TryShow(GameObject gameObject, bool isShow)
	{
		gameObject.SetActive(isShow);
	}
}
