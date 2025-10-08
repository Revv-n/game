using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.Data;
using GreenT.Extensions;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MergeField;
using GreenT.Types;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventFactory : IFactory<EventMapper, EventBundleData, CalendarModel, IBundleProvider<EventBundleData>>, IFactory
{
	private readonly ISaver _saver;

	private readonly DropFactory _dropFactory;

	private readonly MergeFieldManager _fieldManager;

	private readonly LinkedContentFactory _linkedFactory;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	private readonly RatingDataFactory _ratingDataFactory;

	private readonly EventRatingControllerFactory _ratingControllerFactory;

	private readonly RatingManager _ratingManager;

	private readonly RatingService _ratingService;

	private readonly List<RatingData> _ratingDatas;

	private readonly AssetProvider _assetProvider;

	public EventFactory(LinkedContentFactory linkedFactory, DropFactory dropFactory, ISaver saver, LinkedContentAnalyticDataFactory analyticDataFactory, AssetProvider assetProvider, RatingDataFactory ratingDataFactory, EventRatingControllerFactory ratingControllerFactory, RatingManager ratingManager, RatingService ratingService, MergeFieldManager fieldManager)
	{
		_saver = saver;
		_dropFactory = dropFactory;
		_linkedFactory = linkedFactory;
		_analyticDataFactory = analyticDataFactory;
		_ratingDataFactory = ratingDataFactory;
		_ratingControllerFactory = ratingControllerFactory;
		_ratingManager = ratingManager;
		_ratingService = ratingService;
		_fieldManager = fieldManager;
		_assetProvider = assetProvider;
		_ratingDatas = new List<RatingData>();
	}

	public IBundleProvider<EventBundleData> Create(EventMapper mapper, EventBundleData bundle, CalendarModel calendarModel)
	{
		_ratingDatas.Clear();
		ViewSettings viewSettings2;
		if (mapper.event_view == null || mapper.event_view.Length == 0)
		{
			ViewSettings viewSettings = default(ViewSettings);
			viewSettings.Currency = bundle.Currency;
			viewSettings.AlternativeCurrency = bundle.AlternativeCurrency;
			viewSettings.Target = bundle.Target;
			viewSettings.ProgressGirl = bundle.ProgressGirl;
			viewSettings.StartGirl = bundle.StartGirl;
			viewSettings.BankButtonSp = bundle.BankButtonSp;
			viewSettings.ButtonSp = bundle.ButtonSp;
			viewSettings.MergeBackground = bundle.MergeBackground;
			viewSettings.RecipeBookButton = bundle.RecieBookButton;
			viewSettings.RecipeBook = bundle.RecipeBook;
			viewSettings.AnnouncementBackground = bundle.AnnouncementBackground;
			viewSettings.AnnouncementTitleBackground = bundle.AnnouncementTitleBackground;
			viewSettings.BattlePassBackground = bundle.BattlePassBackground;
			viewSettings.StartWindowBackground = bundle.StartWindowBackground;
			viewSettings.DefaultField = bundle.DefaultField;
			viewSettings2 = viewSettings;
		}
		else
		{
			viewSettings2 = SetupViewSettings(mapper.event_view);
		}
		ViewSettings viewSettings3 = viewSettings2;
		List<IController> list = new List<IController>();
		List<DropSettings> previewCards = CreatePreviewCards(mapper);
		List<EventReward> eventReward = CreateRewards(mapper, viewSettings3);
		MergeFieldMapper defaultField = _fieldManager.Collection.FirstOrDefault((MergeFieldMapper item) => item.id == mapper.event_id && item.bundle == mapper.event_bundle);
		if (mapper.rating_id != 0)
		{
			IController item2 = PrepareRating(mapper.rating_id, mapper.event_id, isGlobal: true, calendarModel, !list.Any(), _ratingDatas);
			list.Add(item2);
		}
		if (mapper.group_rating_id != 0)
		{
			IController item3 = PrepareRating(mapper.group_rating_id, mapper.event_id, isGlobal: false, calendarModel, !list.Any(), _ratingDatas);
			list.Add(item3);
		}
		Event @event = new Event(calendarModel.UniqID, mapper.event_id, viewSettings3, previewCards, eventReward, defaultField, mapper.focus, mapper.has_recipe_book, bundle, list, mapper.rating_id, mapper.group_rating_id, mapper.is_separate_energy, mapper.bp_id);
		_saver.Add(@event);
		@event.Initialize();
		return @event;
	}

	private ViewSettings SetupViewSettings(string[] presets)
	{
		ViewSettings result = default(ViewSettings);
		result.Currency = GetEventView(presets[0], AssetResolveType.Fake);
		result.AlternativeCurrency = GetEventView(presets[0], AssetResolveType.Fake);
		result.Target = GetEventView(presets[1], AssetResolveType.Fake);
		result.ProgressGirl = GetEventView(presets[2], AssetResolveType.Fake);
		result.StartGirl = GetEventView(presets[3], AssetResolveType.Fake);
		result.BankButtonSp = GetEventView(presets[4], AssetResolveType.Fake);
		result.ButtonSp = GetEventView(presets[5], AssetResolveType.Fake);
		result.MergeBackground = GetEventView(presets[7], AssetResolveType.Fake);
		result.RecipeBookButton = GetEventView(presets[8], AssetResolveType.Silent);
		result.RecipeBook = GetEventView(presets[9], AssetResolveType.Silent);
		result.AnnouncementBackground = GetEventView(presets[10], AssetResolveType.Fake);
		result.AnnouncementTitleBackground = GetEventView(presets[11], AssetResolveType.Fake);
		result.StartWindowBackground = GetEventView(presets[12], AssetResolveType.Fake);
		result.DefaultField = GetField(presets[13], AssetResolveType.Hard);
		return result;
	}

	private Sprite GetEventView(string preset, AssetResolveType resolveType = (AssetResolveType)0)
	{
		string[] array = preset.Split(":", StringSplitOptions.None);
		ContentSource contentSource = ((array.Length < 2) ? ContentSource.EventBundle : Enum.Parse<ContentSource>(array[0]));
		return _assetProvider.FindBundleImageOrFake(contentSource, array[array.Length - 1], resolveType);
	}

	private TextAsset GetField(string preset, AssetResolveType resolveType = (AssetResolveType)0)
	{
		string[] array = preset.Split(":", StringSplitOptions.None);
		ContentSource contentSource = ((array.Length < 2) ? ContentSource.EventBundle : Enum.Parse<ContentSource>(array[0]));
		return _assetProvider.FindBundleAssetOrFake(contentSource, array[array.Length - 1], resolveType);
	}

	public IObservable<Event> TryRegisterRating(Event @event)
	{
		if (!_ratingDatas.Any())
		{
			return Observable.Return<Event>(@event);
		}
		string text = null;
		foreach (RatingData ratingData2 in _ratingDatas)
		{
			if (string.IsNullOrEmpty(text))
			{
				if (!string.IsNullOrEmpty(ratingData2.AuthorizationToken))
				{
					text = ratingData2.AuthorizationToken;
				}
			}
			else
			{
				ratingData2.AuthorizationToken = text;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			RatingData targetRatingData = _ratingDatas.First();
			return Observable.Catch<Event, Exception>(Observable.Select<RegistrationResponse, Event>(Observable.Do<RegistrationResponse>(Observable.Take<RegistrationResponse>(_ratingService.TryRegister(targetRatingData), 1), (Action<RegistrationResponse>)delegate(RegistrationResponse response)
			{
				if (response != null)
				{
					RatingData ratingData = _ratingDatas.FirstOrDefault();
					if (ratingData != null)
					{
						float playerPower = ratingData.PlayerPower;
						foreach (RatingData ratingData3 in _ratingDatas)
						{
							ratingData3.AuthorizationToken = response.token;
							if (ratingData3.PlayerPower == 0f)
							{
								ratingData3.PlayerPower = playerPower;
							}
						}
					}
				}
			}), (Func<RegistrationResponse, Event>)((RegistrationResponse _) => @event)), (Func<Exception, IObservable<Event>>)delegate(Exception ex)
			{
				throw ex;
			});
		}
		return Observable.Return<Event>(@event);
	}

	private RatingController PrepareRating(int ratingId, int eventId, bool isGlobal, CalendarModel calendarModel, bool isCurrencyTrackable, List<RatingData> ratingDatas)
	{
		Rating ratingInfo = _ratingManager.GetRatingInfo(ratingId);
		RatingData ratingData = _ratingDataFactory.Create(eventId, calendarModel.UniqID, isGlobal, ratingInfo);
		RatingController result = _ratingControllerFactory.Create(ratingData, isCurrencyTrackable, calendarModel);
		ratingDatas.Add(ratingData);
		return result;
	}

	private List<EventReward> CreateRewards(EventMapper mapper, ViewSettings viewSettings)
	{
		List<EventReward> list = new List<EventReward>();
		if (mapper.bp_id != 0)
		{
			return list;
		}
		CheckRewardsLenght(mapper);
		try
		{
			for (int i = 0; i < mapper.event_targets.Length; i++)
			{
				Selector selector = SelectorTools.CreateSelector(mapper.rew_id[i]);
				LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.Event);
				EventReward item = new EventReward(_linkedFactory.Create(mapper.rew_type[i], selector, mapper.rew_qty[i], 0, ContentType.Main, analyticData), mapper.event_targets[i], viewSettings.Target, (i == 0) ? null : list[i - 1]);
				list.Add(item);
			}
			return list;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name + " can't create EventRewards," + $" check event_targets/rew_type/rew_id/rew_qty from {mapper.event_id}");
		}
	}

	private List<DropSettings> CreatePreviewCards(EventMapper mapper)
	{
		CheckPreviewLenght(mapper);
		List<DropSettings> list = new List<DropSettings>();
		try
		{
			for (int i = 0; i < mapper.preview_rew_id.Length; i++)
			{
				Selector selector = SelectorTools.CreateSelector(mapper.preview_rew_id[i]);
				list.Add(_dropFactory.Create(mapper.preview_rew_type[i], selector, mapper.preview_rew_qty[i], 0, ContentType.Main));
			}
			return list;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name + " can't create PreviewCards," + $" check preview_rew_type/preview_rew_id/preview_rew_qty from {mapper.event_id}");
		}
	}

	private bool CheckRewardsLenght(EventMapper mapper)
	{
		return ExtensionMethods.CheckRewardsLength(mapper.event_targets.Length, mapper.rew_type, "rew_type", mapper.rew_id, "rew_id", mapper.rew_qty, "rew_qty");
	}

	private bool CheckPreviewLenght(EventMapper mapper)
	{
		return ExtensionMethods.CheckRewardsLength(mapper.preview_rew_type.Length, mapper.preview_rew_type, "preview_rew_type", mapper.preview_rew_id, "preview_rew_id", mapper.preview_rew_qty, "preview_rew_qty");
	}

	private void CheckProperty(int referenceLenght, int lenght, string errorProperty, out bool condition)
	{
		ExtensionMethods.CheckProperty(referenceLenght, lenght, errorProperty, out condition);
	}
}
