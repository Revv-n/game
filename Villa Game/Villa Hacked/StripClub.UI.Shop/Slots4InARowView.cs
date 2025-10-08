using System;
using System.Collections.Generic;
using System.Linq;
using GreenT;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Localizations;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class Slots4InARowView : BankSectionView
{
	private const string firstPurchaseKey = "ui.shop.slots.header.double_value";

	private const string extraRewardKey = "ui.shop.slots.header.extra_reward";

	private const int maxLotsCount = 4;

	[SerializeField]
	private SlotsHeaderView headerView;

	private MiniEventsBundlesProvider bundlesProvider;

	private LocalizationService _localizationService;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	[Inject]
	public void Init(MiniEventsBundlesProvider bundlesProvider, LocalizationService localizationService)
	{
		this.bundlesProvider = bundlesProvider;
		_localizationService = localizationService;
	}

	public override void Set(BankTab settings)
	{
		base.Set(settings);
		SlotsAndBannerSettings slotSettings = (SlotsAndBannerSettings)settings.LayoutSets;
		try
		{
			SetupHeader(slotSettings);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Ошибка при отображении секции с ID:" + settings.ID);
		}
		_visibleLots.Count();
		_ = 4;
	}

	private void SetupHeader(SlotsAndBannerSettings slotSettings)
	{
		bool flag = _visibleLots.OfType<GemShopLot>().Any((GemShopLot _lot) => _lot.Stickers.HasFlag(Stickers.FirstPurchase) && _lot.Received == 0);
		bool flag2 = false;
		SlotsSectionSettings settings = base.Source.GetSettings(slotSettings.SettingsPath);
		if (settings == null)
		{
			new ArgumentNullException("settings", "В бандле shop/event_shop отсутствует пресет с именем: " + slotSettings.SettingsPath).LogException();
		}
		Sprite sprite = (flag ? settings.DoubleValue : (flag2 ? settings.ExtraReward : settings.Header));
		string key = (flag ? "ui.shop.slots.header.double_value" : (flag2 ? "ui.shop.slots.header.extra_reward" : settings.LocalizationKey));
		_localizationDisposables.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>((IObservable<string>)_localizationService.ObservableText(key), (Action<string>)delegate(string text)
		{
			headerView.Set(sprite, text, settings.TitleColor, settings.TitleColorGradient);
		}), (ICollection<IDisposable>)_localizationDisposables);
		(viewManager as LotView.LotContainerManager).Setup(settings.LotBackground);
	}

	private void OnDestroy()
	{
		_localizationDisposables.Dispose();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		_localizationDisposables.Clear();
	}
}
