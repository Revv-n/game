using GreenT.AssetBundles;
using StripClub.Model.Cards;
using StripClub.Model.Shop.UI;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class LootboxContentBaseView : MonoView
{
	[SerializeField]
	protected Transform _smallOptionsContainer;

	[SerializeField]
	protected Transform _bigOptionsContainer;

	protected BigCardsViewManager _bigCardsViewManager;

	protected SmallCardsViewManager _smallCardsViewManager;

	protected CardsCollection _cards;

	protected FakeAssetService _fakeAssetService;

	[Inject]
	public void Init(BigCardsViewManager bigCardsViewManager, SmallCardsViewManager smallViewsManager, CardsCollection cards, FakeAssetService fakeAssetService)
	{
		_bigCardsViewManager = bigCardsViewManager;
		_smallCardsViewManager = smallViewsManager;
		_cards = cards;
		_fakeAssetService = fakeAssetService;
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
