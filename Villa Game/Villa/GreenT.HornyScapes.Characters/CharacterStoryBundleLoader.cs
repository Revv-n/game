using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Character;

namespace GreenT.HornyScapes.Characters;

public class CharacterStoryBundleLoader : AssetBundleLoaderByType<int, CharacterStories>
{
	public CharacterStoryBundleLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
		: base(assetBundlesLoader, projectSettings, bundleType)
	{
	}
}
