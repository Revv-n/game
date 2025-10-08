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
		return Observable.CatchIgnore<CharacterData, Exception>(Observable.Where<CharacterData>(_bundleLoader.Load<CharacterData>(BundleType.CharactersData, characterID.ToString(), ContentSource.Employee), (Func<CharacterData, bool>)((CharacterData _characterData) => _characterData.ID.Equals(characterID))), (Action<Exception>)delegate
		{
		});
	}
}
