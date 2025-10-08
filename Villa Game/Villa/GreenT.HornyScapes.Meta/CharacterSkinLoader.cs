using System;
using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using Spine.Unity;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Meta;

public class CharacterSkinLoader : IBundlesLoader<int, SkeletonAnimation>, ILoader<int, SkeletonAnimation>
{
	private readonly IAddressablesBundlesLoader _addressablesService;

	private readonly IProjectSettings _projectSettings;

	public CharacterSkinLoader(IAddressablesBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
	{
		_addressablesService = assetBundlesLoader;
		_projectSettings = projectSettings;
	}

	private string GetPath(int bundleName)
	{
		return string.Format(_projectSettings.BundleUrlResolver.BundleUrl(BundleType.CharacterSkinAnimation), bundleName);
	}

	IObservable<SkeletonAnimation> ILoader<int, SkeletonAnimation>.Load(int bundleName)
	{
		string name = bundleName.ToString();
		return from x in AssetsLoaderUtilities<GameObject>.LoadAssetByName(_addressablesService, () => GetPath(bundleName), name)
			select x.GetComponent<SkeletonAnimation>();
	}

	public void ReleaseBundle(int param)
	{
		AssetsLoaderUtilities<GameObject>.ReleaseBundle(_addressablesService, GetPath(param));
	}
}
