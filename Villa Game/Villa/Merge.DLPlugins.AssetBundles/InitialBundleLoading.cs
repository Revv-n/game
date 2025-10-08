using System;
using System.Collections;
using BundlesManagement;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

internal class InitialBundleLoading
{
	private MonoBehaviour m_Host;

	public Action<bool> onComplete;

	public Action onSuccesComplete;

	public Action onFailedComplete;

	public Action<bool> onCachingDetecting;

	public Action<float, float, string> onLoadingProgress;

	public Action<int, int, float, float, string, string> OnBundleProgress;

	public InitialBundleLoading(MonoBehaviour host)
	{
		m_Host = host;
		AssetBundleManager.Instance.Initialization();
		onSuccesComplete = delegate
		{
		};
		onFailedComplete = delegate
		{
		};
		onComplete = delegate
		{
		};
		onCachingDetecting = delegate
		{
		};
		onLoadingProgress = delegate
		{
		};
		AssetBundleManager.Instance.bundle_delegates.asyncLoadingBundleCallback = delegate(int index, int count, float progress, float lenght, string unit, string name)
		{
			OnBundleProgress(index, count, progress, lenght, unit, name);
		};
	}

	public void ClearAction()
	{
		onSuccesComplete = null;
		onFailedComplete = null;
		onComplete = null;
		onLoadingProgress = null;
		onCachingDetecting = null;
		OnBundleProgress = null;
		AssetBundleManager.Instance.bundle_delegates.asyncLoadingBundleCallback = null;
	}

	public Coroutine LoadAssetBundles()
	{
		return m_Host?.StartCoroutine(LoadingAssetBundles());
	}

	public Coroutine ReadLocalicazitionBundle(Action<bool> OnComplete)
	{
		return m_Host?.StartCoroutine(ReadingLocalizationBundle(OnComplete));
	}

	private IEnumerator ReadingLocalizationBundle(Action<bool> OnComplete)
	{
		bool result = true;
		yield return AssetBundleManager.Instance.LocalBundleLoading(Bundles.Localization.Bundle, delegate(AssetBundle b, bool r)
		{
			result = r;
		});
		OnComplete?.Invoke(result);
	}

	private IEnumerator LoadingAssetBundles()
	{
		bool result = Application.internetReachability != NetworkReachability.NotReachable;
		onCachingDetecting?.Invoke(obj: true);
		if (Application.internetReachability != 0 && NoInternetController.HasInternet)
		{
			StopwatchMaster.Start("DownloadManifest");
			yield return AssetBundleManager.Instance.DownloadManifest(delegate
			{
			}, delegate(DLAssetBundleManifest manifest, bool r)
			{
				result = r;
			});
			if (!result)
			{
				yield return AssetBundleManager.Instance.ReadManifest(delegate
				{
				}, delegate(DLAssetBundleManifest manifestb, bool r)
				{
					result = r;
				});
			}
			if (result)
			{
				yield return AssetBundleManager.Instance.RequestAllBundlesDownload(onCachingDetecting.Invoke, onLoadingProgress.Invoke, delegate(AssetBundle manifest, bool r)
				{
					result = r;
				});
			}
			StopwatchMaster.Stop("DownloadManifest", "Total bundle downloaded lasted for {0}", delegate(string s)
			{
				BaseLogger<AssetBundleManager.Logger>.Log(s);
			});
		}
		yield return AssetBundleManager.Instance.ReadManifest(onLoadingProgress.Invoke, delegate(DLAssetBundleManifest b, bool r)
		{
			result = r;
		});
		if (result)
		{
			onSuccesComplete?.Invoke();
		}
		else
		{
			onFailedComplete?.Invoke();
		}
		onComplete?.Invoke(result);
	}
}
