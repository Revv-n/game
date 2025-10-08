using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.Content;
using StripClub.Model;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public sealed class EventBattlePassLoader
{
	private readonly BattlePassBundleDataLoader _bundleLoader;

	private readonly BattlePassFactory _factory;

	private readonly BattlePassSettingsProvider _settingsProvider;

	private readonly BattlePassMergeIconsLoader _iconsLoader;

	private readonly BattlePassMergeIconProvider _iconProvider;

	private readonly CharacterProvider _characterProvider;

	private readonly SkinDataLoadingController _skinLoader;

	private readonly CompositeDisposable _calendarLoadDisposable = new CompositeDisposable();

	public EventBattlePassLoader(BattlePassBundleDataLoader bundleLoader, BattlePassFactory factory, BattlePassSettingsProvider settingsProvider, BattlePassMergeIconsLoader iconsLoader, BattlePassMergeIconProvider iconProvider, CharacterProvider characterProvider, SkinDataLoadingController skinLoader)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_bundleLoader = bundleLoader;
		_factory = factory;
		_settingsProvider = settingsProvider;
		_iconsLoader = iconsLoader;
		_iconProvider = iconProvider;
		_characterProvider = characterProvider;
		_skinLoader = skinLoader;
	}

	public IObservable<BattlePass> Load(BattlePassMapper battlePassMapper)
	{
		IConnectableObservable<IBundleProvider<BattlePassBundleData>> val = Observable.Publish<IBundleProvider<BattlePassBundleData>>(Observable.Catch<IBundleProvider<BattlePassBundleData>, Exception>(Observable.ContinueWith<BattlePass, IBundleProvider<BattlePassBundleData>>(Observable.Do<BattlePass>(Observable.Select<BattlePassBundleData, BattlePass>(_bundleLoader.Load(battlePassMapper.Bundle), (Func<BattlePassBundleData, BattlePass>)((BattlePassBundleData data) => CreateBattlePass(battlePassMapper, data))), (Action<BattlePass>)delegate(BattlePass battlePass)
		{
			_settingsProvider.Add(battlePass);
		}), (Func<BattlePass, IObservable<IBundleProvider<BattlePassBundleData>>>)SelectGirlsId), (Func<Exception, IObservable<IBundleProvider<BattlePassBundleData>>>)delegate(Exception ex)
		{
			throw ex.SendException(GetType().Name + ": Error when creating BattlePass" + $" with structure: {EventStructureType.BattlePass}" + "\n");
		}));
		IConnectableObservable<IEnumerable<Sprite>> val2 = Observable.Publish<IEnumerable<Sprite>>(_iconsLoader.Load(battlePassMapper.Bundle));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IEnumerable<Sprite>>((IObservable<IEnumerable<Sprite>>)val2, (Action<IEnumerable<Sprite>>)_iconProvider.AddRange), (ICollection<IDisposable>)_calendarLoadDisposable);
		IConnectableObservable<BattlePass> obj = Observable.Publish<BattlePass>(Observable.Do<BattlePass>(Observable.Select<Unit, BattlePass>(Observable.LastOrDefault<Unit>(Observable.Merge<Unit>(Observable.AsUnitObservable<IBundleProvider<BattlePassBundleData>>((IObservable<IBundleProvider<BattlePassBundleData>>)val), new IObservable<Unit>[1] { Observable.AsUnitObservable<IEnumerable<Sprite>>((IObservable<IEnumerable<Sprite>>)val2) })), (Func<Unit, BattlePass>)((Unit _) => _settingsProvider.GetBattlePass(battlePassMapper.bp_id))), (Action<BattlePass>)delegate
		{
		}));
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		DisposableExtensions.AddTo<IDisposable>(val2.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		return (IObservable<BattlePass>)obj;
	}

	private BattlePass CreateBattlePass(BattlePassMapper battlePassMapper, BattlePassBundleData data)
	{
		return _factory.Create(battlePassMapper, data, BattlePassCategory.Event);
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
}
