using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

public static class PlatformsUtility
{
	public static RuntimePlatform GetCurrentPlatfrom()
	{
		return RuntimePlatform.WindowsPlayer;
	}

	public static string GetCurrentPlatformFolderName()
	{
		return GetPlatfromFolderName(GetCurrentPlatfrom());
	}

	public static string GetPlatfromFolderName(RuntimePlatform platfrom)
	{
		return platfrom switch
		{
			RuntimePlatform.Android => "Android", 
			RuntimePlatform.IPhonePlayer => "iOS", 
			RuntimePlatform.WindowsPlayer => "Windows", 
			RuntimePlatform.OSXPlayer => "OSX", 
			_ => "", 
		};
	}
}
