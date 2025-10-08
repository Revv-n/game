using Merge.DLPlugins.AssetBundles;
using UnityEngine;

namespace BundlesManagement;

public static class Bundles
{
	public class Art
	{
		public static BundleName Bundle(string name)
		{
			return string.Format("{0}{1}", name, PlayerPrefs.GetString("atlas_quality", ".sd"));
		}
	}

	public static class Balance
	{
		public static readonly BundleName Bundle = string.Concat(Directory, "balance");

		private static BundleName Directory => "content/";
	}

	public class Localization
	{
		private static BundleName Directory => "";

		public static BundleName Bundle => string.Concat(Directory, "localization");
	}
}
