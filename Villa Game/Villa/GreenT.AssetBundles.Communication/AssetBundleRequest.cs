using System;
using System.Collections;
using System.Collections.Generic;
using GreenT.HornyScapes;
using UnityEngine;
using UnityEngine.Networking;

namespace GreenT.AssetBundles.Communication;

public abstract class AssetBundleRequest : BaseFileRequest<AssetBundleResponse>
{
	protected AssetBundleRequestData data;

	protected AssetBundle previewBundle;

	public const string Error_Message_Duplicate_Bundle = "Can't be loaded because another AssetBundle with the same files is already loaded.";

	public static Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

	public string BundleUrl => data.bundleUrl;

	public bool isRemovePreviewBundle => previewBundle != null;

	protected AssetBundleRequest(AssetBundleRequestData data, AssetBundle previewBundle)
	{
		this.data = data;
		this.previewBundle = previewBundle;
		base.Response = new AssetBundleResponse();
	}

	protected abstract bool TrySetBundleFromCache(out CachedAssetBundle cachedAssetBundle);

	protected override uint GetRetryCount()
	{
		return data.retryCount;
	}

	public override IEnumerator Send()
	{
		yield return null;
	}

	protected IEnumerator DownloadManifest()
	{
		yield return DownloadManifest(data.manifestUrl, GetBundleInfo);
	}

	protected IEnumerator DownloadManifest(string url, Action<UnityWebRequest> onSuccess)
	{
		Uri uri = CreateUri(url);
		if (HasError)
		{
			Abort();
		}
		else
		{
			yield return DownloadFile(UnityWebRequestFactory, onSuccess);
		}
		UnityWebRequest UnityWebRequestFactory()
		{
			return UnityWebRequest.Get(uri);
		}
	}

	protected void GetBundleInfo(UnityWebRequest unityWebRequest)
	{
		string[] array = unityWebRequest.downloadHandler.text.Split("\n".ToCharArray());
		if (array.Length >= 6)
		{
			SetBundleInfoToResponse(array);
			return;
		}
		string message = "Asset bundle with path " + data.manifestUrl + " has empty manifest files. Check path or rebuild AssetBundles.\n";
		HandleException(message);
	}

	protected void SetBundleInfoToResponse(string[] splittedManifest)
	{
		string text = splittedManifest[5];
		try
		{
			Hash128 bundleHash = Hash128.Parse(text.Split(':')[1].Trim());
			base.Response.info = new BundleBuildInfo(data.name, bundleHash, 0u, "", default(DateTime), new List<AssetDateInfo>());
		}
		catch (Exception exception)
		{
			string message = "Exception on try to parse hashrow: " + text + "\n";
			HandleException(message, exception);
		}
	}

	protected IEnumerator DownloadBundle()
	{
		Uri uri = CreateUri(data.bundleUrl);
		CachedAssetBundle cachedAssetBundle;
		if (HasError)
		{
			Abort();
		}
		else if (!TrySetBundleFromCache(out cachedAssetBundle))
		{
			yield return DownloadFile(UnityWebRequestFactory, SetBundleToResponse);
		}
		UnityWebRequest UnityWebRequestFactory()
		{
			return UnityWebRequestAssetBundle.GetAssetBundle(uri, cachedAssetBundle);
		}
	}

	protected AssetBundle GetBundleFromRequest(UnityWebRequest unityWebRequest)
	{
		AssetBundle result = null;
		try
		{
			result = DownloadHandlerAssetBundle.GetContent(unityWebRequest);
		}
		catch (Exception exception)
		{
			string message = "Can't be loaded because another AssetBundle with the same files is already loaded.";
			HandleException(message, exception);
		}
		return result;
	}

	protected void SetBundleToResponse(UnityWebRequest unityWebRequest)
	{
		AssetBundle bundleFromRequest = GetBundleFromRequest(unityWebRequest);
		if (bundleFromRequest != null && !HasError)
		{
			base.Response.bundle = bundleFromRequest;
		}
		else if (bundleFromRequest == null)
		{
			base.Error = new ArgumentNullException("Bundle by path is null: \"" + data.bundleUrl + "\"");
			HandleException("", base.Error);
		}
	}
}
