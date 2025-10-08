using UnityEngine;

namespace GreenT.Settings.Data;

public abstract class ConfigLoadingSettingsBase : ScriptableObject, IConfigUrlResolver
{
	public abstract string GetConfigUrl();

	public virtual void SetBackupURL()
	{
	}
}
