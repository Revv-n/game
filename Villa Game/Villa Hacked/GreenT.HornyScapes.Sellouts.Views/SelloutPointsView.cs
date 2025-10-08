using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Analytics;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Providers;
using GreenT.HornyScapes.Sellouts.Services;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub.Model.Shop;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Views;

public sealed class SelloutPointsView : MonoView
{
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TMP_Text _points;

	private SelloutStateManager _selloutStateManager;

	private SelloutMapperProvider _selloutMapperProvider;

	private SelloutRewardsMapperProvider _selloutRewardsMapperProvider;

	private SelloutAnalytic _selloutAnalytic;

	private LocalizedPriceService _localizedPriceService;

	private CalendarManager _calendarManager;

	private Sellout _sellout;

	private CalendarModel _calendarModel;

	private ValuableLot<decimal> _bundleLot;

	private IDisposable _calendarStateStream;

	private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

	[Inject]
	private void Init(SelloutStateManager selloutStateManager, SelloutMapperProvider selloutMapperProvider, SelloutRewardsMapperProvider selloutRewardsMapperProvider, SelloutAnalytic selloutAnalytic, LocalizedPriceService localizedPriceService, CalendarManager calendarManager)
	{
		_selloutStateManager = selloutStateManager;
		_selloutMapperProvider = selloutMapperProvider;
		_selloutRewardsMapperProvider = selloutRewardsMapperProvider;
		_selloutAnalytic = selloutAnalytic;
		_localizedPriceService = localizedPriceService;
		_calendarManager = calendarManager;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Sellout>(_selloutStateManager.Activated, (Action<Sellout>)delegate(Sellout sellout)
		{
			Set(sellout);
		}), (ICollection<IDisposable>)_subscriptions);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Sellout>(_selloutStateManager.Deactivated, (Action<Sellout>)delegate
		{
			Deactivate();
		}), (ICollection<IDisposable>)_subscriptions);
	}

	public void SetLot(ValuableLot<decimal> bundleLot)
	{
		_bundleLot = bundleLot;
		UpdatePoints();
		CheckActivate();
	}

	public void CheckSellout()
	{
		Sellout activeSellout = _selloutStateManager.GetActiveSellout();
		if (activeSellout != null)
		{
			Set(activeSellout);
		}
	}

	public void OnPurchase(CurrencyAmplitudeAnalytic.SourceType sourceType, ContentType contentType)
	{
		if (_sellout != null)
		{
			int selloutPoints = _localizedPriceService.GetSelloutPoints(_bundleLot);
			if (_bundleLot.IsReal)
			{
				_sellout.AddPoints(selloutPoints);
				_selloutAnalytic.SendPointsReceivedEvent(selloutPoints, sourceType, contentType);
				CheckPoints();
			}
		}
	}

	private void OnDestroy()
	{
		_subscriptions.Dispose();
	}

	private void Set(Sellout sellout)
	{
		_sellout = sellout;
		_icon.sprite = sellout.BundleData.CurrencySprite;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)_sellout.CanPointsTrack, (Func<bool, bool>)((bool result) => !result)), (Action<bool>)delegate
		{
			Deactivate();
		}), (ICollection<IDisposable>)_subscriptions);
		UpdatePoints();
		CheckActivate();
		TrackCalendarState();
	}

	private void UpdatePoints()
	{
		int selloutPoints = _localizedPriceService.GetSelloutPoints(_bundleLot);
		if (_bundleLot != null && _bundleLot.IsReal)
		{
			_points.text = $"+{selloutPoints}";
		}
	}

	private void CheckPoints()
	{
		if (_sellout == null)
		{
			Deactivate();
			return;
		}
		int value = _sellout.Points.Value;
		int[] rewards_id = _selloutMapperProvider.Get(_sellout.ID).rewards_id;
		int id = rewards_id[rewards_id.Length - 1];
		if (_selloutRewardsMapperProvider.Get(id).points_price <= value)
		{
			_sellout.StopTrackPoints();
		}
	}

	private void TrackCalendarState()
	{
		if (_sellout == null)
		{
			return;
		}
		_calendarModel = _calendarManager.Collection.FirstOrDefault((CalendarModel calendarModel) => calendarModel.EventType == EventStructureType.Sellout && calendarModel.BalanceId == _sellout.ID);
		if (_calendarModel != null)
		{
			_calendarStateStream?.Dispose();
			_calendarStateStream = ObservableExtensions.Subscribe<EntityStatus>((IObservable<EntityStatus>)_calendarModel.CalendarState, (Action<EntityStatus>)delegate
			{
				CheckActivate();
			});
		}
	}

	private void CheckActivate()
	{
		bool active = _sellout != null && _sellout.CanPointsTrack.Value && _calendarModel != null && _calendarModel.CalendarState.Value != EntityStatus.Blocked && _bundleLot != null && _bundleLot.IsReal;
		base.gameObject.SetActive(active);
	}

	private void Deactivate()
	{
		_sellout = null;
		base.gameObject.SetActive(value: false);
	}
}
