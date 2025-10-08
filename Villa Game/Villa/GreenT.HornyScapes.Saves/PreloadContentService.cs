using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Extensions;
using StripClub.Model;
using StripClub.Model.Character;
using UniRx;

namespace GreenT.HornyScapes.Saves;

public class PreloadContentService : IDisposable
{
	private readonly CharacterBundleLoader _characterBundleDataLoader;

	private readonly SkinDataLoadingController _skinDataLoadingController;

	private readonly CharacterProvider _characterProvider;

	private readonly CompositeDisposable _streams = new CompositeDisposable();

	public PreloadContentService(CharacterBundleLoader characterBundleDataLoader, SkinDataLoadingController skinDataLoadingController, CharacterProvider characterProvider)
	{
		_characterBundleDataLoader = characterBundleDataLoader;
		_skinDataLoadingController = skinDataLoadingController;
		_characterProvider = characterProvider;
	}

	public void PreloadRewards(LinkedContent linkedContents)
	{
		GetPreloadRewardsStream(linkedContents).Debug("PreloadRewards").LastOrDefault().Subscribe()
			.AddTo(_streams);
	}

	public void PreloadRewards(IEnumerable<LinkedContent> linkedContents)
	{
		GetPreloadRewardsStream(linkedContents).Debug("PreloadRewards").LastOrDefault().Subscribe()
			.AddTo(_streams);
	}

	public IObservable<Unit> GetPreloadRewardsStream(LinkedContent linkedContents)
	{
		List<LinkedContent> linkedContents2 = LinkedContentExtensions.TransformToArray(linkedContents);
		return GetPreloadRewardsStream(linkedContents2);
	}

	public IObservable<Unit> GetPreloadRewardsStream(IEnumerable<LinkedContent> linkedContents)
	{
		if (!linkedContents.Any())
		{
			return Observable.Empty<Unit>().AsSingleUnitObservable();
		}
		IObservable<Unit> first = (from _lc in linkedContents.OfType<CardLinkedContent>()
			select _lc.Card as CharacterInfo).Select(_characterProvider.Get).Concat().AsSingleUnitObservable();
		IObservable<Unit> observable = (from _lc in linkedContents.OfType<SkinLinkedContent>()
			select _lc.Skin).Select(_skinDataLoadingController.InsertDataOnLoad).Concat().AsSingleUnitObservable();
		return first.Merge(observable);
	}

	public IObservable<CharacterData> LoadBundleByCharacter(CharacterInfo _character)
	{
		return _characterBundleDataLoader.Load(_character.ID).Catch(delegate(Exception ex)
		{
			throw ex.SendException("Exception on trying to load character: " + _character);
		}).Do(_character.Set);
	}

	public void Dispose()
	{
		_streams?.Dispose();
	}
}
