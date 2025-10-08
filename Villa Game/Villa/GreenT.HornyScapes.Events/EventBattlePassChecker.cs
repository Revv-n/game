using System;
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
		IConnectableObservable<ICharacter> connectableObservable = ((bp_preview_girl_id != 0) ? _characterProvider.Get(bp_preview_girl_id).Debug($"CharactersLoading, characterId = {bp_preview_girl_id}").Publish() : (Observable.Empty<ICharacter>() as IConnectableObservable<ICharacter>));
		IConnectableObservable<CharacterStories> connectableObservable2 = ((bp_preview_girl_id != 0) ? _characterProvider.GetStory(bp_preview_girl_id).Debug($"CharactersStoriesLoading, characterId = {bp_preview_girl_id}").Publish() : (Observable.Empty<CharacterStories>() as IConnectableObservable<CharacterStories>));
		IConnectableObservable<BattlePass> connectableObservable3 = (from _ in _loader.Load(eventMapper).Do(delegate(BattlePass battlePass)
			{
				InitBattlePass(battlePass, currencyTypeTarget, saveKey);
			}).AsUnitObservable()
				.Merge(connectableObservable.AsUnitObservable())
				.Merge(connectableObservable2.AsUnitObservable())
				.LastOrDefault()
				.Debug($"Character {bp_preview_girl_id} loaded, battlePass id = {battlePassId}")
			select _battlePassSettingsProvider.GetBattlePass(battlePassId)).Do(delegate(BattlePass battlePass)
		{
			TrySetRewPlaceholder(battlePass);
		}).Publish();
		connectableObservable.Connect().AddTo(_disposables);
		connectableObservable2.Connect().AddTo(_disposables);
		connectableObservable3.Connect().AddTo(_disposables);
		return connectableObservable3;
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
