using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.Localizations;

public class LanguageSelector : IInitializable
{
	private readonly LocalizationState _localizationState;

	public LanguageSelector(LocalizationState localizationState)
	{
		_localizationState = localizationState;
	}

	public void Initialize()
	{
		SetLanguage(_localizationState.TryGetLocalLanguage(out var language) ? language : string.Empty);
	}

	private void SetLanguage(string language)
	{
		if (language.IsNullOrEmpty())
		{
			_localizationState.UpdateLanguage(GetLanguage());
			return;
		}
		_localizationState.UpdateLanguage(language);
		_localizationState.UpdateLocalLanguage(language);
	}

	private static string GetLanguage()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		switch (systemLanguage)
		{
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
			return "ZH-HANS-CN";
		case SystemLanguage.ChineseTraditional:
			return "ZH-HANS-TW";
		default:
			return systemLanguage.ToString();
		}
	}
}
