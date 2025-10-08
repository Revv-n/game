using System;
using System.Collections.Generic;
using GreenT.AssetBundles;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public sealed class MiniEventFactory : IFactory<MiniEventMapper, CalendarModel, MiniEvent>, IFactory
{
	private readonly MiniEventAdministrator _miniEventAdministrator;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	private readonly ActivitiesManager _activitiesManager;

	private readonly RatingManager _activitiesRatingManager;

	private readonly MiniEventConfigHandler _miniEventConfigHandler;

	private readonly SimpleCurrencyFactory _simpleCurrencyFactory;

	private AssetProvider _assetProvider;

	private readonly LinkedContentFactory _linkedContentFactory;

	private readonly ISaver _saver;

	public MiniEventFactory(RatingManager activitiesRatingManager, MiniEventAdministrator miniEventAdministrator, ActivitiesManager activitiesManager, MiniEventConfigHandler miniEventTabsFactory, SimpleCurrencyFactory simpleCurrencyFactory, LinkedContentFactory linkedContentFactory, LinkedContentAnalyticDataFactory analyticDataFactory, ISaver saver, AssetProvider assetProvider)
	{
		_activitiesRatingManager = activitiesRatingManager;
		_miniEventAdministrator = miniEventAdministrator;
		_analyticDataFactory = analyticDataFactory;
		_linkedContentFactory = linkedContentFactory;
		_miniEventConfigHandler = miniEventTabsFactory;
		_activitiesManager = activitiesManager;
		_simpleCurrencyFactory = simpleCurrencyFactory;
		_saver = saver;
		_assetProvider = assetProvider;
	}

	public MiniEvent Create(MiniEventMapper eventMapper, CalendarModel calendarModel)
	{
		IEnumerable<IController> controllers = null;
		bool isMultiTabbed = false;
		switch (eventMapper.config_type)
		{
		case ConfigType.Activity:
		{
			ActivityMapper activityInfo = _activitiesManager.GetActivityInfo(eventMapper.activity_id);
			controllers = _miniEventConfigHandler.Handle(eventMapper.config_type, activityInfo, eventMapper.id, eventMapper.activity_id, calendarModel.UniqID);
			isMultiTabbed = activityInfo.mult_tab;
			break;
		}
		case ConfigType.Rating:
		{
			Rating ratingInfo = _activitiesRatingManager.GetRatingInfo(eventMapper.activity_id);
			controllers = _miniEventConfigHandler.Handle(eventMapper.config_type, ratingInfo, eventMapper.id, eventMapper.activity_id, calendarModel.UniqID);
			break;
		}
		}
		CompositeIdentificator currencyIdentificator = new CompositeIdentificator(eventMapper.currency_id);
		MiniEvent.PromoSettings promoSettings = default(MiniEvent.PromoSettings);
		promoSettings.PromoView = eventMapper.promo_view;
		promoSettings.PromoContent = CreatePromoContent(eventMapper.promo_type, eventMapper.promo_id, eventMapper.promo_qty);
		MiniEvent.ViewSettings? viewSettings = ((eventMapper.girl_view != null || eventMapper.minitab_view != null || eventMapper.background != null) ? new MiniEvent.ViewSettings?(GetMiniEventView(eventMapper)) : null);
		MiniEvent miniEvent = new MiniEvent(calendarModel.UniqID, eventMapper.position, eventMapper.activity_id, eventMapper.id, isMultiTabbed, controllers, currencyIdentificator, viewSettings, promoSettings, eventMapper.config_type);
		_miniEventAdministrator.AdministrateMiniEvent(miniEvent);
		_saver.Add(miniEvent);
		miniEvent.Initialize();
		return miniEvent;
	}

	public MiniEvent.ViewSettings GetMiniEventView(MiniEventMapper eventMapper)
	{
		MiniEvent.ViewSettings result = default(MiniEvent.ViewSettings);
		result.Background = SetupAsset(eventMapper.background, AssetResolveType.Fake | AssetResolveType.Hard);
		result.Girl = SetupAsset(eventMapper.girl_view, AssetResolveType.Silent);
		result.Tab = SetupAsset(eventMapper.minitab_view, AssetResolveType.Fake | AssetResolveType.Hard);
		return result;
	}

	private Sprite SetupAsset(string preset, AssetResolveType resolveType)
	{
		string[] array = preset.Split(':');
		ContentSource contentSource = ((array.Length < 2) ? ContentSource.MiniEvent : Enum.Parse<ContentSource>(array[0]));
		return _assetProvider.FindBundleImageOrFake(contentSource, array[^1], resolveType);
	}

	private List<LinkedContent> CreatePromoContent(RewType[] rewType, string[] reward_id, int[] reward_count)
	{
		List<LinkedContent> list = new List<LinkedContent>();
		if (rewType == null || reward_id == null || reward_count == null)
		{
			return list;
		}
		for (int i = 0; i < rewType.Length; i++)
		{
			Selector selector = SelectorTools.CreateSelector(reward_id[i]);
			LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.None);
			LinkedContent item = _linkedContentFactory.Create(rewType[i], selector, reward_count[i], 0, ContentType.Main, analyticData);
			list.Add(item);
		}
		return list;
	}
}
