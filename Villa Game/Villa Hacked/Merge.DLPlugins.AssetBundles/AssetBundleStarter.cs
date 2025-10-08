using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Merge.DLPlugins.AssetBundles;

public class AssetBundleStarter : MonoBehaviour
{
	public UnityEvent afterBundleLoaded;

	[SerializeField]
	private AssetBundleManager bundleManager;

	private AssetBundle bundle;

	private InitialBundleLoading bundleLoader;

	private InitialBundleLoadingViewControl bundleLoaderView;

	[Header("For Tests")]
	[SerializeField]
	private string bundleName;

	[SerializeField]
	private string spriteName;

	[SerializeField]
	private Image image;

	[SerializeField]
	private int currentRoom;

	[SerializeField]
	private List<bool> rooms;

	private string SavedVersion
	{
		get
		{
			return PlayerPrefs.GetString("app_version", "");
		}
		set
		{
			PlayerPrefs.SetString("app_version", Application.version);
			PlayerPrefs.Save();
		}
	}

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		Initialize();
	}

	private void Initialize()
	{
		bundleLoaderView = new InitialBundleLoadingViewControl(SetStatusLabelText, ViewProgress, SetUnitLabelText, SetBundleCountLabelText);
		bundleLoader = new InitialBundleLoading(this);
		InitialBundleLoading initialBundleLoading = bundleLoader;
		initialBundleLoading.onSuccesComplete = (Action)Delegate.Combine(initialBundleLoading.onSuccesComplete, new Action(AtLoadSuccess));
		InitialBundleLoading initialBundleLoading2 = bundleLoader;
		initialBundleLoading2.onFailedComplete = (Action)Delegate.Combine(initialBundleLoading2.onFailedComplete, new Action(AtLoadFail));
		InitialBundleLoading initialBundleLoading3 = bundleLoader;
		initialBundleLoading3.onComplete = (Action<bool>)Delegate.Combine(initialBundleLoading3.onComplete, new Action<bool>(AtComplete));
		InitialBundleLoading initialBundleLoading4 = bundleLoader;
		initialBundleLoading4.onFailedComplete = (Action)Delegate.Combine(initialBundleLoading4.onFailedComplete, new Action(OnFailedLoadingBundles));
		InitialBundleLoading initialBundleLoading5 = bundleLoader;
		initialBundleLoading5.onLoadingProgress = (Action<float, float, string>)Delegate.Combine(initialBundleLoading5.onLoadingProgress, new Action<float, float, string>(bundleLoaderView.OnDownloadingBundles));
		InitialBundleLoading initialBundleLoading6 = bundleLoader;
		initialBundleLoading6.onCachingDetecting = (Action<bool>)Delegate.Combine(initialBundleLoading6.onCachingDetecting, new Action<bool>(bundleLoaderView.OnCachingDetecting));
		InitialBundleLoading initialBundleLoading7 = bundleLoader;
		initialBundleLoading7.OnBundleProgress = (Action<int, int, float, float, string, string>)Delegate.Combine(initialBundleLoading7.OnBundleProgress, new Action<int, int, float, float, string, string>(bundleLoaderView.OnBundleProgress));
		CheckVersionChange(out var mustClearCache, out var mustHaveInternet);
		if (mustClearCache)
		{
			Debug.Log("AssetBundles >>> Clearing cache");
			StopwatchMaster.Start("ClearStreamingAssets");
			if (AssetBundleManager.Instance.TryClearCachedStreamingAssetsBundles())
			{
				StopwatchMaster.Stop("ClearStreamingAssets", "Time to clearing StreamingAssets Bundles in Cache: {0}", delegate(string s)
				{
					BaseLogger<AssetBundleManager.Logger>.Log(s);
				});
			}
			else
			{
				StopwatchMaster.Stop("ClearStreamingAssets");
			}
		}
		if (mustHaveInternet)
		{
			Debug.Log("AssetBundles >>> Need internet");
			CheckInternetReachibelity();
		}
		else
		{
			Debug.Log("AssetBundles >>> Simple load bundles at start");
			LoadBundles();
		}
		void AtComplete(bool result)
		{
			OnSuccessfulLoadingBundles();
		}
		void AtLoadFail()
		{
			bundleLoaderView.OnErrorDownloadingBundles();
		}
		void AtLoadSuccess()
		{
			SavedVersion = Application.version;
			bundleLoaderView.OnSuccesComplete();
		}
	}

	private void CheckVersionChange(out bool mustClearCache, out bool mustHaveInternet)
	{
		string savedVersion = SavedVersion;
		mustClearCache = false;
		mustHaveInternet = false;
		if (AssetBundleManager.Instance.CacheIsEmpty())
		{
			Debug.Log("AssetBundles: Its new user");
			SavedVersion = Application.version;
		}
		else if (!(savedVersion == Application.version))
		{
			mustClearCache = true;
			mustHaveInternet = true;
			if (string.IsNullOrEmpty(savedVersion))
			{
				Debug.Log("AssetBundles: Its old user");
			}
			else if (savedVersion != Application.version)
			{
				Debug.Log("AssetBundles: Update from " + savedVersion);
			}
		}
	}

	private void CheckInternetReachibelity()
	{
		if (NoInternetController.NoInternet)
		{
			Controller<NoInternetController>.Instance.ShowNoInternetWindow(CheckInternetReachibelity);
			Debug.Log("AssetBundles >>> No internet");
		}
		else
		{
			Debug.Log("AssetBundles >>> Load bundles at internet reached");
			LoadBundles();
		}
	}

	private void OnDestroy()
	{
		bundleLoader.ClearAction();
	}

	private void LoadBundles()
	{
		Debug.Log("AssetBundles >>> Becomes loading");
		bundleLoader?.LoadAssetBundles();
	}

	private void OnSuccessfulLoadingBundles()
	{
		StopwatchMaster.Stop("LoadingBundles", "Time to loading Bundles: {0}", delegate(string s)
		{
			BaseLogger<AssetBundleManager.Logger>.Log(s);
		});
		StartCoroutine(SuccessfulLoadingBundles());
	}

	private void OnFailedLoadingBundles()
	{
		Debug.LogError("Ошибка загрузки бандла!");
	}

	private IEnumerator SuccessfulLoadingBundles()
	{
		yield return new WaitForEndOfFrame();
		bundleLoader.ClearAction();
		SwitchScenePanel.SetBundleCountLabelText("");
		SwitchScenePanel.SetUnitLabelText("");
		yield return new WaitForEndOfFrame();
		yield return StartScreenLoading();
	}

	private IEnumerator StartScreenLoading()
	{
		AssetBundleManager.Instance.AllBundleInited = true;
		yield return null;
		afterBundleLoaded?.Invoke();
	}

	private void SetStatusLabelText(string text)
	{
		SwitchScenePanel.SetStatusLabelText(text);
	}

	private void SetUnitLabelText(string text)
	{
		SwitchScenePanel.SetUnitLabelText(text);
	}

	private void SetBundleCountLabelText(string text)
	{
		SwitchScenePanel.SetBundleCountLabelText(text);
	}

	protected void ViewProgress(float value)
	{
		SwitchScenePanel.SetSliderValue(value);
	}

	[ContextMenu("TestRoomBundleManagment")]
	private void TestRoomBundleManagment()
	{
	}

	[ContextMenu("TestLoad")]
	private void TestLoad()
	{
		StartCoroutine(LoadBundle(bundleName));
	}

	[ContextMenu("TestLoadSprite")]
	private void TestLoadSprite()
	{
		if ((UnityEngine.Object)(object)bundle == null)
		{
			Debug.LogError("Бандл не загружен " + bundleName);
			return;
		}
		Sprite sprite = bundle.LoadAsset<Sprite>(spriteName);
		if (sprite != null)
		{
			image.sprite = sprite;
			image.preserveAspect = true;
		}
		else
		{
			Debug.LogError("Нет спрайта " + spriteName);
		}
	}

	[ContextMenu("Clear")]
	private void Clear()
	{
		bundleManager.Clear(bundleName);
	}

	private IEnumerator LoadBundle(string bundleName)
	{
		yield return AssetBundleManager.Instance.RequestBundleDownload(bundleName, delegate(AssetBundle bundle, bool result)
		{
			OnBundleLoaded(bundle, bundleName, result);
		});
	}

	private void OnBundleLoaded(AssetBundle bundle, string bundleName, bool result)
	{
		if ((UnityEngine.Object)(object)bundle == null)
		{
			Debug.Log($"Бандл не загрузился result={result}");
			return;
		}
		this.bundle = bundle;
		Debug.Log($"Бандл загрузился result={result}");
	}
}
