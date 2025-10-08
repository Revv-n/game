namespace Merge.ResourcesManagement;

public static class EditorFolder
{
	public class Art
	{
		public static string ItemIcons => BaseAssetBundleDirectory + "rooms/story/Room{0}/ObjectIcons/";

		public static string ItemSprites => BaseAssetBundleDirectory + "rooms/story/Room{0}/RoomObjects/";

		public static string Sprites => BaseAssetBundleDirectory + "rooms/story/Room{0}/";

		public static string RoomPreview => BaseAssetBundleDirectory + "rooms/common/PreviewRoom/";
	}

	public class Configs
	{
		public static string Directory => BaseAssetBundleDirectory + "rooms/story/Room{0}/Configs/";

		public static string RoomConfigs => Directory + "RoomConfigs/";

		public static string CommonConfigs => BaseAssetBundleDirectory + "rooms/common/";
	}

	public class Balance
	{
		public static string Directory => BaseAssetBundleDirectory + "content/balance/";
	}

	public class Dialogs
	{
		public static string Directory => BaseAssetBundleDirectory + "rooms/story/Room{0}/Dialogs/";
	}

	public class Localization
	{
		public static string Directory => BaseAssetBundleDirectory + "content/localization/";

		public static string Locales => Directory + "Locales";
	}

	public class Events
	{
		public static string Directory => BaseAssetBundleDirectory + "events/";

		public static string Balance(string eventName)
		{
			return Directory + eventName + "/balance/";
		}

		public static string RemoteConfig(string eventName)
		{
			return Balance(eventName) + "RemoteConfigs/";
		}

		public static string Config(string eventName)
		{
			return Balance(eventName) + "/";
		}

		public static string Sprite(string eventName)
		{
			return Directory + eventName + "/";
		}
	}

	public static string BaseAssetBundleDirectory => "Assets/Editor Default Resources/AssetBundles/";
}
