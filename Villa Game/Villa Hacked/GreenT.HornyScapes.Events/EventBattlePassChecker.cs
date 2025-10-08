using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Lootboxes;
using StripClub;
using StripClub.Model;
using StripClub.Model.Character;
using UniRx;

namespace GreenT.HornyScapes.Events;

public sealed class EventBattlePassChecker
{
	private readonly BattlePassMapperProvider _mapperProvider;

	private readonly EventBattlePassLoader _loader;

	private readonly EventBattlePassDataBuilder _dataBuilder;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly CharacterProvider _characterProvider;

	private readonly GameSettings _gameSettings;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public EventBattlePassChecker(BattlePassMapperProvider mapperProvider, EventBattlePassLoader loader, EventBattlePassDataBuilder dataBuilder, BattlePassSettingsProvider battlePassSettingsProvider, CharacterProvider characterProvider, GameSettings gameSettings)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_mapperProvider = mapperProvider;
		_loader = loader;
		_dataBuilder = dataBuilder;
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_characterProvider = characterProvider;
		_gameSettings = gameSettings;
	}

	public IObservable<BattlePass> LoadBattlePass(int battlePassId)
	{
		if (battlePassId == 0)
		{
			return Observable.Empty<BattlePass>();
		}
		BattlePassMapper eventMapper = _mapperProvider.GetEventMapper(battlePassId);
		string text = eventMapper.bp_resource;
		if (string.IsNullOrEmpty(text))
		{
			text = "bp_points";
		}
		string saveKey = $"{1}_{eventMapper.bp_id}_battlepass";
		SelectorTools.GetResourceEnumValueByConfigKey(text, out var currencyTypeTarget);
		int bp_preview_girl_id = eventMapper.bp_preview_girl_id;
		IConnectableObservable<ICharacter> val = ((bp_preview_girl_id != 0) ? Observable.Publish<ICharacter>(_characterProvider.Get(bp_preview_girl_id).Debug($"CharactersLoading, characterId = {bp_preview_girl_id}")) : (Observable.Empty<ICharacter>() as IConnectableObservable<ICharacter>));
		IConnectableObservable<CharacterStories> val2 = ((bp_preview_girl_id != 0) ? Observable.Publish<CharacterStories>(_characterProvider.GetStory(bp_preview_girl_id).Debug($"CharactersStoriesLoading, characterId = {bp_preview_girl_id}")) : (Observable.Empty<CharacterStories>() as IConnectableObservable<CharacterStories>));
		IConnectableObservable<BattlePass> obj = Observable.Publish<BattlePass>(Observable.Do<BattlePass>(Observable.Select<Unit, BattlePass>(Observable.LastOrDefault<Unit>(Observable.Merge<Unit>(Observable.Merge<Unit>(Observable.AsUnitObservable<BattlePass>(Observable.Do<BattlePass>(_loader.Load(eventMapper), (Action<BattlePass>)delegate(BattlePass battlePass)
		{
			InitBattlePass(battlePass, currencyTypeTarget, saveKey);
		})), new IObservable<Unit>[1] { Observable.AsUnitObservable<ICharacter>((IObservable<ICharacter>)val) }), new IObservable<Unit>[1] { Observable.AsUnitObservable<CharacterStories>((IObservable<CharacterStories>)val2) })).Debug($"Character {bp_preview_girl_id} loaded, battlePass id = {battlePassId}"), (Func<Unit, BattlePass>)((Unit _) => _battlePassSettingsProvider.GetBattlePass(battlePassId))), (Action<BattlePass>)delegate(BattlePass battlePass)
		{
			TrySetRewPlaceholder(battlePass);
		}));
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(val2.Connect(), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)_disposables);
		return (IObservable<BattlePass>)obj;
	}

	private void InitBattlePass(BattlePass battlePass, CurrencyType currencyTypeTarget, string saveKey)
	{
		_dataBuilder.CreateData(battlePass, currencyTypeTarget, saveKey);
	}

	private void TrySetRewPlaceholder(BattlePass battlePass)
	{
		BattlePassBundleData bundle = battlePass.Bundle;
		StripClub.GameSettings.RewTypeSettingsDictionary rewPlaceholder = _gameSettings.RewPlaceholder;
		if (rewPlaceholder.ContainsKey(RewType.Level))
		{
			RewSettings rewSettings = rewPlaceholder[RewType.Level];
			if (rewSettings.LevelRewardBaseIcon == null || rewSettings.LevelRewardExpensiveIcon == null)
			{
				rewSettings.Set(bundle);
			}
		}
		else
		{
			rewPlaceholder[RewType.Level].Set(bundle);
		}
	}
}
