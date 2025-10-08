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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
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
		IDisposable value = ObservableExtensions.Subscribe<CalendarLoadingStatus>((IObservable<CalendarLoadingStatus>)calendarModel.LoadingStatus, (Action<CalendarLoadingStatus>)delegate(CalendarLoadingStatus state)
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
		return Observable.Select<(CalendarModel, CalendarLoadingStatus), CalendarModel>(Observable.Where<(CalendarModel, CalendarLoadingStatus)>((IObservable<(CalendarModel, CalendarLoadingStatus)>)_onLoadingStateChangeSubject, (Func<(CalendarModel, CalendarLoadingStatus), bool>)(((CalendarModel, CalendarLoadingStatus) tuple) => tuple.Item1.EventType == type && tuple.Item1.LoadingStatus.Value == loadingStatus)), (Func<(CalendarModel, CalendarLoadingStatus), CalendarModel>)(((CalendarModel, CalendarLoadingStatus) tuple) => tuple.Item1));
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
		IConnectableObservable<IBundleProvider<BattlePassBundleData>> val = Observable.Publish<IBundleProvider<BattlePassBundleData>>(Observable.Catch<IBundleProvider<BattlePassBundleData>, Exception>(Observable.ContinueWith<BattlePass, IBundleProvider<BattlePassBundleData>>(Observable.Do<BattlePass>(Observable.Select<BattlePassBundleData, BattlePass>(_bundleLoader.Load<BattlePassBundleData>(BundleType.BattlePass, battlePassMapper.bp_bundle.ToLower(), ContentSource.BattlePass), (Func<BattlePassBundleData, BattlePass>)((BattlePassBundleData data) => CreateBattlePass(calendarModel, battlePassMapper, data))), (Action<BattlePass>)delegate(BattlePass battlePass)
		{
			_battlePassSettingsProvider.Add(battlePass);
			calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.End);
			Remove(calendarModel);
		}), (Func<BattlePass, IObservable<IBundleProvider<BattlePassBundleData>>>)SelectGirlsId), (Func<Exception, IObservable<IBundleProvider<BattlePassBundleData>>>)delegate(Exception ex)
		{
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
			throw ex.SendException(GetType().Name + ": Error when creating BattlePass" + $" with structure: {EventStructureType.BattlePass}" + "\n");
		}));
		IConnectableObservable<IEnumerable<Sprite>> val2 = Observable.Publish<IEnumerable<Sprite>>(_iconsLoader.Load(calendarModel.EventMapper.Bundle));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IEnumerable<Sprite>>((IObservable<IEnumerable<Sprite>>)val2, (Action<IEnumerable<Sprite>>)_iconProvider.AddRange), (ICollection<IDisposable>)_calendarLoadDisposable);
		IConnectableObservable<CalendarModel> obj = Observable.Publish<CalendarModel>(Observable.Do<CalendarModel>(Observable.Select<Unit, CalendarModel>(Observable.LastOrDefault<Unit>(Observable.Merge<Unit>(Observable.AsUnitObservable<IBundleProvider<BattlePassBundleData>>((IObservable<IBundleProvider<BattlePassBundleData>>)val), new IObservable<Unit>[1] { Observable.AsUnitObservable<IEnumerable<Sprite>>((IObservable<IEnumerable<Sprite>>)val2) })), (Func<Unit, CalendarModel>)((Unit _) => calendarModel)), (Action<CalendarModel>)delegate
		{
		}));
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		DisposableExtensions.AddTo<IDisposable>(val2.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		return (IObservable<CalendarModel>)obj;
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
		IObservable<Unit> observable = Observable.AsUnitObservable<ICharacter>(Observable.LastOrDefault<ICharacter>(Observable.DefaultIfEmpty<ICharacter>(Observable.Concat<ICharacter>(from girl in source.OfType<CardLinkedContent>()
			select girl.Card.ID into id
			select TryLoadCharacter(id, battlePass)))));
		IObservable<Unit> observable2 = Observable.AsUnitObservable<SkinData>(Observable.LastOrDefault<SkinData>(Observable.DefaultIfEmpty<SkinData>(Observable.Concat<SkinData>(from girl in source.OfType<SkinLinkedContent>()
			select girl.Skin.ID into id
			select TryLoadSkin(id, battlePass)))));
		return Observable.Select<Unit, BattlePass>(Observable.LastOrDefault<Unit>(Observable.Merge<Unit>(observable, new IObservable<Unit>[1] { observable2 })).Debug($"Finished character and skin data loading for battle pass {battlePass.ID}", LogType.Events), (Func<Unit, BattlePass>)((Unit _) => battlePass));
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
