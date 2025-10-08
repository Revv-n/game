using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

public class Utility
{
	public const string AssetBundlesOutputPath = "AssetBundles";

	public static string GetPlatformName()
	{
		return GetPlatformForAssetBundles(Application.platform);
	}

	private static string GetPlatformForAssetBundles(RuntimePlatform platform)
	{
		return platform switch
		{
			RuntimePlatform.Android => "Android", 
			RuntimePlatform.WebGLPlayer => "WebGL", 
			RuntimePlatform.IPhonePlayer => "iOS", 
			RuntimePlatform.tvOS => "tvOS", 
			RuntimePlatform.OSXPlayer => "OSX", 
			RuntimePlatform.WindowsPlayer => "Windows", 
			_ => null, 
		};
	}
}
