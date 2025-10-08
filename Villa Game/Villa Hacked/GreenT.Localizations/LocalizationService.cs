using System;
using UniRx;

namespace GreenT.Localizations;

public class LocalizationService
{
	private readonly LocalizationProvider _localizationProvider;

	public LocalizationService(LocalizationProvider localizationProvider)
	{
		_localizationProvider = localizationProvider;
	}

	public string Text(string key)
	{
		return _localizationProvider.TryGetValue(key);
	}

	public IReadOnlyReactiveProperty<string> ObservableText(string key)
	{
		string.IsNullOrEmpty(key);
		return (IReadOnlyReactiveProperty<string>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<string>(Observable.Select<LocalizationDictionary, string>(_localizationProvider.OnLocalizationChange, (Func<LocalizationDictionary, string>)((LocalizationDictionary x) => Text(key))), Text(key));
	}
}
