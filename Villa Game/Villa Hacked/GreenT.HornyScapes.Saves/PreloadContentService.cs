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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_characterBundleDataLoader = characterBundleDataLoader;
		_skinDataLoadingController = skinDataLoadingController;
		_characterProvider = characterProvider;
	}

	public void PreloadRewards(LinkedContent linkedContents)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.LastOrDefault<Unit>(GetPreloadRewardsStream(linkedContents).Debug("PreloadRewards"))), (ICollection<IDisposable>)_streams);
	}

	public void PreloadRewards(IEnumerable<LinkedContent> linkedContents)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.LastOrDefault<Unit>(GetPreloadRewardsStream(linkedContents).Debug("PreloadRewards"))), (ICollection<IDisposable>)_streams);
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
			return Observable.AsSingleUnitObservable<Unit>(Observable.Empty<Unit>());
		}
		IObservable<Unit> observable = Observable.AsSingleUnitObservable<ICharacter>(Observable.Concat<ICharacter>((from _lc in linkedContents.OfType<CardLinkedContent>()
			select _lc.Card as CharacterInfo).Select(_characterProvider.Get)));
		IObservable<Unit> observable2 = Observable.AsSingleUnitObservable<SkinData>(Observable.Concat<SkinData>((from _lc in linkedContents.OfType<SkinLinkedContent>()
			select _lc.Skin).Select(_skinDataLoadingController.InsertDataOnLoad)));
		return Observable.Merge<Unit>(observable, new IObservable<Unit>[1] { observable2 });
	}

	public IObservable<CharacterData> LoadBundleByCharacter(CharacterInfo _character)
	{
		return Observable.Do<CharacterData>(Observable.Catch<CharacterData, Exception>(_characterBundleDataLoader.Load(_character.ID), (Func<Exception, IObservable<CharacterData>>)delegate(Exception ex)
		{
			throw ex.SendException("Exception on trying to load character: " + _character);
		}), (Action<CharacterData>)_character.Set);
	}

	public void Dispose()
	{
		CompositeDisposable streams = _streams;
		if (streams != null)
		{
			streams.Dispose();
		}
	}
}
