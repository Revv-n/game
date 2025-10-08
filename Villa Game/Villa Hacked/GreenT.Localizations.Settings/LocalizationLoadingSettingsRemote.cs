using StripClub.Utility;
using UnityEngine;

namespace GreenT.Localizations.Settings;

[CreateAssetMenu(menuName = "GreenT/Connection Settings/Localization/Remote")]
public class LocalizationLoadingSettingsRemote : LocalizationLoadingSettingsBase
{
	private const string LocalizationRoot = "Localization";

	[SerializeField]
	private string _localizationUrl;

	[SerializeField]
	[ReadOnly]
	private string backupUrl;

	public override void SetBackupURL()
	{
	}

	public override string GetLocalizationUrl(string key)
	{
		return _localizationUrl + "/Localization/" + key;
	}
}
