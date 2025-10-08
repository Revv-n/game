using GreenT.Settings.Data;
using UnityEngine;

namespace GreenT.Localizations.Settings;

[CreateAssetMenu(menuName = "GreenT/Connection Settings/Localization/Local")]
public class LocalizationLoadingSettingsLocal : LocalizationLoadingSettingsBase
{
	[SerializeField]
	private string _path;

	public override string GetLocalizationUrl(string key)
	{
		return ConfigLocalLoadingSettings.AddPathToProject(_path) + "/" + key + ".json";
	}
}
