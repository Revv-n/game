using UnityEngine;

namespace GreenT.Localizations.Settings;

public abstract class LocalizationLoadingSettingsBase : ScriptableObject, ILocalizationUrlResolver
{
	public abstract string GetLocalizationUrl(string key);

	public virtual void SetBackupURL()
	{
	}
}
