using System;
using GreenT.Settings.Data;
using UniRx;
using UnityEngine;

namespace GreenT;

[CreateAssetMenu(menuName = "GreenT/Connection Settings/Bundles/Remote", order = 2)]
public class BundlesRemoteLoadingSettings : BundlesLoadingSettings
{
	[SerializeField]
	private string bundlesInfoRequestUrl;

	protected string bundlesRootPath = string.Empty;

	public override string BundlesRoot
	{
		get
		{
			if (string.IsNullOrEmpty(bundlesRootPath))
			{
				Debug.LogError("Bundles root path didn't initialized yet");
			}
			return bundlesRootPath;
		}
	}

	private string BundlesVersionRequestUrl(string bundlesVersion, string bundlesBuild)
	{
		return string.Format(bundlesInfoRequestUrl, bundlesVersion, bundlesBuild);
	}

	public IObservable<Unit> UpdateBundlesRequestUrls(string bundlesVersion, string bundleBuild)
	{
		bundlesRootPath = BundlesVersionRequestUrl(bundlesVersion, bundleBuild);
		return Observable.ReturnUnit();
	}
}
