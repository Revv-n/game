using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Messenger.Data;

namespace GreenT.HornyScapes.Characters;

public class CustomConversationDataBundleLoader : AssetBundleLoaderByType<string, CustomConversationData>
{
	public CustomConversationDataBundleLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
		: base(assetBundlesLoader, projectSettings, bundleType)
	{
	}
}
