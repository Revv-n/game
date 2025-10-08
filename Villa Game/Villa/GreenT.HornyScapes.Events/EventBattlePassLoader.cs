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
		IConnectableObservable<IBundleProvider<BattlePassBundleData>> connectableObservable = (from data in _bundleLoader.Load(battlePassMapper.Bundle)
			select CreateBattlePass(battlePassMapper, data)).Do(delegate(BattlePass battlePass)
		{
			_settingsProvider.Add(battlePass);
		}).ContinueWith(SelectGirlsId).Catch(delegate(Exception ex)
		{
			throw ex.SendException(GetType().Name + ": Error when creating BattlePass" + $" with structure: {EventStructureType.BattlePass}" + "\n");
		})
			.Publish();
		IConnectableObservable<IEnumerable<Sprite>> connectableObservable2 = _iconsLoader.Load(battlePassMapper.Bundle).Publish();
		connectableObservable2.Subscribe(_iconProvider.AddRange).AddTo(_calendarLoadDisposable);
		IConnectableObservable<BattlePass> connectableObservable3 = (from _ in connectableObservable.AsUnitObservable().Merge(connectableObservable2.AsUnitObservable()).LastOrDefault()
			select _settingsProvider.GetBattlePass(battlePassMapper.bp_id)).Do(delegate
		{
		}).Publish();
		connectableObservable.Connect().AddTo(_calendarLoadDisposable);
		connectableObservable2.Connect().AddTo(_calendarLoadDisposable);
		connectableObservable3.Connect().AddTo(_calendarLoadDisposable);
		return connectableObservable3;
	}

	private BattlePass CreateBattlePass(BattlePassMapper battlePassMapper, BattlePassBundleData data)
	{
		return _factory.Create(battlePassMapper, data, BattlePassCategory.Event);
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
}
