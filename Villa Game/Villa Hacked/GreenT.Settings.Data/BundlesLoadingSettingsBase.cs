using UnityEngine;

namespace GreenT.Settings.Data;

public abstract class BundlesLoadingSettingsBase : ScriptableObject, IBundleUrlResolver
{
	public abstract string BundlesRoot { get; }

	public abstract string BundleUrl(BundleType type);

	public abstract string BundleUrl(string bundle);

	public abstract string GetBundleSubDir(BundleType type);
}
