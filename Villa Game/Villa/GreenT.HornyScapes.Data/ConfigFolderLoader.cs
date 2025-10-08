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
		IObservable<string> source = ((getConfigUrlParameters == null) ? Observable.Return(urlResolver.GetConfigUrl()) : (from _data in getConfigUrlParameters.Get()
			select string.Format(urlResolver.GetConfigUrl(), _data.game_config_version)));
		return source.ContinueWith((string _url) => HttpRequestExecutor.GetRequest(_url)).Select(ConfigParser.GetConfigFolder);
	}
}
