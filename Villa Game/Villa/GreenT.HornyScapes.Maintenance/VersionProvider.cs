using System;
using GreenT.Net;
using GreenT.Settings.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Maintenance;

public class VersionProvider : IInitializable, IDisposable
{
	private GetConfigsVersionRequest _getConfigsVersionRequest;

	private IDisposable _subscription;

	private string _version;

	public VersionProvider(GetConfigsVersionRequest getConfigsVersionRequest)
	{
		_getConfigsVersionRequest = getConfigsVersionRequest;
	}

	public void Initialize()
	{
		_subscription = _getConfigsVersionRequest.Get().Subscribe(delegate(Response<ConfigurationInfo> response)
		{
			HandleResponse(response);
		});
	}

	public string GetUrlDownloadPage()
	{
		return "https://apk2.b-cdn.net/HaremVilla_" + _version + ".apk";
	}

	private void HandleResponse(Response<ConfigurationInfo> response)
	{
		_version = response.Data.curr_version_client;
	}

	public void Dispose()
	{
		_subscription?.Dispose();
	}
}
