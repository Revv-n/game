using StripClub.Extensions;
using UnityEngine;

namespace GreenT.Settings.Data;

[CreateAssetMenu(menuName = "GreenT/Connection Settings/Config/Local", order = 1)]
public class ConfigLocalLoadingSettings : ConfigLoadingSettingsBase, IConfigUrlResolver
{
	[SerializeField]
	protected string rootPath;

	[SerializeField]
	protected string configBatch;

	public override string GetConfigUrl()
	{
		return AddPathToProject(rootPath) + "/" + configBatch;
	}

	public static string AddPathToProject(string path)
	{
		return ExtensionMethods.PathCombineUnixStyle("file://", Application.dataPath, path);
	}
}
