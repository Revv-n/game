using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Merge.DLPlugins.AssetBundles;

public class RequestLoading : AssetBundleOperation
{
	private int count_total;

	private int count_step;

	private Coroutine m_AsyncLoadingRoutine;

	private bool m_AsyncBundleLoadingInProgress;

	private float progress_total;

	private float progress_step;

	public RequestLoading(ref AssetBundleCollection collection, AssetBundleDelegates delegates, MonoBehaviour mono_behaviour)
	{
		asset_bundle_collection = collection;
		asset_bundle_delegates = delegates;
		owner = mono_behaviour;
	}

	public override AssetBundle LoadBundle(BundleName bundle_name, BundleCompletionCallback completion_callback = null)
	{
		return null;
	}

	public override Coroutine AsyncBundleOperation(BundleName bundle_name, BundleCompletionCallback completion_callback = null)
	{
		if (asset_bundle_collection == null)
		{
			completion_callback?.Invoke(null, result: false);
			return WaitOfFrame();
		}
		if (asset_bundle_collection.Manifest == null)
		{
			Debug.LogError("Нет манифеста");
			completion_callback?.Invoke(null, result: false);
			return WaitOfFrame();
		}
		List<DLAssetBundleObject> list = new List<DLAssetBundleObject>();
		list.AddRange(asset_bundle_collection.Manifest["content"]);
		list.AddRange(asset_bundle_collection.Manifest["rooms"].Where((DLAssetBundleObject x) => x.Name.Equals("common")));
		int i;
		for (i = 1; i <= 3; i++)
		{
			list.AddRange(asset_bundle_collection.Manifest["rooms"].Where((DLAssetBundleObject x) => x.Name.Equals($"story{i}")));
		}
		if (asset_bundle_collection.Manifest.ContainsKey("events"))
		{
			list.AddRange(asset_bundle_collection.Manifest["events"]);
		}
		if (asset_bundle_collection.Manifest.ContainsKey(bundle_name))
		{
			list.AddRange(asset_bundle_collection.Manifest[bundle_name.ToString()]);
		}
		if (asset_bundle_collection.Manifest["rooms"].Any((DLAssetBundleObject x) => x.Name.Equals(bundle_name)))
		{
			DLAssetBundleObject item = asset_bundle_collection.Manifest["rooms"].First((DLAssetBundleObject x) => x.Name.Equals(bundle_name));
			list.Add(item);
		}
		DLAssetBundleObject dLAssetBundleObject = list.FirstOrDefault((DLAssetBundleObject x) => x.Name == bundle_name);
		m_AsyncLoadingRoutine = owner.StartCoroutine(DownloadBundle(new BundleLoadingInfo(dLAssetBundleObject.Url, dLAssetBundleObject.Name, 0L, async: true, BundleLoadingType.Server), null, completion_callback, reload: false));
		return m_AsyncLoadingRoutine;
	}

	public Coroutine AsyncLoadingAllBundles(Action<bool> is_already_cached = null, BundleProgressCallback progress_callback = null, BundleCompletionCallback completion_callback = null)
	{
		asset_bundle_delegates.messegeLogError?.Invoke("Null Manifest!");
		completion_callback?.Invoke(null, result: false);
		return null;
	}

	private Coroutine WaitOfFrame()
	{
		return owner.StartCoroutine(Wait());
	}

	private IEnumerator Wait()
	{
		yield return new WaitForEndOfFrame();
	}

	private Coroutine StartAsyncBundleDownloading(BundleProgressCallback progress_callback = null, BundleCompletionCallback completion_callback = null, params BundleLoadingInfo[] infoes)
	{
		if (infoes != null)
		{
			asset_bundle_collection.EnqueueAsyncBundleLoading(infoes);
		}
		count_total = asset_bundle_collection.LoadingQueue.Count;
		count_step = 1;
		if (!m_AsyncBundleLoadingInProgress)
		{
			m_AsyncLoadingRoutine = owner.StartCoroutine(AsyncBundleOperationRoutine(progress_callback, completion_callback));
		}
		return m_AsyncLoadingRoutine;
	}

	protected IEnumerator AsyncBundleOperationRoutine(BundleProgressCallback progress_callback, BundleCompletionCallback completion_callback)
	{
		m_AsyncBundleLoadingInProgress = true;
		asset_bundle_delegates.bundleLoadingStarted?.Invoke();
		progress_step = 1f / (float)asset_bundle_collection.LoadingQueue.Count;
		while (asset_bundle_collection.LoadingQueue.Any())
		{
			BundleLoadingInfo bundleLoadingInfo = asset_bundle_collection.LoadingQueue.Dequeue();
			if (bundleLoadingInfo != null && !string.IsNullOrEmpty(bundleLoadingInfo.Name))
			{
				asset_bundle_delegates.messegeLog?.Invoke($"url [{bundleLoadingInfo.Url}] bundle [{bundleLoadingInfo.Name}] start load; bundle count: [{asset_bundle_collection.LoadingQueue.Count + 1}]");
				owner.StartCoroutine(DownloadBundle(bundleLoadingInfo, progress_callback, completion_callback, reload: true));
			}
		}
		while (progress_total < 0.99f)
		{
			yield return new WaitForEndOfFrame();
		}
		m_AsyncBundleLoadingInProgress = false;
		asset_bundle_delegates.bundleLoadingCompleted?.Invoke();
		m_AsyncLoadingRoutine = null;
	}

	private IEnumerator DownloadBundle(BundleLoadingInfo info, BundleProgressCallback progress_callback, BundleCompletionCallback completion_callback, bool reload)
	{
		Hash128 hash = (asset_bundle_collection.Manifest ? asset_bundle_collection.Manifest.GetAssetBundleHash(info.Name) : default(Hash128));
		double norm_size = (double)info.Size / 1000000.0;
		string d_unit = "MB";
		if (norm_size <= 0.5)
		{
			norm_size = (double)info.Size / 1000.0;
			d_unit = "KB";
		}
		yield return DownloadBundle(info.Url, info.Name, hash, delegate
		{
		}, delegate(AssetBundle bundle, bool result)
		{
			progress_total = Mathf.Clamp(progress_total + progress_step, 0f, 1f);
			_ = count_total;
			_ = count_step;
			count_step = Mathf.Clamp(count_step + 1, 0, count_total);
			progress_callback?.Invoke(progress_total);
			asset_bundle_delegates.asyncLoadingBundleCallback?.Invoke(count_step, count_total, progress_total, (float)norm_size, d_unit, info.Name);
			info.InvokeOnComplete(bundle, result);
			completion_callback?.Invoke(bundle, result);
		}, reload);
	}

	private IEnumerator DownloadBundle(string full_url, string bundle_name, Hash128 hash, BundleProgressCallback progress_callback = null, BundleCompletionCallback completion_callback = null, bool reload = false)
	{
		if (!reload && asset_bundle_collection.TryGetBundle(bundle_name, out var bundle) && (bool)bundle)
		{
			progress_callback?.Invoke(1f);
			completion_callback?.Invoke(bundle, result: true);
			yield break;
		}
		if (asset_bundle_collection.TryGetBundle(bundle_name, out var bundle2) && (bool)bundle2)
		{
			AssetBundleManager.Instance.UnloadBundle(bundle_name);
		}
		if (string.IsNullOrEmpty(full_url))
		{
			asset_bundle_delegates.messegeLogError?.Invoke("Error url!");
			completion_callback(null, result: false);
			yield break;
		}
		if (string.IsNullOrEmpty(bundle_name))
		{
			asset_bundle_delegates.messegeLogError?.Invoke("Error bundleName!");
			completion_callback(null, result: false);
			yield break;
		}
		yield return new WaitUntil(() => Caching.ready);
		BundleLoading bundle_loading = new BundleLoading();
		asset_bundle_collection.AddLoadingBundle(bundle_name, bundle_loading);
		string uri = "https://bundle.match3tv.com" + full_url;
		UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, hash);
		request.timeout = 600;
		UnityWebRequestAsyncOperation request_routine = request.SendWebRequest();
		do
		{
			progress_callback?.Invoke(request_routine.progress, 1f, "MB");
			bundle_loading.OnProggress(request_routine.progress, 1f, "MB");
			yield return null;
		}
		while (!request_routine.isDone);
		progress_callback?.Invoke(1f, 1f, "MB");
		bundle_loading.OnProggress(1f, 1f, "MB");
		yield return null;
		if (request.isNetworkError || request.isHttpError)
		{
			asset_bundle_delegates.messegeLogError?.Invoke("Error [" + request.error + "] while downloading [" + bundle_name + "]");
			completion_callback?.Invoke(null, result: false);
			bundle_loading.OnComplete(null, result: false);
			asset_bundle_collection.RemoveLoadingundle(bundle_name);
			asset_bundle_delegates.connectionError?.Invoke();
		}
		else
		{
			AssetBundle content = DownloadHandlerAssetBundle.GetContent(request);
			if (reload)
			{
				asset_bundle_collection.UnloadBundle(bundle_name, unload_all_loaded_objects: true);
			}
			asset_bundle_delegates.messegeLog?.Invoke("Successful download for [" + bundle_name + "]");
			asset_bundle_collection.AddRefBundle(bundle_name, content);
			asset_bundle_collection.RemoveLoadingundle(bundle_name);
			completion_callback?.Invoke(content, result: true);
			bundle_loading.OnComplete(content, result: true);
			request.Dispose();
		}
	}
}
