using System;
using GreenT.Net;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Data;

public class ConfigFolderLoader : ILoader<ConfigParser.Folder>
{
	private readonly IConfigUrlResolver urlResolver;

	private readonly GetConfigUrlParameters getConfigUrlParameters;

	public ConfigFolderLoader(IConfigUrlResolver urlResolver, GetConfigUrlParameters getConfigUrlParameters)
	{
		this.urlResolver = urlResolver;
		this.getConfigUrlParameters = getConfigUrlParameters;
	}

	public IObservable<ConfigParser.Folder> Load()
	{
		IObservable<string> observable = ((getConfigUrlParameters == null) ? Observable.Return<string>(urlResolver.GetConfigUrl()) : Observable.Select<ConfigurationInfo, string>(getConfigUrlParameters.Get(), (Func<ConfigurationInfo, string>)((ConfigurationInfo _data) => string.Format(urlResolver.GetConfigUrl(), _data.game_config_version))));
		return Observable.Select<string, ConfigParser.Folder>(Observable.ContinueWith<string, string>(observable, (Func<string, IObservable<string>>)((string _url) => HttpRequestExecutor.GetRequest(_url))), (Func<string, ConfigParser.Folder>)ConfigParser.GetConfigFolder);
	}
}
