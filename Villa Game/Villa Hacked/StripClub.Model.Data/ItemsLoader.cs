using System;
using System.Collections.Generic;
using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using UniRx;

namespace StripClub.Model.Data;

public class ItemsLoader : ILoader<IEnumerable<IItemInfo>>
{
	private IAssetBundlesLoader bundlesManager;

	private string itemsRequestURL;

	public ItemsLoader(IAssetBundlesLoader bundlesManager, IProjectSettings projectSettings)
	{
		this.bundlesManager = bundlesManager;
		itemsRequestURL = projectSettings.BundleUrlResolver.BundleUrl(BundleType.Items);
	}

	public IObservable<IEnumerable<IItemInfo>> Load()
	{
		return Observable.CatchIgnore<IEnumerable<ScriptableItemInfo>, Exception>(bundlesManager.GetAssets<ScriptableItemInfo>(itemsRequestURL), (Action<Exception>)delegate
		{
		});
	}
}
