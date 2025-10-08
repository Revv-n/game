using System;
using System.Linq;
using GreenT;
using GreenT.AssetBundles;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using StripClub.Model.Cards;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace StripClub.Model.Shop.UI;

public class BigCardsViewManager
{
	private CardDropView.Manager bigCardCharacterViewManager;

	private MergeItemDropCardBigView.Manager bigCardMergeItemManager;

	private ResourceDropCardBigView.Manager bigCardResourceManager;

	private CardsCollection cards;

	private FakeAssetService fakeAssetService;

	private MergeIconService iconProvider;

	private GreenT.HornyScapes.GameSettings gameSettings;

	private GameItemConfigManager gameItemConfigManager;

	[Inject]
	public void Init([Inject(Id = "CardsManager")] CardDropView.Manager bigCardCharacterViewManager, [Inject(Id = "MergeItemBigCardsManager")] MergeItemDropCardBigView.Manager bigCardMergeItemManager, [Inject(Id = "ResourceBigCardsManager")] ResourceDropCardBigView.Manager bigCardResourceManager, CardsCollection cards, FakeAssetService fakeAssetService, SmallCardsViewManager smallCardsViewManager, MergeIconService iconProvider, GreenT.HornyScapes.GameSettings gameSettings, GameItemConfigManager gameItemConfigManager)
	{
		this.bigCardCharacterViewManager = bigCardCharacterViewManager;
		this.bigCardMergeItemManager = bigCardMergeItemManager;
		this.bigCardResourceManager = bigCardResourceManager;
		this.cards = cards;
		this.fakeAssetService = fakeAssetService;
		this.iconProvider = iconProvider;
		this.gameSettings = gameSettings;
		this.gameItemConfigManager = gameItemConfigManager;
	}

	public IView Display(DropSettings drop)
	{
		RewType type = drop.Type;
		IView view = null;
		return type switch
		{
			RewType.Characters => DisplayCharacterCard(drop.Selector, drop.Quantity), 
			RewType.MergeItem => DisplayMergeItemCard(drop.Selector, drop.Quantity), 
			RewType.Resource => DisplayResourceCard(drop.Selector, drop.Quantity), 
			_ => throw new Exception("No behaviour for this type of RewType: " + type).LogException(), 
		};
	}

	public CardDropView GetView()
	{
		return bigCardCharacterViewManager.GetView();
	}

	public void HideAll()
	{
		bigCardCharacterViewManager.HideAll();
		bigCardMergeItemManager.HideAll();
	}

	private IView DisplayCharacterCard(Selector selector, int quantity)
	{
		int id = ((SelectorByID)selector).ID;
		ICharacter character2 = cards.Collection.OfType<ICharacter>().First((ICharacter x) => x.ID == id);
		CardDropView view = bigCardCharacterViewManager.GetView();
		fakeAssetService.SetFakeCharacterBankImages(character2, view.icon, (ICharacter character) => character.BankImages.Big);
		view.SetCharacter(character2, quantity, (int)character2.Rarity);
		return view;
	}

	private IView DisplayMergeItemCard(Selector selector, int? quantity)
	{
		MergeItemDropCardBigView view = bigCardMergeItemManager.GetView();
		int iD = ((SelectorByID)selector).ID;
		gameItemConfigManager.TryGetConfig(iD, out var giConfig);
		view.SetMergeItem(sprite: (giConfig != null) ? iconProvider.GetSprite(giConfig.Key) : null, source: giConfig, quantity: quantity);
		return view;
	}

	private IView DisplayResourceCard(Selector selector, int? quantity)
	{
		ResourceDropCardBigView view = bigCardResourceManager.GetView();
		CurrencySelector currencySelector = (CurrencySelector)selector;
		CurrencyType currency = currencySelector.Currency;
		Sprite alternativeSprite = gameSettings.CurrencySettings[currency, default(CompositeIdentificator)].AlternativeSprite;
		view.SetResource(alternativeSprite, quantity, currency, currencySelector.Identificator);
		return view;
	}
}
