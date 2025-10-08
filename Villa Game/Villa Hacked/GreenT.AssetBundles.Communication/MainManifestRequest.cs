using System;
using System.Collections;
using System.Collections.Generic;
using GreenT.HornyScapes;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace GreenT.AssetBundles.Communication;

public class MainManifestRequest : BaseFileRequest<MainManifestResponse>
{
	private MainManifestRequestData _data;

	protected override uint GetRetryCount()
	{
		return _data.retryCount;
	}

	public MainManifestRequest(MainManifestRequestData data)
	{
		_data = data;
		base.Response = new MainManifestResponse();
	}

	public override IEnumerator Send()
	{
		yield return LoadMainManifest();
	}

	private IEnumerator LoadMainManifest()
	{
		Uri uri = CreateUri(_data.fileUrl);
		if (HasError)
		{
			Abort();
		}
		else
		{
			yield return DownloadFile(UnityWebRequestFactory, SetListToResponse);
		}
		UnityWebRequest UnityWebRequestFactory()
		{
			return UnityWebRequest.Get(uri);
		}
	}

	private void SetListToResponse(UnityWebRequest webRequest)
	{
		try
		{
			List<BuildMainInfo> list = new List<BuildMainInfo>();
			List<BuildMainInfo> collection = JsonConvert.DeserializeObject<List<BuildMainInfo>>(unityWebRequest.downloadHandler.text);
			list.AddRange(collection);
			base.Response.info = list;
		}
		catch (Exception exception)
		{
			string message = "Exception on try to parse main manifest: \n";
			HandleException(message, exception);
		}
	}
}
