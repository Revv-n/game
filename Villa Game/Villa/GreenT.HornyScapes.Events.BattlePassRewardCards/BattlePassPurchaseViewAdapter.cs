using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Monetization;
using GreenT.Localizations;
using GreenT.UI;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassPurchaseViewAdapter : MonoView
{
	[SerializeField]
	private LocalizedTextMeshPro _titleField;

	[SerializeField]
	private LocalizedTextMeshPro _titleShadow;

	[SerializeField]
	private Image _background;

	[SerializeField]
	private Image _headerImage;

	[SerializeField]
	private List<BattlePassLotAdapter> _lotAdapters;

	private LotManager _lotManager;

	private Purchaser _purchaser;

	private LocalizationService _localizationService;

	private WindowOpener _windowOpener;

	private ContentStorageProvider _storageProvider;

	private BattlePass _battlePass;

	private IDisposable _purchaseRequestStream;

	private readonly CompositeDisposable _onPurchaseStream = new CompositeDisposable();

	public void Init(LotManager lotManager, Purchaser purchaser, LocalizationService localizationService, WindowOpener windowOpener, ContentStorageProvider storageProvider, BattlePass battlePass, int[] lots)
	{
		_lotManager = lotManager;
		_purchaser = purchaser;
		_localizationService = localizationService;
		_windowOpener = windowOpener;
		_storageProvider = storageProvider;
		_battlePass = battlePass;
		int num = lots.Length;
		if (_lotAdapters.Count < num)
		{
			throw new ArgumentException($"Wrong reward amount. Currently have {num}, but required {_lotAdapters.Count}");
		}
		BattlePassBundleData bundle = _battlePass.Bundle;
		if (_titleField != null)
		{
			_titleField.Init(bundle.TitleKeyLoc);
		}
		if (_titleShadow != null)
		{
			_titleShadow.Init(bundle.TitleKeyLoc);
		}
		bool flag = num == 1;
		_background.sprite = (flag ? bundle.SinglePurchaseWindow : battlePass.CurrentViewSettings.PurchaseWindow);
		if (_headerImage != null)
		{
			_headerImage.sprite = battlePass.CurrentViewSettings.AnnouncementTitleBackground;
			_headerImage.enabled = !flag;
		}
		if (!_battlePass.Data.StartData.PremiumPurchasedProperty.Value)
		{
			_battlePass.Data.StartData.SetPurchaseStarted(value: false);
			_purchaseRequestStream?.Dispose();
			_purchaseRequestStream = SetupAdapters(lots).Subscribe(Purchase);
		}
	}

	private IObservable<BundleLot> SetupAdapters(int[] lots)
	{
		IObservable<BundleLot> observable = Observable.Empty<BundleLot>();
		int num = 0;
		foreach (int target in lots)
		{
			if (num > _lotAdapters.Count)
			{
				return observable;
			}
			BattlePassLotAdapter adapter = _lotAdapters[num];
			Lot lot2 = _lotManager.Collection.FirstOrDefault((Lot lot) => lot.ID == target);
			Sprite reward = ((num == 0) ? _battlePass.Bundle.LeftReward : _battlePass.CurrentViewSettings.RightReward);
			if (lot2 == null)
			{
				return observable;
			}
			adapter.Set(lot2 as BundleLot);
			adapter.SetRewardSprite(reward, _battlePass.Bundle.LevelBonusHolder);
			_localizationService.ObservableText(_battlePass.Bundle.PremiumLevelBonus).Take(1).Subscribe(delegate(string value)
			{
				adapter.SetLocalization(value);
			});
			observable = observable.Merge(adapter.OnPurchase());
			num++;
		}
		return observable;
	}

	private void Purchase(BundleLot bundleLot)
	{
		_onPurchaseStream.Clear();
		string nameForPopup = GetNameForPopup(bundleLot);
		string descriptionForPopup = GetDescriptionForPopup(bundleLot);
		_battlePass.Data.StartData.SetPurchaseStarted(value: true);
		_purchaser.OnResult.Subscribe(delegate(bool value)
		{
			OnPurchaseEnded(value, bundleLot);
		}).AddTo(_onPurchaseStream);
		_purchaser.TryPurchase(bundleLot, bundleLot.PaymentID, nameForPopup, descriptionForPopup, bundleLot.Data.ImageNameKey);
	}

	private void OnPurchaseEnded(bool success, BundleLot bundleLot)
	{
		_onPurchaseStream.Clear();
		_battlePass.Data.StartData.SetPurchaseStarted(value: false);
		if (!success)
		{
			return;
		}
		_storageProvider.AddToPlayer();
		_purchaseRequestStream?.Dispose();
		_windowOpener.Close();
		foreach (BattlePassLotAdapter lotAdapter in _lotAdapters)
		{
			if (lotAdapter.Source == bundleLot)
			{
				lotAdapter.OnPurchaseEnded(bundleLot);
			}
		}
		_battlePass.Data.StartData.SetPurchaseComplete(value: true);
	}

	private string GetDescriptionForPopup(BundleLot bundleLot)
	{
		_ = string.Empty;
		return GetLocalization(bundleLot.Data.ItemDescriptionKey);
	}

	private string GetNameForPopup(BundleLot bundleLot)
	{
		return string.Empty;
	}

	private string GetLocalization(string key)
	{
		return _localizationService.Text(key);
	}

	private void OnDestroy()
	{
		_purchaseRequestStream?.Dispose();
		_onPurchaseStream.Dispose();
	}
}
