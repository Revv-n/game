using System;
using GreenT.AssetBundles;
using GreenT.Settings.Data;
using StripClub.Model.Character;
using StripClub.Model.Data;
using StripClub.Model.Shop.Data;
using UniRx;

namespace GreenT.HornyScapes.Characters;

public class CharacterBundleLoader : ILoader<int, CharacterData>
{
	private readonly BundleLoader _bundleLoader;

	public CharacterBundleLoader(BundleLoader bundleLoader)
	{
		_bundleLoader = bundleLoader;
	}

	public IObservable<CharacterData> Load(int characterID)
	{
		return (from _characterData in _bundleLoader.Load<CharacterData>(BundleType.CharactersData, characterID.ToString(), ContentSource.Employee)
			where _characterData.ID.Equals(characterID)
			select _characterData).CatchIgnore<CharacterData, Exception>(delegate
		{
		});
	}
}
