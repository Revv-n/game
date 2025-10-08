using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.Settings.Data;
using StripClub.Model;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class BattlePassCalendarLoader : ICalendarLoader
{
	private readonly BattlePassFactory _battlePassFactory;

	private readonly BattlePassMapperProvider _battlePassMapperProvider;

	private readonly BattlePassMergeIconsLoader _iconsLoader;

	private readonly BattlePassMergeIconProvider _iconProvider;

	private readonly CalendarStrategyFactory _calendarStrategyFactory;

	private readonly CharacterProvider _characterProvider;

	private readonly SkinDataLoadingController _skinLoader;

	private readonly BundleLoader _bundleLoader;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly CompositeDisposable _calendarLoadDisposable = new CompositeDisposable();

	private readonly Dictionary<CalendarModel, IDisposable> _stateMap = new Dictionary<CalendarModel, IDisposable>();

	private readonly Subject<(CalendarModel, CalendarLoadingStatus)> _onLoadingStateChangeSubject = new Subject<(CalendarModel, CalendarLoadingStatus)>();

	public BattlePassCalendarLoader(BattlePassFactory battlePassFactory, BattlePassMergeIconsLoader iconsLoader, BattlePassMergeIconProvider iconProvider, BattlePassMapperProvider battlePassMapperProvider, CalendarStrategyFactory calendarStrategyFactory, CharacterProvider characterProvider, SkinDataLoadingController skinLoader, BattlePassSettingsProvider battlePassSettingsProvider, BundleLoader bundleLoader)
	{
		_iconsLoader = iconsLoader;
		_iconProvider = iconProvider;
		_battlePassFactory = battlePassFactory;
		_battlePassMapperProvider = battlePassMapperProvider;
		_calendarStrategyFactory = calendarStrategyFactory;
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_bundleLoader = bundleLoader;
		_characterProvider = characterProvider;
		_skinLoader = skinLoader;
	}

	public void SetLoadingStream(CalendarModel calendarModel)
	{
		IDisposable value = calendarModel.LoadingStatus.Subscribe(delegate(CalendarLoadingStatus state)
		{
			_onLoadingStateChangeSubject.OnNext((calendarModel, state));
		});
		_stateMap.Add(calendarModel, value);
	}

	private void Remove(CalendarModel calendarModel)
	{
		_stateMap[calendarModel]?.Dispose();
		_stateMap.Remove(calendarModel);
	}

	public IObservable<CalendarModel> OnConcreteCalendarLoadingStateChange(EventStructureType type, CalendarLoadingStatus loadingStatus)
	{
		return from tuple in _onLoadingStateChangeSubject
			where tuple.Item1.EventType == type && tuple.Item1.LoadingStatus.Value == loadingStatus
			select tuple.Item1;
	}

	public IObservable<CalendarModel> Load(CalendarModel calendarModel)
	{
		IEventMapper eventMapper = calendarModel.EventMapper;
		BattlePassMapper battlePassMapper = eventMapper as BattlePassMapper;
		if (battlePassMapper == null)
		{
			return Observable.Empty<CalendarModel>();
		}
		SetLoadingStream(calendarModel);
		calendarModel.SetLoadingStatus(CalendarModelLoadingState.InProgress);
		calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.Start);
		IConnectableObservable<IBundleProvider<BattlePassBundleData>> connectableObservable = (from data in _bundleLoader.Load<BattlePassBundleData>(BundleType.BattlePass, battlePassMapper.bp_bundle.ToLower(), ContentSource.BattlePass)
			select CreateBattlePass(calendarModel, battlePassMapper, data)).Do(delegate(BattlePass battlePass)
		{
			_battlePassSettingsProvider.Add(battlePass);
			calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.End);
			Remove(calendarModel);
		}).ContinueWith(SelectGirlsId).Catch(delegate(Exception ex)
		{
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
			throw ex.SendException(GetType().Name + ": Error when creating BattlePass" + $" with structure: {EventStructureType.BattlePass}" + "\n");
		})
			.Publish();
		IConnectableObservable<IEnumerable<Sprite>> connectableObservable2 = _iconsLoader.Load(calendarModel.EventMapper.Bundle).Publish();
		connectableObservable2.Subscribe(_iconProvider.AddRange).AddTo(_calendarLoadDisposable);
		IConnectableObservable<CalendarModel> connectableObservable3 = (from _ in connectableObservable.AsUnitObservable().Merge(connectableObservable2.AsUnitObservable()).LastOrDefault()
			select calendarModel).Do(delegate
		{
		}).Publish();
		connectableObservable.Connect().AddTo(_calendarLoadDisposable);
		connectableObservable2.Connect().AddTo(_calendarLoadDisposable);
		connectableObservable3.Connect().AddTo(_calendarLoadDisposable);
		return connectableObservable3;
	}

	private BattlePass CreateBattlePass(CalendarModel calendarModel, BattlePassMapper battlePassMapper, BattlePassBundleData data)
	{
		try
		{
			BattlePass battlePass = _battlePassFactory.Create(battlePassMapper, data, BattlePassCategory.Main);
			ICalendarStateStrategy calendarStateStrategy = _calendarStrategyFactory.Create(calendarModel.EventType, calendarModel);
			calendarModel.Set(battlePass, calendarStateStrategy, battlePass.ID);
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Success);
			return battlePass;
		}
		catch (Exception arg)
		{
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
			Debug.LogError(GetType().Name + ": Error when creating BattlePassSpace" + $" with structure: {EventStructureType.BattlePass}\n{arg}");
			throw;
		}
	}

	private IObservable<IBundleProvider<BattlePassBundleData>> SelectGirlsId(BattlePass battlePass)
	{
		List<LinkedContent> source = battlePass.AllRewardContainer.Rewards.Select((RewardWithManyConditions linkedContent) => linkedContent.Content).ToList();
		IObservable<Unit> first = (from girl in source.OfType<CardLinkedContent>()
			select girl.Card.ID into id
			select TryLoadCharacter(id, battlePass)).Concat().DefaultIfEmpty().LastOrDefault()
			.AsUnitObservable();
		IObservable<Unit> observable = (from girl in source.OfType<SkinLinkedContent>()
			select girl.Skin.ID into id
			select TryLoadSkin(id, battlePass)).Concat().DefaultIfEmpty().LastOrDefault()
			.AsUnitObservable();
		return from _ in first.Merge(observable).LastOrDefault().Debug($"Finished character and skin data loading for battle pass {battlePass.ID}", LogType.Events)
			select battlePass;
	}

	private IObservable<ICharacter> TryLoadCharacter(int id, BattlePass battlePass)
	{
		try
		{
			return _characterProvider.Get(id);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"Can't load character {id} for battle pass {battlePass.ID}");
		}
	}

	private IObservable<SkinData> TryLoadSkin(int id, BattlePass battlePass)
	{
		try
		{
			return _skinLoader.InsertDataOnLoad(id);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"Can't load skin {id} for battle pass {battlePass.ID}");
		}
	}

	public IEventMapper GetEventMapper(int battlePassID)
	{
		return _battlePassMapperProvider.GetEventMapper(battlePassID);
	}
}
