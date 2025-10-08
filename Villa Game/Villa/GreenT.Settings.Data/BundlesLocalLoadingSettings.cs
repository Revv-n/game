using StripClub.Extensions;
using UnityEngine;

namespace GreenT.Settings.Data;

[CreateAssetMenu(menuName = "GreenT/Connection Settings/Bundles/Local", order = 1)]
public class BundlesLocalLoadingSettings : BundlesLoadingSettings
{
	[SerializeField]
	protected string bundlesRootPath = string.Empty;

	public override string BundlesRoot => AddPathToProject(bundlesRootPath);

	public static string AddPathToProject(string path)
	{
		return ExtensionMethods.PathCombineUnixStyle("file://", Application.streamingAssetsPath, path);
	}
}
