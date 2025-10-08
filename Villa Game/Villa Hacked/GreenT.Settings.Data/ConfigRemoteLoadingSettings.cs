using StripClub.Utility;
using UnityEngine;

namespace GreenT.Settings.Data;

[CreateAssetMenu(menuName = "GreenT/Connection Settings/Config/Remote", order = 2)]
public class ConfigRemoteLoadingSettings : ConfigLoadingSettingsBase, IConfigUrlResolver
{
	[SerializeField]
	protected string configsBatchRequestUrl;

	[SerializeField]
	[ReadOnly]
	private string backupUrl;

	public override void SetBackupURL()
	{
	}

	public override string GetConfigUrl()
	{
		return configsBatchRequestUrl;
	}
}
