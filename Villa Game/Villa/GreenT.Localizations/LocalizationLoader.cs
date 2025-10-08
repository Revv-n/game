using System;
using GreenT.Localizations.Settings;
using GreenT.Net;
using GreenT.Settings.Data;
using Newtonsoft.Json;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.Localizations;

public class LocalizationLoader : ILoader<string, LocalizationDictionary>, IDisposable
{
	private readonly ILocalizationUrlResolver _urlResolver;

	private readonly GetConfigUrlParameters _getConfigUrlParameters;

	private Subject<Unit> _downloadComplete = new Subject<Unit>();

	private Subject<Unit> _downloadStart = new Subject<Unit>();

	public IObservable<Unit> DownloadStart => _downloadStart.AsObservable();

	public IObservable<Unit> DownloadComplete => _downloadComplete.AsObservable();

	public LocalizationLoader(ILocalizationUrlResolver urlResolver, GetConfigUrlParameters getConfigUrlParameters)
	{
		_urlResolver = urlResolver;
		_getConfigUrlParameters = getConfigUrlParameters;
	}

	public IObservable<LocalizationDictionary> Load(string language)
	{
		return (from data in ((_getConfigUrlParameters != null) ? (from data in _getConfigUrlParameters.Get()
				select GetPath(language, data)) : Observable.Return(_urlResolver.GetLocalizationUrl(language))).Do(delegate
			{
				_downloadStart.OnNext(Unit.Default);
			}).ContinueWith((string _url) => HttpRequestExecutor.GetRequest(_url))
			select JsonConvert.DeserializeObject<LocalizationDictionary>(data)).Do(delegate
		{
			_downloadComplete.OnNext(Unit.Default);
		});
	}

	private string GetPath(string language, ConfigurationInfo data)
	{
		string localizationUrl = _urlResolver.GetLocalizationUrl(language);
		string game_config_version = data.game_config_version;
		return string.Format(localizationUrl, game_config_version);
	}

	public void Dispose()
	{
		_downloadComplete.Dispose();
		_downloadStart.Dispose();
	}
}
