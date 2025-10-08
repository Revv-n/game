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

	public IObservable<Unit> DownloadStart => Observable.AsObservable<Unit>((IObservable<Unit>)_downloadStart);

	public IObservable<Unit> DownloadComplete => Observable.AsObservable<Unit>((IObservable<Unit>)_downloadComplete);

	public LocalizationLoader(ILocalizationUrlResolver urlResolver, GetConfigUrlParameters getConfigUrlParameters)
	{
		_urlResolver = urlResolver;
		_getConfigUrlParameters = getConfigUrlParameters;
	}

	public IObservable<LocalizationDictionary> Load(string language)
	{
		return Observable.Do<LocalizationDictionary>(Observable.Select<string, LocalizationDictionary>(Observable.ContinueWith<string, string>(Observable.Do<string>((_getConfigUrlParameters != null) ? Observable.Select<ConfigurationInfo, string>(_getConfigUrlParameters.Get(), (Func<ConfigurationInfo, string>)((ConfigurationInfo data) => GetPath(language, data))) : Observable.Return<string>(_urlResolver.GetLocalizationUrl(language)), (Action<string>)delegate
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			_downloadStart.OnNext(Unit.Default);
		}), (Func<string, IObservable<string>>)((string _url) => HttpRequestExecutor.GetRequest(_url))), (Func<string, LocalizationDictionary>)((string data) => JsonConvert.DeserializeObject<LocalizationDictionary>(data))), (Action<LocalizationDictionary>)delegate
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
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
