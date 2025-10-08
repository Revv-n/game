using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;

namespace GreenT.HornyScapes.Characters.Skins;

public class SkinLoader : AssetBundleLoaderByType<int, SkinData>
{
	public SkinLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
		: base(assetBundlesLoader, projectSettings, BundleType.CharacterSkinCard)
	{
	}

	public override string GetPath(int bundleName)
	{
		return string.Format(projectSettings.BundleUrlResolver.BundleUrl(bundleType), bundleName);
	}
}
