using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.RoomObjects;
using Merge.Meta.RoomObjects;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.StarShop.UI;

public class StarShopView : MonoView<IStarShopItem>
{
	public class Factory : PlaceholderFactory<StarShopView>
	{
	}

	private const string keyLocalization = "ui.starshop.";

	[SerializeField]
	private Button start;

	[SerializeField]
	private Button complete;

	[SerializeField]
	private CompletableView completeElements;

	[SerializeField]
	private LocalizedTextMeshPro title;

	[SerializeField]
	private TextMeshProUGUI[] starPrices;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image lootboxIcon;

	[SerializeField]
	private List<CurrencySpriteAttacher> currencySpriteAttachers;

	protected Subject<IStarShopItem> onSet = new Subject<IStarShopItem>();

	private CompositeDisposable eventStream = new CompositeDisposable();

	private IDisposable completeBtn;

	private StarShopController starShopController;

	private GameSettings gameSettings;

	private LootboxCollection lootboxCollection;

	private RoomManager house;

	private ICurrencyProcessor currencyProcessor;

	public IObservable<IStarShopItem> OnSet => onSet.AsObservable();

	[Inject]
	private void InnerInit(StarShopController starShopController, LootboxCollection lootboxCollection, GameSettings gameSettings, RoomManager house, ICurrencyProcessor currencyProcessor)
	{
		this.starShopController = starShopController;
		this.lootboxCollection = lootboxCollection;
		this.gameSettings = gameSettings;
		this.house = house;
		this.currencyProcessor = currencyProcessor;
	}

	public override void Set(IStarShopItem item)
	{
		base.Set(item);
		SetView(item);
		SubscribeCompleteBtn();
		ListenEvent(item);
		onSet.OnNext(item);
	}

	private void SetView(IStarShopItem item)
	{
		IGameRoomObject<BaseObjectConfig> roomObject = house.GetRoomObject(item.HouseObjectID);
		SetAvatar(roomObject);
		LocalizeTitle();
		SetLootboxIcon(item);
		SetPriceOnButton(item);
		SetCurrencyOnButton(item);
		UpdateButtonState(item);
	}

	private void SetPriceOnButton(IStarShopItem item)
	{
		TextMeshProUGUI[] array = starPrices;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = item.Cost.Amount.ToString();
		}
	}

	private void SetCurrencyOnButton(IStarShopItem item)
	{
		for (int i = 0; i < currencySpriteAttachers.Count; i++)
		{
			currencySpriteAttachers[i].ChangeCurrency(item.Cost.Currency);
		}
	}

	private void SubscribeCompleteBtn()
	{
		completeBtn?.Dispose();
		completeBtn = complete.OnClickAsObservable().Subscribe(delegate
		{
			SetComplete();
		});
	}

	private void ListenEvent(IStarShopItem item)
	{
		eventStream.Clear();
		item.OnUpdate.Where((StarShopItem _item) => _item.State == EntityStatus.InProgress || _item.State == EntityStatus.Complete).Subscribe(delegate(StarShopItem _item)
		{
			UpdateButtonState(_item);
		}).AddTo(eventStream);
		item.OnUpdate.Where((StarShopItem _item) => _item.State == EntityStatus.Rewarded).Subscribe(delegate
		{
			Display(display: false);
		}).AddTo(eventStream);
	}

	private void UpdateButtonState(IStarShopItem item)
	{
		bool flag = item.State == EntityStatus.Complete || (item.State == EntityStatus.InProgress && item.Cost.Amount <= currencyProcessor.GetCount(item.Cost.Currency));
		completeElements.SetComplete(flag);
		start.gameObject.SetActive(!flag);
		complete.gameObject.SetActive(flag);
	}

	private void SetLootboxIcon(IStarShopItem item)
	{
		Rarity rarity = lootboxCollection.Get(item.LootboxIdReward).Rarity;
		lootboxIcon.sprite = gameSettings.LootBoxSprites[rarity];
	}

	private void SetAvatar(IGameRoomObject<BaseObjectConfig> roomObject)
	{
		icon.sprite = roomObject.Config.TaskIcon;
	}

	private void LocalizeTitle()
	{
		title.Init("ui.starshop." + base.Source.ID);
	}

	private void SetComplete()
	{
		starShopController.TrySetRewarded(base.Source);
	}

	protected virtual void OnDisable()
	{
		eventStream.Clear();
	}

	protected virtual void OnDestroy()
	{
		eventStream.Dispose();
	}
}
