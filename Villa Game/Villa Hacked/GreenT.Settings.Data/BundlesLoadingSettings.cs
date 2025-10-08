using System;
using StripClub.Extensions;
using UnityEngine;

namespace GreenT.Settings.Data;

public abstract class BundlesLoadingSettings : BundlesLoadingSettingsBase
{
	[field: SerializeField]
	protected BundleSettings BundleSettings { get; private set; }

	public override string BundleUrl(string bundleName)
	{
		return ExtensionMethods.PathCombineUnixStyle(BundlesRoot, bundleName);
	}

	public override string BundleUrl(BundleType type)
	{
		string bundleSubDir = GetBundleSubDir(type);
		return ExtensionMethods.PathCombineUnixStyle(BundlesRoot, bundleSubDir);
	}

	public override string GetBundleSubDir(BundleType type)
	{
		try
		{
			return BundleSettings.Object[type];
		}
		catch (Exception innerException)
		{
			throw new Exception("<color=#cd00cd>" + type.ToString() + "</color> was not present in BundleType dictionary", innerException);
		}
	}
}
