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
		return _localizationProvider.OnLocalizationChange.Select((LocalizationDictionary x) => Text(key)).ToReadOnlyReactiveProperty(Text(key));
	}
}
