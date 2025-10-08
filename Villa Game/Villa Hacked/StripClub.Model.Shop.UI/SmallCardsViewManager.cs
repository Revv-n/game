using System;
using System.Collections.Generic;
using System.Linq;
using GreenT;
using GreenT.AssetBundles;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.UI;
using GreenT.Types;
using StripClub.Model.Cards;
using StripClub.UI;

namespace StripClub.Model.Shop.UI;

public class SmallCardsViewManager
{
	private readonly CardDropView.Manager cardOptionViewManager;

	private readonly BoosterDropView.Manager _boosterOptionViewManager;

	private readonly MergeItemDropView.Manager itemOptionViewManager;

	private readonly CurrencyDropView.Manager currencyOptionViewManager;

	private readonly SkinDropView.Manager skinOptionViewManager;

	private readonly DecorationDropView.Manager decorationOptionViewManager;

	private readonly DecorationDropViewWithRarity.Manager decorationOptionViewWithRarityManager;

	private readonly LootboxDropView.Manager lootboxOptionViewManager;

	private readonly PresentDropView.Manager presentOptionViewManager;

	private readonly FakeAssetService _fakeAssetService;

	private readonly LevelDropView.Manager _levelManager;

	private readonly SkinManager skinManager;

	private readonly CardsCollection cards;

	private readonly MergeIconService _iconProvider;

	private readonly IEnumerable<IView> visibleViews;

	private readonly DecorationManager decorationManager;

	private readonly BoosterMapperManager _boosterMapperManager;

	private readonly RoomManager house;

	private readonly LootboxCollection lootboxCollection;

	private readonly GameItemConfigManager _gameItemConfigManager;

	public IEnumerable<IView> VisibleViews => visibleViews;

	public SmallCardsViewManager(MergeItemDropView.Manager itemOptionViewManager, CurrencyDropView.Manager currencyOptionViewManager, BoosterDropView.Manager boosterOptionViewManager, CardDropView.Manager cardOptionViewManager, CardsCollection cards, MergeIconService iconProvider, SkinDropView.Manager skinOptionViewManager, SkinManager skinManager, DecorationManager decorationManager, RoomManager house, DecorationDropView.Manager decorationOptionViewManager, FakeAssetService fakeAssetService, DecorationDropViewWithRarity.Manager decorationOptionViewWithRarityManager, LevelDropView.Manager levelManager, BoosterMapperManager boosterMapperManager, LootboxDropView.Manager lootboxOptionViewManager, LootboxCollection lootboxCollection, GameItemConfigManager gameItemConfigManager, PresentDropView.Manager presentOptionViewManager)
	{
		_boosterOptionViewManager = boosterOptionViewManager;
		this.cardOptionViewManager = cardOptionViewManager;
		this.itemOptionViewManager = itemOptionViewManager;
		this.currencyOptionViewManager = currencyOptionViewManager;
		this.skinOptionViewManager = skinOptionViewManager;
		this.skinManager = skinManager;
		this.cards = cards;
		_iconProvider = iconProvider;
		this.decorationManager = decorationManager;
		this.house = house;
		this.decorationOptionViewManager = decorationOptionViewManager;
		this.decorationOptionViewWithRarityManager = decorationOptionViewWithRarityManager;
		this.lootboxOptionViewManager = lootboxOptionViewManager;
		this.presentOptionViewManager = presentOptionViewManager;
		this.lootboxCollection = lootboxCollection;
		_fakeAssetService = fakeAssetService;
		_levelManager = levelManager;
		_boosterMapperManager = boosterMapperManager;
		_gameItemConfigManager = gameItemConfigManager;
		visibleViews = cardOptionViewManager.VisibleViews.OfType<IView>().Concat(itemOptionViewManager.VisibleViews).Concat(currencyOptionViewManager.VisibleViews);
	}

	public IView Display(DropSettings source)
	{
		return Display(source.Type, source.Selector, null, source.Quantity);
	}

	public IView Display(Reward reward, int rarity)
	{
		return Display(reward.Type, reward.GetSelector(), rarity);
	}

	public IView Display(RewType type, Selector selector, int? rarity = null, int? quantity = null)
	{
		switch (type)
		{
		case RewType.Characters:
			return GetCharacterView(selector, rarity, quantity);
		case RewType.MergeItem:
			return GetMergeItemView(selector, quantity);
		case RewType.Resource:
			if (IsPresent(selector))
			{
				return GetPresentView(selector, quantity);
			}
			return GetResourceView(selector, quantity);
		case RewType.Level:
			return GetLevel(quantity);
		case RewType.Skin:
			return GetSkinView(selector, rarity);
		case RewType.Decorations:
			return GetDecorationView(selector, rarity);
		case RewType.Booster:
			return GetBooster(selector);
		case RewType.Lootbox:
			return GetLootboxView(selector, rarity);
		default:
			throw new Exception("No behaviour for this type of RewType: " + type).LogException();
		}
	}

	private IView GetLevel(int? quantity)
	{
		LevelDropView view = _levelManager.GetView();
		view.Set();
		return view;
	}

	private IView GetBooster(Selector selector)
	{
		BoosterDropView view = _boosterOptionViewManager.GetView();
		int id = ((SelectorByID)selector).ID;
		BoosterMapper boosterMapper = _boosterMapperManager.Collection.First((BoosterMapper x) => x.booster_id == id);
		view.Set(boosterMapper.bonus_type, boosterMapper.bonus_value);
		return view;
	}

	private bool IsPresent(Selector selector)
	{
		if (!(selector is CurrencySelector { Currency: var currency }))
		{
			return false;
		}
		if (currency != CurrencyType.Present1 && currency != CurrencyType.Present2 && currency != CurrencyType.Present3)
		{
			return currency == CurrencyType.Present4;
		}
		return true;
	}

	public void HideAll()
	{
		cardOptionViewManager.HideAll();
		itemOptionViewManager.HideAll();
		currencyOptionViewManager.HideAll();
		skinOptionViewManager.HideAll();
		decorationOptionViewManager.HideAll();
		decorationOptionViewWithRarityManager.HideAll();
		lootboxOptionViewManager.HideAll();
		presentOptionViewManager?.HideAll();
		_levelManager.HideAll();
	}

	private IView GetCharacterView(Selector selector, int? rarity, int? quantity)
	{
		CardDropView view = cardOptionViewManager.GetView();
		int id = ((SelectorByID)selector).ID;
		ICharacter character2 = cards.Collection.OfType<ICharacter>().First((ICharacter x) => x.ID == id);
		int rarity2 = ((!rarity.HasValue) ? ((int)character2.Rarity) : rarity.Value);
		_fakeAssetService.SetFakeCharacterBankImages(character2, view.icon, (ICharacter character) => character.BankImages.Small);
		view.SetCharacter(character2, quantity, rarity2);
		return view;
	}

	private IView GetMergeItemView(Selector selector, int? quantity)
	{
		MergeItemDropView view = itemOptionViewManager.GetView();
		int iD = ((SelectorByID)selector).ID;
		_gameItemConfigManager.TryGetConfig(iD, out var giConfig);
		view.Set(itemIcon: (giConfig != null) ? _iconProvider.GetSprite(giConfig.Key) : null, source: giConfig, quantity: quantity);
		return view;
	}

	private IView GetResourceView(Selector selector, int? quantity)
	{
		CurrencyDropView view = currencyOptionViewManager.GetView();
		CurrencyType currency = ((CurrencySelector)selector).Currency;
		CompositeIdentificator identificator = ((CurrencySelector)selector).Identificator;
		view.Set(currency, quantity, identificator);
		return view;
	}

	private IView GetSkinView(Selector selector, int? rarity)
	{
		int iD = ((SelectorByID)selector).ID;
		Skin skin2 = skinManager.Get(iD);
		SkinDropView view = skinOptionViewManager.GetView();
		_fakeAssetService.SetFakeSkinIcon(skin2, view.icon, (Skin skin) => skin.Data.Icon);
		view.Set(skin2, rarity);
		return view;
	}

	private IView GetLootboxView(Selector selector, int? rarity)
	{
		int iD = ((SelectorByID)selector).ID;
		Lootbox lootbox = lootboxCollection.Get(iD);
		LootboxDropView view = lootboxOptionViewManager.GetView();
		view.Set(lootbox, rarity);
		return view;
	}

	private IView GetDecorationView(Selector selector, int? rarity)
	{
		int iD = ((SelectorByID)selector).ID;
		Decoration item = decorationManager.GetItem(iD);
		BaseObjectConfig config = house.GetObject(item.HouseObjectID).Config;
		if (!rarity.HasValue)
		{
			DecorationDropView view = decorationOptionViewManager.GetView();
			view.Set(config);
			return view;
		}
		DecorationDropViewWithRarity view2 = decorationOptionViewWithRarityManager.GetView();
		view2.Set(config, rarity.Value);
		return view2;
	}

	private IView GetPresentView(Selector selector, int? quantity)
	{
		PresentDropView obj = presentOptionViewManager?.GetView();
		CurrencySelector selector2 = selector as CurrencySelector;
		obj.Set(selector2, quantity.Value);
		return obj;
	}
}
